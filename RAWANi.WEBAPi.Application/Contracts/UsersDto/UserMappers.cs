using Microsoft.AspNetCore.Identity;
using RAWANi.WEBAPi.Application.Contracts.UsersDto.Responses;
using RAWANi.WEBAPi.Domain.Entities.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.UsersDto
{
    public static class UserMappers
    {
        public static UserResponseDto ToUserResponseDto(
            IdentityUser user,
            List<string> roles,
            List<UserProfile> userProfiles)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = roles,
                UserProfiles = userProfiles,
            };
        }
    }
}
