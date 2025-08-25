using TrainMe.Core.DTOs;

namespace TrainMe.Core.Interfaces.Services.WorkoutItem;

/// <summary>
/// Các thao tác CRUD cơ bản cho business logic của WorkoutItem
/// </summary>
public interface IWorkoutItemService
{
    /// <summary>
    /// Creates a new workout item for a user
    /// </summary>
    Task<ApiResponse<WorkoutItemDto>> CreateWorkoutItemAsync(int userId, CreateWorkoutItemRequest request);

    /// <summary>
    /// Gets a workout item by ID
    /// </summary>
    Task<ApiResponse<WorkoutItemDto>> GetWorkoutItemByIdAsync(int id);

    /// <summary>
    /// Updates an existing workout item
    /// </summary>
    Task<ApiResponse<WorkoutItemDto>> UpdateWorkoutItemAsync(int id, int userId, UpdateWorkoutItemRequest request);

    /// <summary>
    /// Deletes a workout item
    /// </summary>
    Task<ApiResponse> DeleteWorkoutItemAsync(int id, int userId);
}
