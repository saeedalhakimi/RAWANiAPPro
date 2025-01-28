using RAWANi.WEBAPi.Domain.Entities.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.UsersDto.Responses
{
    public record UserResponseDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
    }
}
