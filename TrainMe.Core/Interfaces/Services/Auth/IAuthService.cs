using TrainMe.Core.DTOs;

namespace TrainMe.Core.Interfaces.Services.Auth;

public interface IAuthService
{
    Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId);
}


