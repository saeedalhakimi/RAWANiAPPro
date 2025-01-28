using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RAWANi.WEBAPi.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid? GetUserProfileIdClaimValue(this HttpContext context)
        {
            return GetGuidClaimValue("UserProfileID", context);
        }

        public static Guid? GetIdentityIdClaimValue(this HttpContext context)
        {
            return GetGuidClaimValue("IdentityID", context);
        }

        private static Guid? GetGuidClaimValue(string key, HttpContext context)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            var claim = identity?.FindFirst(key);

            if (claim == null || !Guid.TryParse(claim.Value, out var guidValue))
            {
                return null; // Return null if the claim is missing or invalid
            }

            return guidValue;
        }
    }
}