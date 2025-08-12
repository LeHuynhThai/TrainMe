using Microsoft.Extensions.Logging;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories;
using TrainMe.Core.Interfaces.Services.Auth;

namespace TrainMe.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            if (await _userRepository.ExistsByUserNameAsync(request.UserName))
                return ApiResponse<RegisterResponse>.ErrorResult("Tên đăng nhập đã tồn tại");

            var user = CreateUser(request);
            var createdUser = await _userRepository.CreateAsync(user);
            var response = MapToRegisterResponse(createdUser);

            _logger.LogInformation("User {UserName} registered successfully", request.UserName);
            return ApiResponse<RegisterResponse>.SuccessResult(response, "Đăng ký thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user {UserName}", request.UserName);
            return ApiResponse<RegisterResponse>.ErrorResult("Có lỗi xảy ra khi đăng ký");
        }
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetByUserNameAsync(request.UserName);
            if (user == null)
            {
                _logger.LogWarning("Login attempt with non-existent username: {UserName}", request.UserName);
                return ApiResponse<AuthResponse>.ErrorResult("Thông tin đăng nhập không chính xác");
            }

            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                return ApiResponse<AuthResponse>.ErrorResult("Thông tin đăng nhập không chính xác");

            var response = CreateAuthResponse(user);

            _logger.LogInformation("User {UserName} logged in successfully", request.UserName);
            return ApiResponse<AuthResponse>.SuccessResult(response, "Đăng nhập thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {UserName}", request.UserName);
            return ApiResponse<AuthResponse>.ErrorResult("Có lỗi xảy ra khi đăng nhập");
        }
    }

    public async Task<ApiResponse<UserInfoDto>> GetCurrentUserAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserInfoDto>.ErrorResult("Người dùng không tồn tại");

            var userInfo = MapToUserInfoDto(user);
            return ApiResponse<UserInfoDto>.SuccessResult(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user {UserId}", userId);
            return ApiResponse<UserInfoDto>.ErrorResult("Có lỗi xảy ra khi lấy thông tin người dùng");
        }
    }

    // Private helper methods
    private User CreateUser(RegisterRequest request) => new()
    {
        UserName = request.UserName,
        PasswordHash = _passwordService.HashPassword(request.Password),
        Role = "User",
        CreatedAt = DateTime.UtcNow
    };

    private static RegisterResponse MapToRegisterResponse(User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        Role = user.Role,
        CreatedAt = user.CreatedAt
    };

    private AuthResponse CreateAuthResponse(User user) => new()
    {
        AccessToken = _tokenService.CreateAccessToken(user),
        ExpiresAt = _tokenService.GetExpiration(),
        User = MapToUserInfoDto(user)
    };

    private static UserInfoDto MapToUserInfoDto(User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        Role = user.Role,
        CreatedAt = user.CreatedAt
    };
}
