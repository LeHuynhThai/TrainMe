using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Services.Auth;
using TrainMe.Data;

namespace TrainMe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext db, IPasswordService passwordService, ITokenService tokenService)
    {
        _db = db;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "UserName và Password là bắt buộc." });

        var exists = await _db.Users.AnyAsync(x => x.UserName == request.UserName);
        if (exists) return Conflict(new { message = "UserName đã tồn tại." });

        var user = new User
        {
            UserName = request.UserName,
            PasswordHash = _passwordService.HashPassword(request.Password),
            Role = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Created(string.Empty, new { user.Id, user.UserName, user.Role });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _db.Users.SingleOrDefaultAsync(x => x.UserName == request.UserName);
        if (user == null) return Unauthorized(new { message = "Sai thông tin đăng nhập." });

        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Sai thông tin đăng nhập." });

        var accessToken = _tokenService.CreateAccessToken(user);
        var expiresAt = _tokenService.GetExpiration();

        return Ok(new { 
            accessToken, 
            expiresAt, 
            userName = user.UserName, 
            role = user.Role 
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new { userId, userName, role });
    }
}

// DTOs
public class RegisterRequest
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string? Role { get; set; }
}

public class LoginRequest
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
}
