using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainMe.Core.DTOs;
using TrainMe.Core.Interfaces.Services.Auth;

namespace TrainMe.API.Controllers;

// Controller xử lý các API liên quan đến xác thực và người dùng
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // Service xử lý logic xác thực và quản lý user
    private readonly IAuthService _authService;

    /// <summary>
    /// Hàm khởi tạo controller, inject IAuthService
    /// </summary>
    /// <param name="authService">Service xác thực</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    /// <summary>
    /// API đăng ký tài khoản mới
    /// </summary>
    /// <param name="request">Thông tin đăng ký</param>
    /// <returns>Kết quả đăng ký</returns>
    [AllowAnonymous, HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // Kiểm tra dữ liệu đầu vào
        if (!ModelState.IsValid) return BadRequest(CreateValidationErrorResponse());
        // Gọi service để xử lý đăng ký
        var result = await _authService.RegisterAsync(request);
        // Trả về kết quả tương ứng
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// API đăng nhập
    /// </summary>
    /// <param name="request">Thông tin đăng nhập</param>
    /// <returns>Kết quả đăng nhập</returns>
    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // Kiểm tra dữ liệu đầu vào
        if (!ModelState.IsValid) return BadRequest(CreateValidationErrorResponse());
        // Gọi service xử lý đăng nhập
        var result = await _authService.LoginAsync(request);
        // Trả về thành công hoặc lỗi đăng nhập
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    /// <summary>
    /// API lấy thông tin người dùng hiện tại (đã đăng nhập)
    /// </summary>
    /// <returns>Thông tin user hiện tại</returns>
    [Authorize, HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        // Lấy userId từ token
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(ApiResponse.ErrorResult("Không thể xác định người dùng"));
        // Gọi service lấy thông tin user
        var result = await _authService.GetCurrentUserAsync(userId.Value);
        // Trả về thông tin user hoặc lỗi
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Tạo response trả về khi dữ liệu đầu vào không hợp lệ
    /// </summary>
    /// <returns>ApiResponse chứa danh sách lỗi</returns>
    private ApiResponse CreateValidationErrorResponse() =>
        ApiResponse.ErrorResult("Dữ liệu không hợp lệ",
            ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());

    private int? GetCurrentUserId() =>
        int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId) ? userId : null;
}
