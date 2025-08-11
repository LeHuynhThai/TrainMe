using TrainMe.Core.Interfaces.Services.Auth;

namespace TrainMe.Services.Auth
{
    public class PasswordService : IPasswordService
    {
        // Hash password
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Password is required");
            }
            // hash password using BCrypt
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verify password
        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            {
                return false;
            }
            // verify password using BCrypt
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
