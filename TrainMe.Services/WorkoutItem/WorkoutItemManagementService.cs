using AutoMapper;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;
using TrainMe.Core.Interfaces.Services.WorkoutItem;

namespace TrainMe.Services.WorkoutItem;

/// <summary>
/// Service xử lý các thao tác quản lý nâng cao cho thực thể WorkoutItem.
/// Bao gồm các nghiệp vụ phức tạp như sắp xếp lại thứ tự và nhân bản mục tập luyện.
/// </summary>
public class WorkoutItemManagementService : IWorkoutItemManagementService
{
    private readonly IWorkoutItemRepository _repository;
    private readonly IWorkoutItemQueryRepository _queryRepository;
    private readonly IWorkoutItemSortRepository _sortRepository;
    private readonly IMapper _mapper;

    public WorkoutItemManagementService(
        IWorkoutItemRepository repository,
        IWorkoutItemQueryRepository queryRepository,
        IWorkoutItemSortRepository sortRepository,
        IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));
        _sortRepository = sortRepository ?? throw new ArgumentNullException(nameof(sortRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Sắp xếp lại thứ tự các mục tập luyện cho một người dùng trong một ngày cụ thể.
    /// Đảm bảo tính nhất quán dữ liệu và kiểm tra quyền sở hữu.
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <param name="dayOfWeek">Ngày trong tuần cần sắp xếp</param>
    /// <param name="itemSortOrders">Danh sách cặp <Id, SortOrder> cần cập nhật</param>
    /// <returns>Kết quả API thể hiện trạng thái sắp xếp</returns>
    public async Task<ApiResponse> ReorderWorkoutItemsAsync(int userId, Weekday dayOfWeek, Dictionary<int, int> itemSortOrders)
    {
        try
        {
            // Kiểm tra hợp lệ các tham số đầu vào
            if (userId <= 0)
                return ApiResponse.ErrorResult("Invalid user ID");

            if (itemSortOrders == null || !itemSortOrders.Any())
                return ApiResponse.ErrorResult("Sort orders cannot be empty");

            // Kiểm tra giá trị sort order
            if (itemSortOrders.Values.Any(order => order < 0))
                return ApiResponse.ErrorResult("Sort orders must be non-negative");

            // Xác thực tất cả mục thuộc về người dùng và đúng ngày chỉ định
            var workoutItemIds = itemSortOrders.Keys.ToList();
            var existingItems = await _queryRepository.GetByUserIdAndDayAsync(userId, dayOfWeek);
            var existingIds = existingItems.Select(wi => wi.Id).ToHashSet();

            // Kiểm tra tất cả ID cung cấp có tồn tại và thuộc về user/day
            var invalidIds = workoutItemIds.Where(id => !existingIds.Contains(id)).ToList();
            if (invalidIds.Any())
                return ApiResponse.ErrorResult($"Invalid workout item IDs: {string.Join(", ", invalidIds)}");

            // Thực hiện thao tác cập nhật thứ tự
            await _sortRepository.UpdateSortOrdersAsync(userId, dayOfWeek, itemSortOrders);

            return ApiResponse.SuccessResult("Workout items reordered successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.ErrorResult($"Failed to reorder workout items: {ex.Message}");
        }
    }

    /// <summary>
    /// Nhân bản một mục tập luyện sang ngày khác với đầy đủ kiểm tra hợp lệ.
    /// Tạo mục mới với thứ tự sắp xếp (SortOrder) và mốc thời gian phù hợp.
    /// </summary>
    /// <param name="id">ID mục tập luyện cần nhân bản</param>
    /// <param name="userId">ID người dùng thực hiện thao tác</param>
    /// <param name="targetDay">Ngày trong tuần sẽ nhân bản tới</param>
    /// <returns>ApiResponse chứa WorkoutItemDto sau khi nhân bản</returns>
    public async Task<ApiResponse<WorkoutItemDto>> DuplicateWorkoutItemAsync(int id, int userId, Weekday targetDay)
    {
        try
        {
            // Kiểm tra hợp lệ các tham số đầu vào
            if (id <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid workout item ID");

            if (userId <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid user ID");

            // Lấy mục tập luyện nguồn
            var sourceItem = await _repository.GetByIdAsync(id);
            if (sourceItem == null)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Source workout item not found");

            // Ủy quyền: Đảm bảo người dùng sở hữu mục tập luyện
            if (sourceItem.UserId != userId)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Unauthorized to duplicate this workout item");

            // Quy tắc nghiệp vụ: Kiểm tra mục trùng tên đã tồn tại ở ngày đích chưa
            var exists = await _queryRepository.ExistsAsync(userId, sourceItem.Name, targetDay);
            if (exists)
                return ApiResponse<WorkoutItemDto>.ErrorResult($"Workout item '{sourceItem.Name}' already exists for {targetDay}");

            // Tạo bản sao với thuộc tính mới
            var duplicateItem = new Core.Entities.WorkoutItem
            {
                UserId = userId,
                Name = sourceItem.Name,
                DayOfWeek = targetDay,
                SortOrder = await _sortRepository.GetNextSortOrderAsync(userId, targetDay),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            // Tạo bản ghi thông qua repository
            var createdItem = await _repository.CreateAsync(duplicateItem);

            // Ánh xạ sang DTO và trả về
            var dto = _mapper.Map<WorkoutItemDto>(createdItem);
            return ApiResponse<WorkoutItemDto>.SuccessResult(dto, $"Workout item duplicated to {targetDay} successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<WorkoutItemDto>.ErrorResult($"Failed to duplicate workout item: {ex.Message}");
        }
    }
}
