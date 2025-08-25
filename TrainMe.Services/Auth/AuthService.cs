using Microsoft.Extensions.Logging;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories.User;
using TrainMe.Core.Interfaces.Services.Auth;

namespace TrainMe.Services.Auth;

/// <summary>
/// Service xử lý nghiệp vụ xác thực: đăng ký, đăng nhập,
/// và lấy thông tin người dùng hiện tại dựa trên userId.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    /// <summary>
    /// Khởi tạo AuthService với các phụ thuộc cần thiết.
    /// </summary>
    /// <param name="userRepository">Repository người dùng</param>
    /// <param name="passwordService">Service băm/xác thực mật khẩu</param>
    /// <param name="tokenService">Service tạo token JWT</param>
    /// <param name="logger">Bộ ghi log</param>
    public AuthService(IUserRepository userRepository, IPasswordService passwordService,
        ITokenService tokenService, ILogger<AuthService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordService = passwordService ?? throw new ArgumentNullException(nameof(passwordService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Đăng ký tài khoản mới.
    /// </summary>
    /// <param name="request">Thông tin đăng ký</param>
    /// <returns>ApiResponse chứa <see cref="UserDto"/> nếu thành công</returns>
    public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            if (await _userRepository.ExistsByUserNameAsync(request.UserName))
                return ApiResponse<UserDto>.ErrorResult("Tên đăng nhập đã tồn tại");

            var createdUser = await _userRepository.CreateAsync(CreateUser(request));
            _logger.LogInformation("User {UserName} registered successfully", request.UserName);
            return ApiResponse<UserDto>.SuccessResult(MapToUserDto(createdUser), "Đăng ký thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user {UserName}", request.UserName);
            return ApiResponse<UserDto>.ErrorResult("Có lỗi xảy ra khi đăng ký");
        }
    }

    /// <summary>
    /// Đăng nhập hệ thống.
    /// </summary>
    /// <param name="request">Thông tin đăng nhập</param>
    /// <returns>ApiResponse chứa <see cref="AuthResponse"/> (token + thông tin người dùng) nếu thành công</returns>
    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetByUserNameAsync(request.UserName);
            if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                if (user == null) _logger.LogWarning("Login attempt with non-existent username: {UserName}", request.UserName);
                return ApiResponse<AuthResponse>.ErrorResult("Thông tin đăng nhập không chính xác");
            }

            _logger.LogInformation("User {UserName} logged in successfully", request.UserName);
            return ApiResponse<AuthResponse>.SuccessResult(CreateAuthResponse(user), "Đăng nhập thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {UserName}", request.UserName);
            return ApiResponse<AuthResponse>.ErrorResult("Có lỗi xảy ra khi đăng nhập");
        }
    }

    /// <summary>
    /// Lấy thông tin người dùng hiện tại theo Id.
    /// </summary>
    /// <param name="userId">Id người dùng</param>
    /// <returns>ApiResponse chứa <see cref="UserDto"/>, hoặc lỗi nếu không tồn tại</returns>
    public async Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null
                ? ApiResponse<UserDto>.ErrorResult("Người dùng không tồn tại")
                : ApiResponse<UserDto>.SuccessResult(MapToUserDto(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user {UserId}", userId);
            return ApiResponse<UserDto>.ErrorResult("Có lỗi xảy ra khi lấy thông tin người dùng");
        }
    }

    /// <summary>
    /// Tạo entity User từ yêu cầu đăng ký.
    /// </summary>
    /// <param name="request">Thông tin đăng ký</param>
    /// <returns>Đối tượng User đã khởi tạo</returns>
    private User CreateUser(RegisterRequest request) => new()
    {
        UserName = request.UserName,
        PasswordHash = _passwordService.HashPassword(request.Password),
        Role = "User",
        CreatedAt = DateTime.UtcNow
    };

    /// <summary>
    /// Tạo phản hồi xác thực chứa token và thông tin người dùng.
    /// </summary>
    /// <param name="user">Người dùng</param>
    /// <returns><see cref="AuthResponse"/></returns>
    private AuthResponse CreateAuthResponse(User user) => new(
        _tokenService.CreateAccessToken(user),
        _tokenService.GetExpiration(),
        MapToUserDto(user));

    /// <summary>
    /// Ánh xạ entity User sang DTO.
    /// </summary>
    /// <param name="user">Entity User</param>
    /// <returns><see cref="UserDto"/></returns>
    private static UserDto MapToUserDto(User user) => new(user.Id, user.UserName, user.Role, user.CreatedAt);
}
