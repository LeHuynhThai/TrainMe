using TrainMe.Core.DTOs;

namespace TrainMe.Core.Interfaces.Services.Auth;

/// <summary>
/// Interface cho service xử lý các chức năng xác thực và quản lý người dùng
/// Định nghĩa các phương thức cần thiết cho authentication và authorization
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Đăng ký tài khoản mới cho người dùng
    /// </summary>
    /// <param name="request">Thông tin đăng ký bao gồm username và password</param>
    /// <returns>Thông tin người dùng vừa được tạo nếu thành công</returns>
    Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Đăng nhập vào hệ thống
    /// </summary>
    /// <param name="request">Thông tin đăng nhập bao gồm username và password</param>
    /// <returns>Thông tin xác thực bao gồm access token và thông tin user nếu thành công</returns>
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);

    /// <summary>
    /// Lấy thông tin chi tiết của người dùng hiện tại
    /// </summary>
    /// <param name="userId">ID của người dùng cần lấy thông tin</param>
    /// <returns>Thông tin chi tiết của người dùng</returns>
    Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId);
}


