using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainMe.Core.DTOs;
using TrainMe.Core.Interfaces.Services.Auth;

namespace TrainMe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    [AllowAnonymous, HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(CreateValidationErrorResponse());
        var result = await _authService.RegisterAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(CreateValidationErrorResponse());
        var result = await _authService.LoginAsync(request);
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    [Authorize, HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized(ApiResponse.ErrorResult("Không thể xác định người dùng"));
        var result = await _authService.GetCurrentUserAsync(userId.Value);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private ApiResponse CreateValidationErrorResponse() =>
        ApiResponse.ErrorResult("Dữ liệu không hợp lệ",
            ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());

    private int? GetCurrentUserId() =>
        int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId) ? userId : null;
}
