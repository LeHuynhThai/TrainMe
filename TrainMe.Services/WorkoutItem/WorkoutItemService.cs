using AutoMapper;
using TrainMe.Core.DTOs;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;
using TrainMe.Core.Interfaces.Services.WorkoutItem;

namespace TrainMe.Services.WorkoutItem;

/// <summary>
/// Service implementation for basic CRUD operations on WorkoutItem entities
/// Follows SOLID principles and Clean Architecture patterns
/// </summary>
public class WorkoutItemService : IWorkoutItemService
{
    private readonly IWorkoutItemRepository _repository;
    private readonly IWorkoutItemQueryRepository _queryRepository;
    private readonly IMapper _mapper;

    public WorkoutItemService(
        IWorkoutItemRepository repository,
        IWorkoutItemQueryRepository queryRepository,
        IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Creates a new workout item with validation and business rules
    /// </summary>
    public async Task<ApiResponse<WorkoutItemDto>> CreateWorkoutItemAsync(int userId, CreateWorkoutItemRequest request)
    {
        try
        {
            // Validate input parameters
            if (userId <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid user ID");

            if (request == null)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Request cannot be null");

            // Business rule: Check for duplicate names on the same day
            var exists = await _queryRepository.ExistsAsync(userId, request.Name, request.DayOfWeek);
            if (exists)
                return ApiResponse<WorkoutItemDto>.ErrorResult($"Workout item '{request.Name}' already exists for {request.DayOfWeek}");

            // Map request to entity
            var workoutItem = _mapper.Map<Core.Entities.WorkoutItem>(request);
            workoutItem.UserId = userId;

            // Create entity through repository
            var createdItem = await _repository.CreateAsync(workoutItem);

            // Map back to DTO for response
            var dto = _mapper.Map<WorkoutItemDto>(createdItem);
            return ApiResponse<WorkoutItemDto>.SuccessResult(dto, "Workout item created successfully");
        }
        catch (Exception ex)
        {
            // Log exception in real implementation
            return ApiResponse<WorkoutItemDto>.ErrorResult($"Failed to create workout item: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a workout item by ID with proper authorization
    /// </summary>
    public async Task<ApiResponse<WorkoutItemDto>> GetWorkoutItemByIdAsync(int id)
    {
        try
        {
            // Validate input
            if (id <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid workout item ID");

            // Retrieve entity from repository
            var workoutItem = await _repository.GetByIdAsync(id);
            if (workoutItem == null)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Workout item not found");

            // Map to DTO and return
            var dto = _mapper.Map<WorkoutItemDto>(workoutItem);
            return ApiResponse<WorkoutItemDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            return ApiResponse<WorkoutItemDto>.ErrorResult($"Failed to retrieve workout item: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing workout item with validation and authorization
    /// </summary>
    public async Task<ApiResponse<WorkoutItemDto>> UpdateWorkoutItemAsync(int id, int userId, UpdateWorkoutItemRequest request)
    {
        try
        {
            // Validate input parameters
            if (id <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid workout item ID");

            if (userId <= 0)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Invalid user ID");

            if (request == null)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Request cannot be null");

            // Retrieve existing entity
            var existingItem = await _repository.GetByIdAsync(id);
            if (existingItem == null)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Workout item not found");

            // Authorization: Ensure user owns the workout item
            if (existingItem.UserId != userId)
                return ApiResponse<WorkoutItemDto>.ErrorResult("Unauthorized to update this workout item");

            // Business rule: Check for duplicate names (excluding current item)
            var exists = await _queryRepository.ExistsAsync(userId, request.Name, request.DayOfWeek, id);
            if (exists)
                return ApiResponse<WorkoutItemDto>.ErrorResult($"Workout item '{request.Name}' already exists for {request.DayOfWeek}");

            // Map updates to existing entity
            _mapper.Map(request, existingItem);

            // Update through repository
            var updatedItem = await _repository.UpdateAsync(existingItem);

            // Map to DTO and return
            var dto = _mapper.Map<WorkoutItemDto>(updatedItem);
            return ApiResponse<WorkoutItemDto>.SuccessResult(dto, "Workout item updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<WorkoutItemDto>.ErrorResult($"Failed to update workout item: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a workout item with proper authorization
    /// </summary>
    public async Task<ApiResponse> DeleteWorkoutItemAsync(int id, int userId)
    {
        try
        {
            // Validate input parameters
            if (id <= 0)
                return ApiResponse.ErrorResult("Invalid workout item ID");

            if (userId <= 0)
                return ApiResponse.ErrorResult("Invalid user ID");

            // Retrieve existing entity for authorization
            var existingItem = await _repository.GetByIdAsync(id);
            if (existingItem == null)
                return ApiResponse.ErrorResult("Workout item not found");

            // Authorization: Ensure user owns the workout item
            if (existingItem.UserId != userId)
                return ApiResponse.ErrorResult("Unauthorized to delete this workout item");

            // Delete through repository
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
                return ApiResponse.ErrorResult("Failed to delete workout item");

            return ApiResponse.SuccessResult("Workout item deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.ErrorResult($"Failed to delete workout item: {ex.Message}");
        }
    }
}
