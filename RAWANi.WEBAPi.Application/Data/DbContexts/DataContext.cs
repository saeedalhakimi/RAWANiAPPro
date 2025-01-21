using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RAWANi.WEBAPi.Application.Data.Configurations;
using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Domain.Entities.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Data.DbContexts
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply the UserProfile configuration
            modelBuilder.ApplyConfiguration(new UserProfileConfig());
            // Apply the RefreshToken configuration
            modelBuilder.ApplyConfiguration(new RefreshTokenConfig());

            base.OnModelCreating(modelBuilder);
        }
    }
}
