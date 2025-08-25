using AutoMapper;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;
using TrainMe.Core.Interfaces.Services.WorkoutItem;

namespace TrainMe.Services.WorkoutItem;

/// <summary>
/// Service thực hiện các thao tác truy vấn (read-only) cho thực thể WorkoutItem.
/// Tập trung tối ưu cho việc đọc dữ liệu và sắp xếp kết quả.
/// </summary>
public class WorkoutItemQueryService : IWorkoutItemQueryService
{
    private readonly IWorkoutItemQueryRepository _queryRepository;
    private readonly IMapper _mapper;

    public WorkoutItemQueryService(
        IWorkoutItemQueryRepository queryRepository,
        IMapper mapper)
    {
        _queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Lấy tất cả mục tập luyện của một người dùng với thứ tự phù hợp.
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <returns>ApiResponse chứa danh sách WorkoutItemDto</returns>
    public async Task<ApiResponse<IEnumerable<WorkoutItemDto>>> GetWorkoutItemsByUserIdAsync(int userId)
    {
        try
        {
            // Kiểm tra đầu vào
            if (userId <= 0)
                return ApiResponse<IEnumerable<WorkoutItemDto>>.ErrorResult("Invalid user ID");

            // Lấy dữ liệu từ repository
            var workoutItems = await _queryRepository.GetByUserIdAsync(userId);

            // Ánh xạ sang DTO
            var dtos = _mapper.Map<IEnumerable<WorkoutItemDto>>(workoutItems);

            return ApiResponse<IEnumerable<WorkoutItemDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<WorkoutItemDto>>.ErrorResult($"Failed to retrieve workout items: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy các mục tập luyện theo người dùng và ngày trong tuần.
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <param name="dayOfWeek">Ngày trong tuần cần lọc</param>
    /// <returns>ApiResponse chứa danh sách WorkoutItemDto</returns>
    public async Task<ApiResponse<IEnumerable<WorkoutItemDto>>> GetWorkoutItemsByUserIdAndDayAsync(int userId, Weekday dayOfWeek)
    {
        try
        {
            // Kiểm tra đầu vào
            if (userId <= 0)
                return ApiResponse<IEnumerable<WorkoutItemDto>>.ErrorResult("Invalid user ID");

            // Lấy dữ liệu từ repository
            var workoutItems = await _queryRepository.GetByUserIdAndDayAsync(userId, dayOfWeek);

            // Ánh xạ sang DTO
            var dtos = _mapper.Map<IEnumerable<WorkoutItemDto>>(workoutItems);

            return ApiResponse<IEnumerable<WorkoutItemDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<WorkoutItemDto>>.ErrorResult($"Failed to retrieve workout items for {dayOfWeek}: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy danh sách mục tập luyện được nhóm theo ngày trong tuần để dễ quản lý.
    /// Cung cấp cái nhìn tổng quan lịch tập hàng tuần của người dùng.
    /// Trả về dữ liệu với khóa số nguyên (1-7) để tương thích frontend.
    /// </summary>
    /// <param name="userId">ID người dùng</param>
    /// <returns>ApiResponse chứa Dictionary<int, IEnumerable<WorkoutItemSummaryDto>></returns>
    public async Task<ApiResponse<Dictionary<int, IEnumerable<WorkoutItemSummaryDto>>>> GetWorkoutItemsGroupedByDayAsync(int userId)
    {
        try
        {
            // Kiểm tra đầu vào
            if (userId <= 0)
                return ApiResponse<Dictionary<int, IEnumerable<WorkoutItemSummaryDto>>>.ErrorResult("Invalid user ID");

            // Lấy tất cả mục tập cho người dùng
            var workoutItems = await _queryRepository.GetByUserIdOrderedAsync(userId);

            // Nhóm theo ngày trong tuần và ánh xạ sang DTO tóm tắt
            var groupedItems = workoutItems
                .GroupBy(wi => wi.DayOfWeek)
                .ToDictionary(
                    group => (int)group.Key, // Chuyển enum sang int để tương thích frontend
                    group => _mapper.Map<IEnumerable<WorkoutItemSummaryDto>>(group.AsEnumerable())
                );

            // Đảm bảo đủ 7 ngày (kể cả rỗng)
            var completeWeek = new Dictionary<int, IEnumerable<WorkoutItemSummaryDto>>();
            foreach (Weekday day in Enum.GetValues<Weekday>())
            {
                var dayInt = (int)day;
                completeWeek[dayInt] = groupedItems.ContainsKey(dayInt)
                    ? groupedItems[dayInt]
                    : Enumerable.Empty<WorkoutItemSummaryDto>();
            }

            return ApiResponse<Dictionary<int, IEnumerable<WorkoutItemSummaryDto>>>.SuccessResult(completeWeek);
        }
        catch (Exception ex)
        {
            return ApiResponse<Dictionary<int, IEnumerable<WorkoutItemSummaryDto>>>.ErrorResult($"Failed to retrieve grouped workout items: {ex.Message}");
        }
    }
}
