using AutoMapper;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;
using TrainMe.Core.Interfaces.Services.WorkoutItem;

namespace TrainMe.Services.WorkoutItem;

/// <summary>
/// Service implementation for advanced management operations on WorkoutItem entities
/// Handles complex business operations like reordering and duplication
/// </summary>
public class WorkoutItemManagementService : IWorkoutItemManagementService
{
    private readonly IWorkoutItemRepository _repository;
    private readonly IWorkoutItemQueryRepository _queryRepository;
    private readonly IWorkoutItemSortRepository _sortRepository;
    private readonly IMapper _mapper;

    public WorkoutItemManagementService(
        IWorkoutItemRepository repository,
        IWorkoutItemQueryRepository queryRepository,
        IWorkoutItemSortRepository sortRepository,
        IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));
        _sortRepository = sortRepository ?? throw new ArgumentNullException(nameof(sortRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Reorders workout items for a specific user and day
    /// Ensures data consistency and validates ownership
    /// </summary>
    public async Task<ApiResponse> ReorderWorkoutItemsAsync(int userId, Weekday dayOfWeek, Dictionary<int, int> itemSortOrders)
    {
        try
        {
            // Validate input parameters
            if (userId <= 0)
                return ApiResponse.ErrorResult("Invalid user ID");

            if (itemSortOrders == null || !itemSortOrders.Any())
                return ApiResponse.ErrorResult("Sort orders cannot be empty");

            // Validate sort order values
            if (itemSortOrders.Values.Any(order => order < 0))
                return ApiResponse.ErrorResult("Sort orders must be non-negative");

            // Verify all workout items belong to the user and specified day
            var workoutItemIds = itemSortOrders.Keys.ToList();
            var existingItems = await _queryRepository.GetByUserIdAndDayAsync(userId, dayOfWeek);
            var existingIds = existingItems.Select(wi => wi.Id).ToHashSet();

            // Check if all provided IDs exist and belong to the user/day
            var invalidIds = workoutItemIds.Where(id => !existingIds.Contains(id)).ToList();
            if (invalidIds.Any())
                return ApiResponse.ErrorResult($"Invalid workout item IDs: {string.Join(", ", invalidIds)}");

            // Perform the reordering operation
            await _sortRepository.UpdateSortOrdersAsync(userId, dayOfWeek, itemSortOrders);

            return ApiResponse.SuccessResult("Workout items reordered successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.ErrorResult($"Failed to reorder workout items: {ex.Message}");
        }
    }

    /// <summary>
    /// Duplicates a workout item to another day with proper validation
    /// Creates a new item with updated sort order and timestamps
    /// </summary>
    public async Task<ApiResponse<WorkoutItemDto>> DuplicateWorkoutItemAsync(int id, int userId, Weekday targetDay)
    {
        try
        {
            // Validate input parameters
            if (id <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid workout item ID");

            if (userId <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid user ID");

            // Retrieve the source workout item
            var sourceItem = await _repository.GetByIdAsync(id);
            if (sourceItem == null)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Source workout item not found");

            // Authorization: Ensure user owns the workout item
            if (sourceItem.UserId != userId)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Unauthorized to duplicate this workout item");

            // Business rule: Check if item with same name already exists on target day
            var exists = await _queryRepository.ExistsAsync(userId, sourceItem.Name, targetDay);
            if (exists)
                return ApiResponse<WorkoutItemDto>.ErrorResult($"Workout item '{sourceItem.Name}' already exists for {targetDay}");

            // Create duplicate with new properties
            var duplicateItem = new Core.Entities.WorkoutItem
            {
                UserId = userId,
                Name = sourceItem.Name,
                DayOfWeek = targetDay,
                SortOrder = await _sortRepository.GetNextSortOrderAsync(userId, targetDay),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            // Create the duplicate through repository
            var createdItem = await _repository.CreateAsync(duplicateItem);

            // Map to DTO and return
            var dto = _mapper.Map<WorkoutItemDto>(createdItem);
            return ApiResponse<WorkoutItemDto>.SuccessResult(dto, $"Workout item duplicated to {targetDay} successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<WorkoutItemDto>.ErrorResult($"Failed to duplicate workout item: {ex.Message}");
        }
    }
}
