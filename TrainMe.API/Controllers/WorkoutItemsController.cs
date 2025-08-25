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

        // Gọi service để tạo mới bài tập
        var response = await _workoutItemService.CreateWorkoutItemAsync(userId.Value, request);

        // Trả về kết quả: 201 Created nếu thành công, 400 BadRequest nếu thất bại
        return response.Success
            ? CreatedAtAction(nameof(GetWorkoutItem), new { id = response.Data!.Id }, response)
            : BadRequest(response);
    }

    /// <summary>
    /// API lấy thông tin chi tiết một bài tập theo ID
    /// </summary>
    /// <param name="id">ID của bài tập cần lấy</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWorkoutItem(int id)
    {
        // Gọi service để lấy thông tin bài tập theo ID
        var response = await _workoutItemService.GetWorkoutItemByIdAsync(id);
        
        // Trả về kết quả: 200 OK nếu thành công, 400 BadRequest nếu thất bại
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// API lấy tất cả bài tập của người dùng hiện tại
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyWorkoutItems()
    {
        // Lấy userId từ token xác thực
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("Người dùng chưa đăng nhập"));

        // Gọi service để lấy danh sách bài tập của người dùng
        var response = await _queryService.GetWorkoutItemsByUserIdAsync(userId.Value);
        
        // Trả về kết quả: 200 OK nếu thành công, 400 BadRequest nếu thất bại
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// API lấy danh sách bài tập theo thứ trong tuần
    /// </summary>
    /// <param name="dayOfWeek">Thứ trong tuần (0-6, bắt đầu từ Chủ nhật)</param>
    [HttpGet("day/{dayOfWeek:int}")]
    public async Task<IActionResult> GetWorkoutItemsByDay(int dayOfWeek)
    {
        // Kiểm tra thứ trong tuần có hợp lệ không
        if (!Enum.IsDefined(typeof(Weekday), dayOfWeek))
            return BadRequest(ApiResponse.ErrorResult("Thứ trong tuần không hợp lệ"));

        // Lấy userId từ token xác thực
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("Người dùng chưa đăng nhập"));

        // Gọi service để lấy danh sách bài tập theo ngày
        var response = await _queryService.GetWorkoutItemsByUserIdAndDayAsync(userId.Value, (Weekday)dayOfWeek);
        
        // Trả về kết quả: 200 OK nếu thành công, 400 BadRequest nếu thất bại
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// API lấy danh sách bài tập được nhóm theo thứ trong tuần
    /// </summary>
    [HttpGet("grouped")]
    public async Task<IActionResult> GetWorkoutItemsGrouped()
    {
        // Lấy userId từ token xác thực
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("Người dùng chưa đăng nhập"));

        // Gọi service để lấy danh sách bài tập đã nhóm theo thứ
        var response = await _queryService.GetWorkoutItemsGroupedByDayAsync(userId.Value);
        
        // Trả về kết quả: 200 OK nếu thành công, 400 BadRequest nếu thất bại
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// API cập nhật thông tin bài tập
    /// </summary>
    /// <param name="id">ID của bài tập cần cập nhật</param>
    /// <param name="request">Dữ liệu cập nhật</param>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateWorkoutItem(int id, [FromBody] UpdateWorkoutItemRequest request)
    {
        // Kiểm tra dữ liệu đầu vào
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse.ErrorResult("Dữ liệu không hợp lệ"));
        }

        // Lấy userId từ token xác thực
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("Người dùng chưa đăng nhập"));

        // Gọi service để cập nhật bài tập
        var response = await _workoutItemService.UpdateWorkoutItemAsync(id, userId.Value, request);
        
        // Trả về kết quả: 200 OK nếu thành công, 400 BadRequest nếu thất bại
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// API xóa một bài tập
    /// </summary>
    /// <param name="id">ID của bài tập cần xóa</param>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWorkoutItem(int id)
    {
        // Lấy userId từ token xác thực
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("Người dùng chưa đăng nhập"));

        // Gọi service để xóa bài tập
        var response = await _workoutItemService.DeleteWorkoutItemAsync(id, userId.Value);
        
        // Trả về kết quả: 200 OK nếu thành công, 400 BadRequest nếu thất bại
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// API sắp xếp lại thứ tự các bài tập trong một ngày
    /// </summary>
    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderWorkoutItems([FromBody] ReorderWorkoutItemsRequest request)
    {
        // Kiểm tra dữ liệu đầu vào
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse.ErrorResult("Dữ liệu không hợp lệ"));
        }

        // Lấy userId từ token xác thực
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("Người dùng chưa đăng nhập"));

        // Gọi service để sắp xếp lại thứ tự bài tập
        var response = await _managementService.ReorderWorkoutItemsAsync(userId.Value, request.DayOfWeek, request.ItemSortOrders);
        
        // Trả về kết quả: 200 OK nếu thành công, 400 BadRequest nếu thất bại
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// API sao chép bài tập sang một ngày khác
    /// </summary>
    /// <param name="id">ID của bài tập cần sao chép</param>
    /// <param name="request">Thông tin ngày cần sao chép đến</param>
    [HttpPost("{id:int}/duplicate")]
    public async Task<IActionResult> DuplicateWorkoutItem(int id, [FromBody] DuplicateWorkoutItemRequest request)
    {
        // Kiểm tra dữ liệu đầu vào
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse.ErrorResult("Dữ liệu không hợp lệ"));
        }

        // Lấy userId từ token xác thực
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ApiResponse.ErrorResult("Người dùng chưa đăng nhập"));

        // Gọi service để sao chép bài tập
        var response = await _managementService.DuplicateWorkoutItemAsync(id, userId.Value, request.TargetDayOfWeek);
        
        // Trả về kết quả: 201 Created nếu thành công, 400 BadRequest nếu thất bại
        return response.Success
            ? CreatedAtAction(nameof(GetWorkoutItem), new { id = response.Data!.Id }, response)
            : BadRequest(response);
    }

    /// <summary>
    /// Lấy ID của người dùng hiện tại từ token xác thực
    /// </summary>
    /// <returns>ID của người dùng hoặc null nếu không xác định được</returns>
    private int? GetCurrentUserId()
    {
        // Lấy thông tin người dùng từ claim NameIdentifier
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
