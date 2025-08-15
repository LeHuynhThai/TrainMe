using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Services.WorkoutItem;

/// <summary>
/// Query operations for WorkoutItem business logic
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
    /// </summary>
    Task<ApiResponse<Dictionary<Weekday, IEnumerable<WorkoutItemSummaryDto>>>> GetWorkoutItemsGroupedByDayAsync(int userId);
}
