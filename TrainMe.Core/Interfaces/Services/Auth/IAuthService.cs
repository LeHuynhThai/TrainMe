using TrainMe.Core.DTOs;

namespace TrainMe.Core.Interfaces.Services.Auth
{
    /// <summary>
    /// Interface cho Authentication Service - xử lý đăng ký, đăng nhập và quản lý user
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        /// <param name="request">Thông tin đăng ký (UserName, Password)</param>
        /// <returns>Thông tin user đã tạo hoặc lỗi</returns>
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Đăng nhập vào hệ thống
        /// </summary>
        /// <param name="request">Thông tin đăng nhập (UserName, Password)</param>
        /// <returns>JWT token và thông tin user hoặc lỗi</returns>
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);

        /// <summary>
        /// Lấy thông tin user hiện tại theo ID
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <returns>Thông tin user hoặc lỗi</returns>
        Task<ApiResponse<UserInfoDto>> GetCurrentUserAsync(int userId);
    }
}
