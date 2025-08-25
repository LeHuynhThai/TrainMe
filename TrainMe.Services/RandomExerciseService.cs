using AutoMapper;
using TrainMe.Core.DTOs;
using TrainMe.Core.Interfaces.Repositories;
using TrainMe.Core.Interfaces.Services;

namespace TrainMe.Services;

/// <summary>
/// Service thực hiện các thao tác cho RandomExercise (bài tập ngẫu nhiên).
/// </summary>
public class RandomExerciseService : IRandomExerciseService
{
    private readonly IRandomExerciseRepository _repository;
    private readonly IMapper _mapper;

    public RandomExerciseService(IRandomExerciseRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Lấy tất cả bài tập ngẫu nhiên từ kho dữ liệu.
    /// </summary>
    /// <returns>Danh sách RandomExerciseDto</returns>
    public async Task<ApiResponse<IEnumerable<RandomExerciseDto>>> GetAllAsync()
    {
        try
        {
            // Lấy dữ liệu từ repository
            var exercises = await _repository.GetAllAsync();
            // Ánh xạ entity sang DTO để trả ra API
            var exerciseDtos = _mapper.Map<IEnumerable<RandomExerciseDto>>(exercises);

            return ApiResponse<IEnumerable<RandomExerciseDto>>.SuccessResult(exerciseDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<RandomExerciseDto>>.ErrorResult($"Failed to retrieve exercises: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy ngẫu nhiên một bài tập.
    /// </summary>
    /// <returns>RandomExerciseDto nếu tìm thấy</returns>
    public async Task<ApiResponse<RandomExerciseDto>> GetRandomAsync()
    {
        try
        {
            // Lấy ngẫu nhiên 1 bài tập từ repository
            var exercise = await _repository.GetRandomAsync();

            if (exercise == null)
                return ApiResponse<RandomExerciseDto>.ErrorResult("No exercises found");

            // Ánh xạ sang DTO và trả về
            var exerciseDto = _mapper.Map<RandomExerciseDto>(exercise);
            return ApiResponse<RandomExerciseDto>.SuccessResult(exerciseDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<RandomExerciseDto>.ErrorResult($"Failed to get random exercise: {ex.Message}");
        }
    }

    /// <summary>
    /// Lấy nhiều bài tập ngẫu nhiên.
    /// </summary>
    /// <param name="count">Số lượng bài tập cần lấy (phải > 0)</param>
    /// <returns>Danh sách RandomExerciseDto</returns>
    public async Task<ApiResponse<IEnumerable<RandomExerciseDto>>> GetRandomAsync(int count)
    {
        try
        {
            // Kiểm tra tham số đầu vào
            if (count <= 0)
                return ApiResponse<IEnumerable<RandomExerciseDto>>.ErrorResult("Count must be greater than 0");

            // Lấy danh sách ngẫu nhiên theo số lượng chỉ định
            var exercises = await _repository.GetRandomAsync(count);
            // Ánh xạ sang DTO và trả về
            var exerciseDtos = _mapper.Map<IEnumerable<RandomExerciseDto>>(exercises);

            return ApiResponse<IEnumerable<RandomExerciseDto>>.SuccessResult(exerciseDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<RandomExerciseDto>>.ErrorResult($"Failed to get random exercises: {ex.Message}");
        }
    }
}
