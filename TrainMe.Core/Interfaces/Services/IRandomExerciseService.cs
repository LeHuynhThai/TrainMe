using TrainMe.Core.DTOs;

namespace TrainMe.Core.Interfaces.Services;

/// <summary>
/// Interface service cho các thao tác với bài tập ngẫu nhiên
/// </summary>
public interface IRandomExerciseService
{
    /// <summary>
    /// Lấy tất cả các bài tập ngẫu nhiên
    /// </summary>
    Task<ApiResponse<IEnumerable<RandomExerciseDto>>> GetAllAsync();

    /// <summary>
    /// Lấy một bài tập ngẫu nhiên
    /// </summary>
    Task<ApiResponse<RandomExerciseDto>> GetRandomAsync();

    /// <summary>
    /// Lấy nhiều bài tập ngẫu nhiên
    /// </summary>
    Task<ApiResponse<IEnumerable<RandomExerciseDto>>> GetRandomAsync(int count);
}
