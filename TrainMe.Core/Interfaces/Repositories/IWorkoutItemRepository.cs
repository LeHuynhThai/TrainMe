using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for WorkoutItem entity operations
/// </summary>
public interface IWorkoutItemRepository
{
    /// <summary>
    /// Gets a workout item by ID
    /// </summary>
    Task<WorkoutItem?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new workout item
    /// </summary>
    Task<WorkoutItem> CreateAsync(WorkoutItem workoutItem);

    /// <summary>
    /// Updates an existing workout item
    /// </summary>
    Task<WorkoutItem> UpdateAsync(WorkoutItem workoutItem);

    /// <summary>
    /// Deletes a workout item by ID
    /// </summary>
    Task<bool> DeleteAsync(int id);
    /// <summary>
    /// Gets all workout items for a specific user
    /// </summary>
    Task<IEnumerable<WorkoutItem>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Gets workout items for a specific user and day of week
    /// </summary>
    Task<IEnumerable<WorkoutItem>> GetByUserIdAndDayAsync(int userId, Weekday dayOfWeek);

    /// <summary>
    /// Gets workout items for a specific user ordered by day and sort order
    /// </summary>
    Task<IEnumerable<WorkoutItem>> GetByUserIdOrderedAsync(int userId);

    /// <summary>
    /// Checks if a workout item with the same name exists for the user on the same day
    /// </summary>
    Task<bool> ExistsAsync(int userId, string name, Weekday dayOfWeek, int? excludeId = null);

    /// <summary>
    /// Gets the next available sort order for a user on a specific day
    /// </summary>
    Task<int> GetNextSortOrderAsync(int userId, Weekday dayOfWeek);

    /// <summary>
    /// Updates sort orders for workout items when reordering
    /// </summary>
    Task UpdateSortOrdersAsync(int userId, Weekday dayOfWeek, Dictionary<int, int> itemSortOrders);
}
