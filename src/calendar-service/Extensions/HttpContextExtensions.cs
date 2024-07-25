using Calendar.Authentication;
using Microsoft.AspNetCore.Http;

namespace Calendar.Service.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool IsOnImpersonate(this HttpContext httpContext)
        {
            return httpContext != null && httpContext.User.IsInRole(Roles.IMPERSONATE_USER);
        }

        public static bool IsMigrating(this HttpContext httpContext)
        {
            return httpContext != null && httpContext.User.IsInRole(Roles.MIGRATION_USER);
        }
    }
}