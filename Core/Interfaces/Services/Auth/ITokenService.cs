using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Services.Auth
{
    internal interface ITokenService
    {
        string CreateAccessToken(User user);
        DateTime GetExpiration();
    }
}
