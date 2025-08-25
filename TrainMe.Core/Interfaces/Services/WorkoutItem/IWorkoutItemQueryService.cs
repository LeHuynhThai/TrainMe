using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Services.WorkoutItem;

/// <summary>
/// Các thao tác truy vấn cho business logic của WorkoutItem
/// </summary>
public interface IWorkoutItemQueryService
{
    /// <summary>
    /// Gets all workout items for a specific user
    /// </summary>
    Task<ApiResponse<IEnumerable<WorkoutItemDto>>> GetWorkoutItemsByUserIdAsync(int userId);

    /// <summary>
    /// Gets workout items for a specific user and day of week
    /// </summary>
    Task<ApiResponse<IEnumerable<WorkoutItemDto>>> GetWorkoutItemsByUserIdAndDayAsync(int userId, Weekday dayOfWeek);

    /// <summary>
    /// Gets workout items for a specific user grouped by day of week
    /// Returns data with integer keys (1-7) for frontend compatibility
    /// </summary>
    Task<ApiResponse<Dictionary<int, IEnumerable<WorkoutItemSummaryDto>>>> GetWorkoutItemsGroupedByDayAsync(int userId);
}
