using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Services.WorkoutItem;

namespace TrainMe.API.Controllers;

/// <summary>
/// Controller for managing workout items following RESTful API best practices
/// Handles CRUD operations, queries, and advanced management features
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class WorkoutItemsController : BaseController
{
    private readonly IWorkoutItemService _workoutItemService;
    private readonly IWorkoutItemQueryService _queryService;
    private readonly IWorkoutItemManagementService _managementService;

    public WorkoutItemsController(
        IWorkoutItemService workoutItemService,
        IWorkoutItemQueryService queryService,
        IWorkoutItemManagementService managementService)
    {
        _workoutItemService = workoutItemService ?? throw new ArgumentNullException(nameof(workoutItemService));
        _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        _managementService = managementService ?? throw new ArgumentNullException(nameof(managementService));
    }

    /// <summary>
    /// Creates a new workout item for the authenticated user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<WorkoutItemDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    public async Task<IActionResult> CreateWorkoutItem([FromBody] CreateWorkoutItemRequest request)
    {
        // Validate model state
        var validationResult = ValidateModelState();
        if (validationResult != null) return validationResult;

        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        // Create workout item through service
        var response = await _workoutItemService.CreateWorkoutItemAsync(userId.Value, request);
        
        return response.Success 
            ? CreatedAtAction(nameof(GetWorkoutItem), new { id = response.Data!.Id }, response)
            : CreateResponse(response);
    }

    /// <summary>
    /// Gets a specific workout item by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<WorkoutItemDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    public async Task<IActionResult> GetWorkoutItem(int id)
    {
        var response = await _workoutItemService.GetWorkoutItemByIdAsync(id);
        return CreateResponse(response);
    }

    /// <summary>
    /// Gets all workout items for the authenticated user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WorkoutItemDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    public async Task<IActionResult> GetMyWorkoutItems()
    {
        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        var response = await _queryService.GetWorkoutItemsByUserIdAsync(userId.Value);
        return CreateResponse(response);
    }

    /// <summary>
    /// Gets workout items for the authenticated user by day of week
    /// </summary>
    [HttpGet("day/{dayOfWeek:int}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WorkoutItemDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    public async Task<IActionResult> GetWorkoutItemsByDay(int dayOfWeek)
    {
        // Validate day of week
        if (!Enum.IsDefined(typeof(Weekday), dayOfWeek))
            return BadRequest(ApiResponse.ErrorResult("Invalid day of week"));

        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        var response = await _queryService.GetWorkoutItemsByUserIdAndDayAsync(userId.Value, (Weekday)dayOfWeek);
        return CreateResponse(response);
    }

    /// <summary>
    /// Gets workout items grouped by day of week for the authenticated user
    /// </summary>
    [HttpGet("grouped")]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<Weekday, IEnumerable<WorkoutItemSummaryDto>>>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    public async Task<IActionResult> GetWorkoutItemsGrouped()
    {
        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        var response = await _queryService.GetWorkoutItemsGroupedByDayAsync(userId.Value);
        return CreateResponse(response);
    }

    /// <summary>
    /// Updates an existing workout item
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<WorkoutItemDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    [ProducesResponseType(typeof(ApiResponse), 403)]
    public async Task<IActionResult> UpdateWorkoutItem(int id, [FromBody] UpdateWorkoutItemRequest request)
    {
        // Validate model state
        var validationResult = ValidateModelState();
        if (validationResult != null) return validationResult;

        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        // Update workout item through service
        var response = await _workoutItemService.UpdateWorkoutItemAsync(id, userId.Value, request);
        return CreateResponse(response);
    }

    /// <summary>
    /// Deletes a workout item
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    [ProducesResponseType(typeof(ApiResponse), 403)]
    public async Task<IActionResult> DeleteWorkoutItem(int id)
    {
        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        // Delete workout item through service
        var response = await _workoutItemService.DeleteWorkoutItemAsync(id, userId.Value);
        return CreateResponse(response);
    }

    /// <summary>
    /// Reorders workout items for a specific day
    /// </summary>
    [HttpPut("reorder")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    public async Task<IActionResult> ReorderWorkoutItems([FromBody] ReorderWorkoutItemsRequest request)
    {
        // Validate model state
        var validationResult = ValidateModelState();
        if (validationResult != null) return validationResult;

        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        // Reorder workout items through management service
        var response = await _managementService.ReorderWorkoutItemsAsync(userId.Value, request.DayOfWeek, request.ItemSortOrders);
        return CreateResponse(response);
    }

    /// <summary>
    /// Duplicates a workout item to another day
    /// </summary>
    [HttpPost("{id:int}/duplicate")]
    [ProducesResponseType(typeof(ApiResponse<WorkoutItemDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 401)]
    [ProducesResponseType(typeof(ApiResponse), 403)]
    public async Task<IActionResult> DuplicateWorkoutItem(int id, [FromBody] DuplicateWorkoutItemRequest request)
    {
        // Validate model state
        var validationResult = ValidateModelState();
        if (validationResult != null) return validationResult;

        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        // Duplicate workout item through management service
        var response = await _managementService.DuplicateWorkoutItemAsync(id, userId.Value, request.TargetDay);
        
        return response.Success 
            ? CreatedAtAction(nameof(GetWorkoutItem), new { id = response.Data!.Id }, response)
            : CreateResponse(response);
    }

    /// <summary>
    /// Helper method to get current authenticated user ID from JWT claims
    /// </summary>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
