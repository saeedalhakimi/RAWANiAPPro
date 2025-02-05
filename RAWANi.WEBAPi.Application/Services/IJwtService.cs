using Microsoft.AspNetCore.Identity;
using RAWANi.WEBAPi.Domain.Entities.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(IdentityUser identityUser, UserProfile userProfile, List<string> roles);
        string GenerateRefreshToken();
        DateTime GetRefreshTokenExpiryDate();
    }
}
