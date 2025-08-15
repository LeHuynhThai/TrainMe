using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Services.WorkoutItem;

/// <summary>
/// Advanced management operations for WorkoutItem business logic
/// </summary>
public interface IWorkoutItemManagementService
{
    /// <summary>
    /// Reorders workout items for a specific user and day
    /// </summary>
    Task<ApiResponse> ReorderWorkoutItemsAsync(int userId, Weekday dayOfWeek, Dictionary<int, int> itemSortOrders);

    /// <summary>
    /// Duplicates a workout item to another day
    /// </summary>
    Task<ApiResponse<WorkoutItemDto>> DuplicateWorkoutItemAsync(int id, int userId, Weekday targetDay);
}
