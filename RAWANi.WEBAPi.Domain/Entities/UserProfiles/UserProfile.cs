using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.Entities.UserProfiles
{
    public class UserProfile
    {
        public Guid UserProfileID { get; private set; }
        public string IdentityID { get; private set; }
        public BasicInformation BasicInfo { get; private set; }
        public string? ImageLink { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }
        private UserProfile() { }
        public static OperationResult<UserProfile> Create(
            string identityID, BasicInformation basicInfo, string imageLink)
        {
            var userPrfile = new UserProfile
            {
                UserProfileID = Guid.NewGuid(),
                IdentityID = identityID,
                BasicInfo = basicInfo,
                ImageLink = imageLink,
                CreatedAt = DateTime.UtcNow
            };

            return OperationResult<UserProfile>.Success(userPrfile);
        }

        public void UpdateBasicInfo(BasicInformation basicInfo)
        {
            BasicInfo = basicInfo;
            LastUpdatedAt = DateTime.UtcNow;
        }
    }
}
