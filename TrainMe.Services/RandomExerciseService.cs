using AutoMapper;
using TrainMe.Core.DTOs;
using TrainMe.Core.Interfaces.Repositories;
using TrainMe.Core.Interfaces.Services;

namespace TrainMe.Services;

/// <summary>
/// Service implementation for RandomExercise operations
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
    /// Gets all random exercises
    /// </summary>
    public async Task<ApiResponse<IEnumerable<RandomExerciseDto>>> GetAllAsync()
    {
        try
        {
            var exercises = await _repository.GetAllAsync();
            var exerciseDtos = _mapper.Map<IEnumerable<RandomExerciseDto>>(exercises);

            return ApiResponse<IEnumerable<RandomExerciseDto>>.SuccessResult(exerciseDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<RandomExerciseDto>>.ErrorResult($"Failed to retrieve exercises: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a random exercise
    /// </summary>
    public async Task<ApiResponse<RandomExerciseDto>> GetRandomAsync()
    {
        try
        {
            var exercise = await _repository.GetRandomAsync();

            if (exercise == null)
                return ApiResponse<RandomExerciseDto>.ErrorResult("No exercises found");

            var exerciseDto = _mapper.Map<RandomExerciseDto>(exercise);
            return ApiResponse<RandomExerciseDto>.SuccessResult(exerciseDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<RandomExerciseDto>.ErrorResult($"Failed to get random exercise: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets multiple random exercises
    /// </summary>
    public async Task<ApiResponse<IEnumerable<RandomExerciseDto>>> GetRandomAsync(int count)
    {
        try
        {
            if (count <= 0)
                return ApiResponse<IEnumerable<RandomExerciseDto>>.ErrorResult("Count must be greater than 0");

            var exercises = await _repository.GetRandomAsync(count);
            var exerciseDtos = _mapper.Map<IEnumerable<RandomExerciseDto>>(exercises);

            return ApiResponse<IEnumerable<RandomExerciseDto>>.SuccessResult(exerciseDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<RandomExerciseDto>>.ErrorResult($"Failed to get random exercises: {ex.Message}");
        }
    }
}
