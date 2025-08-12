using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.DTOs
{
    // Request DTOs
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3-100 ký tự")]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
        public string Password { get; set; } = default!;
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; } = default!;
    }

    // Response DTOs
    public class AuthResponse
    {
        public string AccessToken { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public UserInfoDto User { get; set; } = default!;
    }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Role { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }

    public class RegisterResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Role { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
