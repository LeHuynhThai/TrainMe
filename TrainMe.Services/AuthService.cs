using Microsoft.Extensions.Logging;
using TrainMe.Core.DTOs;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories;
using TrainMe.Core.Interfaces.Services.Auth;

namespace TrainMe.Services
{
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
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Validate if username already exists
                if (await _userRepository.ExistsByUserNameAsync(request.UserName))
                {
                    return ApiResponse<RegisterResponse>.ErrorResult("Tên đăng nhập đã tồn tại");
                }

                // Create user
                var user = new User
                {
                    UserName = request.UserName,
                    PasswordHash = _passwordService.HashPassword(request.Password),
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.CreateAsync(user);

                var response = new RegisterResponse
                {
                    Id = createdUser.Id,
                    UserName = createdUser.UserName,
                    Role = createdUser.Role,
                    CreatedAt = createdUser.CreatedAt
                };

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
                // Get user
                var user = await _userRepository.GetByUserNameAsync(request.UserName);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt with non-existent username: {UserName}", request.UserName);
                    return ApiResponse<AuthResponse>.ErrorResult("Thông tin đăng nhập không chính xác");
                }

                // Verify password
                if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return ApiResponse<AuthResponse>.ErrorResult("Thông tin đăng nhập không chính xác");
                }

                // Create token
                var accessToken = _tokenService.CreateAccessToken(user);

                var response = new AuthResponse
                {
                    AccessToken = accessToken,
                    ExpiresAt = _tokenService.GetExpiration(),
                    User = new UserInfoDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Role = user.Role,
                        CreatedAt = user.CreatedAt
                    }
                };

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
                {
                    return ApiResponse<UserInfoDto>.ErrorResult("Người dùng không tồn tại");
                }

                var userInfo = new UserInfoDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                };

                return ApiResponse<UserInfoDto>.SuccessResult(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user {UserId}", userId);
                return ApiResponse<UserInfoDto>.ErrorResult("Có lỗi xảy ra khi lấy thông tin người dùng");
            }
        }
    }
}
