using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RAWANi.WEBAPi.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Data.Configurations
{
    internal class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Configure the primary key
            builder.HasKey(rt => rt.Id);

            // Configure properties
            builder.Property(rt => rt.Token).IsRequired();
            builder.Property(rt => rt.ExpiryDate).IsRequired();
            builder.Property(rt => rt.IdentityId).IsRequired();
            builder.Property(rt => rt.IsUsed).IsRequired();
            builder.Property(rt => rt.IsRevoked).IsRequired();
        }
    }
}
