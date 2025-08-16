using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Repositories;

/// <summary>
/// Repository interface for RandomExercise entity
/// </summary>
public interface IRandomExerciseRepository
{
    /// <summary>
    /// Gets all random exercises from database
    /// </summary>
    Task<IEnumerable<RandomExercise>> GetAllAsync();

    /// <summary>
    /// Gets a random exercise from database
    /// </summary>
    Task<RandomExercise?> GetRandomAsync();

    /// <summary>
    /// Gets multiple random exercises from database
    /// </summary>
    Task<IEnumerable<RandomExercise>> GetRandomAsync(int count);
}
