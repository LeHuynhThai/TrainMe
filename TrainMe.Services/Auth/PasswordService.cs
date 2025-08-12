using TrainMe.Core.Interfaces.Services.Auth;

namespace TrainMe.Services.Auth;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
