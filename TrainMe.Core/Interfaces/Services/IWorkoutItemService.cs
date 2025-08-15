using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Services;

/// <summary>
/// Service interface for WorkoutItem business operations
/// </summary>
public interface IWorkoutItemService
{
    /// <summary>
    /// Creates a new workout item for a user
    /// </summary>
    Task<ApiResponse<WorkoutItemDto>> CreateWorkoutItemAsync(int userId, CreateWorkoutItemRequest request);

    /// <summary>
    /// Gets a workout item by ID
    /// </summary>
    Task<ApiResponse<WorkoutItemDto>> GetWorkoutItemByIdAsync(int id);

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

    /// <summary>
    /// Updates an existing workout item
    /// </summary>
    Task<ApiResponse<WorkoutItemDto>> UpdateWorkoutItemAsync(int id, int userId, UpdateWorkoutItemRequest request);

    /// <summary>
    /// Deletes a workout item
    /// </summary>
    Task<ApiResponse> DeleteWorkoutItemAsync(int id, int userId);

    /// <summary>
    /// Reorders workout items for a specific user and day
    /// </summary>
    Task<ApiResponse> ReorderWorkoutItemsAsync(int userId, Weekday dayOfWeek, Dictionary<int, int> itemSortOrders);

    /// <summary>
    /// Duplicates a workout item to another day
    /// </summary>
    Task<ApiResponse<WorkoutItemDto>> DuplicateWorkoutItemAsync(int id, int userId, Weekday targetDay);
}
