using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Repositories.WorkoutItem;

/// <summary>
/// Basic CRUD operations for WorkoutItem entity
/// </summary>
public interface IWorkoutItemRepository
{
    /// <summary>
    /// Gets a workout item by ID
    /// </summary>
    Task<Entities.WorkoutItem?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new workout item
    /// </summary>
    Task<Entities.WorkoutItem> CreateAsync(Entities.WorkoutItem workoutItem);

    /// <summary>
    /// Updates an existing workout item
    /// </summary>
    Task<Entities.WorkoutItem> UpdateAsync(Entities.WorkoutItem workoutItem);

    /// <summary>
    /// Deletes a workout item by ID
    /// </summary>
    Task<bool> DeleteAsync(int id);
}
