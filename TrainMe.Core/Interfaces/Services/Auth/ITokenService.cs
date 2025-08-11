using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Services.Auth
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        DateTime GetExpiration();
    }
}
