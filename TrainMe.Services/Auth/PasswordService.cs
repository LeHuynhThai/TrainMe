using TrainMe.Core.Interfaces.Services.Auth;

namespace TrainMe.Services.Auth;

/// <summary>
/// Service xử lý băm và xác thực mật khẩu sử dụng BCrypt.
/// </summary>
public class PasswordService : IPasswordService
{
    /// <summary>
    /// Băm mật khẩu thuần bằng thuật toán BCrypt.
    /// </summary>
    /// <param name="password">Mật khẩu thuần (plaintext)</param>
    /// <returns>Chuỗi hash an toàn</returns>
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Xác thực mật khẩu thuần với chuỗi hash đã lưu.
    /// </summary>
    /// <param name="password">Mật khẩu thuần</param>
    /// <param name="hash">Chuỗi hash lưu trong cơ sở dữ liệu</param>
    /// <returns>True nếu khớp, ngược lại False</returns>
    public bool VerifyPassword(string password, string hash) =>
        !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(hash) &&
        BCrypt.Net.BCrypt.Verify(password, hash);
}
