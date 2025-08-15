using AutoMapper;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;
using TrainMe.Core.Interfaces.Services.WorkoutItem;

namespace TrainMe.Services.WorkoutItem;

/// <summary>
/// Service implementation for query operations on WorkoutItem entities
/// Focuses on read-only operations with optimized data retrieval
/// </summary>
public class WorkoutItemQueryService : IWorkoutItemQueryService
{
    private readonly IWorkoutItemQueryRepository _queryRepository;
    private readonly IMapper _mapper;

    public WorkoutItemQueryService(
        IWorkoutItemQueryRepository queryRepository,
        IMapper mapper)
    {
        _queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Retrieves all workout items for a specific user with proper ordering
    /// </summary>
    public async Task<ApiResponse<IEnumerable<WorkoutItemDto>>> GetWorkoutItemsByUserIdAsync(int userId)
    {
        try
        {
            // Validate input
            if (userId <= 0)
                return ApiResponse<IEnumerable<WorkoutItemDto>>.ErrorResult("Invalid user ID");

            // Retrieve entities from repository
            var workoutItems = await _queryRepository.GetByUserIdAsync(userId);

            // Map to DTOs
            var dtos = _mapper.Map<IEnumerable<WorkoutItemDto>>(workoutItems);

            return ApiResponse<IEnumerable<WorkoutItemDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<WorkoutItemDto>>.ErrorResult($"Failed to retrieve workout items: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves workout items for a specific user and day of week
    /// </summary>
    public async Task<ApiResponse<IEnumerable<WorkoutItemDto>>> GetWorkoutItemsByUserIdAndDayAsync(int userId, Weekday dayOfWeek)
    {
        try
        {
            // Validate input
            if (userId <= 0)
                return ApiResponse<IEnumerable<WorkoutItemDto>>.ErrorResult("Invalid user ID");

            // Retrieve entities from repository
            var workoutItems = await _queryRepository.GetByUserIdAndDayAsync(userId, dayOfWeek);

            // Map to DTOs
            var dtos = _mapper.Map<IEnumerable<WorkoutItemDto>>(workoutItems);

            return ApiResponse<IEnumerable<WorkoutItemDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<WorkoutItemDto>>.ErrorResult($"Failed to retrieve workout items for {dayOfWeek}: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves workout items grouped by day of week for better organization
    /// Provides a comprehensive view of user's weekly workout schedule
    /// Returns data with integer keys (1-7) for frontend compatibility
    /// </summary>
    public async Task<ApiResponse<Dictionary<int, IEnumerable<WorkoutItemSummaryDto>>>> GetWorkoutItemsGroupedByDayAsync(int userId)
    {
        try
        {
            // Validate input
            if (userId <= 0)
                return ApiResponse<Dictionary<int, IEnumerable<WorkoutItemSummaryDto>>>.ErrorResult("Invalid user ID");

            // Retrieve all workout items for the user
            var workoutItems = await _queryRepository.GetByUserIdOrderedAsync(userId);

            // Group by day of week and map to summary DTOs
            var groupedItems = workoutItems
                .GroupBy(wi => wi.DayOfWeek)
                .ToDictionary(
                    group => (int)group.Key, // Convert enum to int for frontend compatibility
                    group => _mapper.Map<IEnumerable<WorkoutItemSummaryDto>>(group.AsEnumerable())
                );

            // Ensure all days of week are represented (even if empty)
            var completeWeek = new Dictionary<int, IEnumerable<WorkoutItemSummaryDto>>();
            foreach (Weekday day in Enum.GetValues<Weekday>())
            {
                var dayInt = (int)day;
                completeWeek[dayInt] = groupedItems.ContainsKey(dayInt)
                    ? groupedItems[dayInt]
                    : Enumerable.Empty<WorkoutItemSummaryDto>();
            }

            return ApiResponse<Dictionary<int, IEnumerable<WorkoutItemSummaryDto>>>.SuccessResult(completeWeek);
        }
        catch (Exception ex)
        {
            return ApiResponse<Dictionary<int, IEnumerable<WorkoutItemSummaryDto>>>.ErrorResult($"Failed to retrieve grouped workout items: {ex.Message}");
        }
    }
}
