using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Services.WorkoutItem;

namespace TrainMe.API.Controllers;

/// <summary>
/// Controller quản lý các bài tập (Workout Item) theo chuẩn RESTful API
/// Xử lý các chức năng CRUD, truy vấn và các tính năng quản lý nâng cao
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Tất cả các endpoint đều yêu cầu xác thực
public class WorkoutItemsController : ControllerBase
{
    // Service xử lý các thao tác CRUD cho bài tập
    private readonly IWorkoutItemService _workoutItemService;
    // Service xử lý truy vấn nâng cao cho bài tập
    private readonly IWorkoutItemQueryService _queryService;
    // Service xử lý quản lý nâng cao (ví dụ: sắp xếp, sao chép) cho bài tập
    private readonly IWorkoutItemManagementService _managementService;

    /// <summary>
    /// Hàm khởi tạo controller, inject các service liên quan đến bài tập
    /// </summary>
    /// <param name="workoutItemService">Service CRUD cho bài tập</param>
    /// <param name="queryService">Service truy vấn nâng cao</param>
    /// <param name="managementService">Service quản lý nâng cao</param>
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
    /// API tạo mới một bài tập cho người dùng đã đăng nhập
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateWorkoutItem([FromBody] CreateWorkoutItemRequest request)
    {
        // Kiểm tra dữ liệu đầu vào
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse.ErrorResult("Dữ liệu không hợp lệ"));
        }

        // Lấy userId từ token xác thực
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        // Create workout item through service
        var response = await _workoutItemService.CreateWorkoutItemAsync(userId.Value, request);

        return response.Success
            ? CreatedAtAction(nameof(GetWorkoutItem), new { id = response.Data!.Id }, response)
            : BadRequest(response);
    }

    /// <summary>
    /// Gets a specific workout item by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWorkoutItem(int id)
    {
        var response = await _workoutItemService.GetWorkoutItemByIdAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Gets all workout items for the authenticated user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyWorkoutItems()
    {
        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        var response = await _queryService.GetWorkoutItemsByUserIdAsync(userId.Value);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Gets workout items for the authenticated user by day of week
    /// </summary>
    [HttpGet("day/{dayOfWeek:int}")]
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
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Gets workout items grouped by day of week for the authenticated user
    /// </summary>
    [HttpGet("grouped")]
    public async Task<IActionResult> GetWorkoutItemsGrouped()
    {
        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        var response = await _queryService.GetWorkoutItemsGroupedByDayAsync(userId.Value);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Updates an existing workout item
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateWorkoutItem(int id, [FromBody] UpdateWorkoutItemRequest request)
    {
        // Validate model state
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse.ErrorResult("Dữ liệu không hợp lệ"));
        }

        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        // Update workout item through service
        var response = await _workoutItemService.UpdateWorkoutItemAsync(id, userId.Value, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Deletes a workout item
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWorkoutItem(int id)
    {
        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        // Delete workout item through service
        var response = await _workoutItemService.DeleteWorkoutItemAsync(id, userId.Value);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Reorders workout items for a specific day
    /// </summary>
    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderWorkoutItems([FromBody] ReorderWorkoutItemsRequest request)
    {
        // Validate model state
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse.ErrorResult("Dữ liệu không hợp lệ"));
        }

        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        // Reorder workout items through management service
        var response = await _managementService.ReorderWorkoutItemsAsync(userId.Value, request.DayOfWeek, request.ItemSortOrders);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Duplicates a workout item to another day
    /// </summary>
    [HttpPost("{id:int}/duplicate")]
    public async Task<IActionResult> DuplicateWorkoutItem(int id, [FromBody] DuplicateWorkoutItemRequest request)
    {
        // Validate model state
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse.ErrorResult("Dữ liệu không hợp lệ"));
        }

        // Get authenticated user ID
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("User not authenticated"));

        // Duplicate workout item through management service
        var response = await _managementService.DuplicateWorkoutItemAsync(id, userId.Value, request.TargetDay);

        return response.Success
            ? CreatedAtAction(nameof(GetWorkoutItem), new { id = response.Data!.Id }, response)
            : BadRequest(response);
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
