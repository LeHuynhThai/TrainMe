using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Repositories.WorkoutItem;

/// <summary>
/// Sort and ordering operations for WorkoutItem entity
/// </summary>
public interface IWorkoutItemSortRepository
{
    /// <summary>
    /// Gets the next available sort order for a user on a specific day
    /// </summary>
    Task<int> GetNextSortOrderAsync(int userId, Weekday dayOfWeek);

    /// <summary>
    /// Updates sort orders for workout items when reordering
    /// </summary>
    Task UpdateSortOrdersAsync(int userId, Weekday dayOfWeek, Dictionary<int, int> itemSortOrders);
}
