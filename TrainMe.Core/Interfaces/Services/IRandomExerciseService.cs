using TrainMe.Core.DTOs;

namespace TrainMe.Core.Interfaces.Services;

/// <summary>
/// Service interface for RandomExercise operations
/// </summary>
public interface IRandomExerciseService
{
    /// <summary>
    /// Gets all random exercises
    /// </summary>
    Task<ApiResponse<IEnumerable<RandomExerciseDto>>> GetAllAsync();

    /// <summary>
    /// Gets a random exercise
    /// </summary>
    Task<ApiResponse<RandomExerciseDto>> GetRandomAsync();

    /// <summary>
    /// Gets multiple random exercises
    /// </summary>
    Task<ApiResponse<IEnumerable<RandomExerciseDto>>> GetRandomAsync(int count);
}
