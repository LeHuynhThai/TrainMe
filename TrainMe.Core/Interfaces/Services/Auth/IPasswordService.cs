namespace TrainMe.Core.Interfaces.Services.Auth
{
    /// <summary>
    /// Interface cho service xử lý mã hóa và xác thực mật khẩu
    /// Cung cấp các phương thức để hash và verify mật khẩu một cách an toàn
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Mã hóa mật khẩu thành hash string để lưu trữ an toàn
        /// Sử dụng thuật toán hash mạnh (như bcrypt) để bảo vệ mật khẩu
        /// </summary>
        /// <param name="password">Mật khẩu gốc cần mã hóa</param>
        /// <returns>Chuỗi hash của mật khẩu</returns>
        string HashPassword(string password);

        /// <summary>
        /// Xác thực mật khẩu bằng cách so sánh với hash đã lưu
        /// </summary>
        /// <param name="password">Mật khẩu gốc cần xác thực</param>
        /// <param name="hash">Hash đã lưu trong database</param>
        /// <returns>True nếu mật khẩu đúng, False nếu sai</returns>
        bool VerifyPassword(string password, string hash);
    }
}
