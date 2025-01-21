using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RAWANi.WEBAPi.Domain.Entities.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Data.Configurations
{
    internal class UserProfileConfig : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            // Configure UserProfileID as the primary key
            builder.HasKey(up => up.UserProfileID);

            // Configure UserProfileID as a nonclustered index
            builder.HasIndex(up => up.UserProfileID)
                   .IsClustered(false); // Explicitly set as nonclustered
            builder.Property(u => u.IdentityID).IsRequired();
            builder.Property(u => u.ImageLink).IsRequired(false);

            builder.OwnsOne(up => up.BasicInfo);
        }
    }
}
