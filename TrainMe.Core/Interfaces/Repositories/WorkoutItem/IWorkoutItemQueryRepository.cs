using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Repositories.WorkoutItem;

/// <summary>
/// Các thao tác truy vấn cho entity WorkoutItem
/// </summary>
public interface IWorkoutItemQueryRepository
{
    /// <summary>
    /// Gets all workout items for a specific user
    /// </summary>
    Task<IEnumerable<Entities.WorkoutItem>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Gets workout items for a specific user and day of week
    /// </summary>
    Task<IEnumerable<Entities.WorkoutItem>> GetByUserIdAndDayAsync(int userId, Weekday dayOfWeek);

    /// <summary>
    /// Gets workout items for a specific user ordered by day and sort order
    /// </summary>
    Task<IEnumerable<Entities.WorkoutItem>> GetByUserIdOrderedAsync(int userId);

    /// <summary>
    /// Checks if a workout item with the same name exists for the user on the same day
    /// </summary>
    Task<bool> ExistsAsync(int userId, string name, Weekday dayOfWeek, int? excludeId = null);
}
