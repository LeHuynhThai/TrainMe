using AutoMapper;
using TrainMe.Core.DTOs;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;
using TrainMe.Core.Interfaces.Services.WorkoutItem;

namespace TrainMe.Services.WorkoutItem;

/// <summary>
/// Service thực hiện các thao tác CRUD cơ bản cho thực thể WorkoutItem.
/// Tuân theo các nguyên tắc SOLID và mô hình Clean Architecture.
/// </summary>
public class WorkoutItemService : IWorkoutItemService
{
    private readonly IWorkoutItemRepository _repository;
    private readonly IWorkoutItemQueryRepository _queryRepository;
    private readonly IMapper _mapper;

    public WorkoutItemService(
        IWorkoutItemRepository repository,
        IWorkoutItemQueryRepository queryRepository,
        IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Tạo mới một mục tập luyện với kiểm tra hợp lệ và quy tắc nghiệp vụ.
    /// </summary>
    /// <param name="userId">ID người dùng sở hữu mục tập</param>
    /// <param name="request">Thông tin yêu cầu tạo mới</param>
    /// <returns>ApiResponse chứa WorkoutItemDto vừa được tạo</returns>
    public async Task<ApiResponse<WorkoutItemDto>> CreateWorkoutItemAsync(int userId, CreateWorkoutItemRequest request)
    {
        try
        {
            // Kiểm tra hợp lệ các tham số đầu vào
            if (userId <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid user ID");

            if (request == null)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Request cannot be null");



            // Ánh xạ request sang entity
            var workoutItem = _mapper.Map<Core.Entities.WorkoutItem>(request);
            workoutItem.UserId = userId;

            // Tạo entity thông qua repository
            var createdItem = await _repository.CreateAsync(workoutItem);

            // Ánh xạ ngược sang DTO để trả về
            var dto = _mapper.Map<WorkoutItemDto>(createdItem);
            return ApiResponse<WorkoutItemDto>.SuccessResult(dto, "Workout item created successfully");
        }
        catch (Exception ex)
        {
            // Ghi log ngoại lệ trong triển khai thực tế
            return ApiResponse<WorkoutItemDto>.ErrorResult($"Failed to create workout item: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy một mục tập luyện theo ID, kèm kiểm tra hợp lệ/ủy quyền cơ bản.
    /// </summary>
    /// <param name="id">ID mục tập luyện</param>
    /// <returns>ApiResponse chứa WorkoutItemDto nếu tìm thấy</returns>
    public async Task<ApiResponse<WorkoutItemDto>> GetWorkoutItemByIdAsync(int id)
    {
        try
        {
            // Kiểm tra đầu vào
            if (id <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid workout item ID");

            // Lấy entity từ repository
            var workoutItem = await _repository.GetByIdAsync(id);
            if (workoutItem == null)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Workout item not found");

            // Ánh xạ sang DTO và trả về
            var dto = _mapper.Map<WorkoutItemDto>(workoutItem);
            return ApiResponse<WorkoutItemDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ApiResponse<WorkoutItemDto>.ErrorResult($"Failed to retrieve workout item: {ex.Message}");
        }
    }

    /// <summary>
    /// Cập nhật một mục tập luyện hiện có với kiểm tra hợp lệ và ủy quyền.
    /// </summary>
    /// <param name="id">ID mục tập luyện cần cập nhật</param>
    /// <param name="userId">ID người dùng (để kiểm tra quyền sở hữu)</param>
    /// <param name="request">Dữ liệu cập nhật</param>
    /// <returns>ApiResponse chứa WorkoutItemDto sau khi cập nhật</returns>
    public async Task<ApiResponse<WorkoutItemDto>> UpdateWorkoutItemAsync(int id, int userId, UpdateWorkoutItemRequest request)
    {
        try
        {
            // Kiểm tra hợp lệ các tham số đầu vào
            if (id <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid workout item ID");

            if (userId <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid user ID");

            if (request == null)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Request cannot be null");

            // Lấy entity hiện có
            var existingItem = await _repository.GetByIdAsync(id);
            if (existingItem == null)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Workout item not found");

            // Ủy quyền: Đảm bảo người dùng sở hữu mục tập luyện
            if (existingItem.UserId != userId)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Unauthorized to update this workout item");



            // Ánh xạ dữ liệu cập nhật vào entity hiện có
            _mapper.Map(request, existingItem);

            // Cập nhật thông qua repository
            var updatedItem = await _repository.UpdateAsync(existingItem);

            // Ánh xạ sang DTO và trả về
            var dto = _mapper.Map<WorkoutItemDto>(updatedItem);
            return ApiResponse<WorkoutItemDto>.SuccessResult(dto, "Workout item updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<WorkoutItemDto>.ErrorResult($"Failed to update workout item: {ex.Message}");
        }
    }

    /// <summary>
    /// Xóa một mục tập luyện với kiểm tra ủy quyền phù hợp.
    /// </summary>
    /// <param name="id">ID mục tập luyện cần xóa</param>
    /// <param name="userId">ID người dùng (để xác thực quyền xóa)</param>
    /// <returns>ApiResponse thể hiện kết quả xóa</returns>
    public async Task<ApiResponse> DeleteWorkoutItemAsync(int id, int userId)
    {
        try
        {
            // Kiểm tra hợp lệ các tham số đầu vào
            if (id <= 0)
                return ApiResponse.ErrorResult("Invalid workout item ID");

            if (userId <= 0)
                return ApiResponse.ErrorResult("Invalid user ID");

            // Lấy entity hiện có để kiểm tra ủy quyền
            var existingItem = await _repository.GetByIdAsync(id);
            if (existingItem == null)
                return ApiResponse.ErrorResult("Workout item not found");

            // Ủy quyền: Đảm bảo người dùng sở hữu mục tập luyện
            if (existingItem.UserId != userId)
                return ApiResponse.ErrorResult("Unauthorized to delete this workout item");

            // Xóa thông qua repository
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return ApiResponse.ErrorResult("Failed to delete workout item");

            return ApiResponse.SuccessResult("Workout item deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.ErrorResult($"Failed to delete workout item: {ex.Message}");
        }
    }
}
