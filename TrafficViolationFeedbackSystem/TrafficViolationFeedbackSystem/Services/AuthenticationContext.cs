using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TrafficViolationFeedbackSystem.Services
{
    public static class AuthenticationContext    {
        public static ClaimsPrincipal CurrentUser
            => App.Current.Properties["CurrentUser"] as ClaimsPrincipal;

        public static string UserId
            => CurrentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public static string UserRole
            => CurrentUser?.FindFirst(ClaimTypes.Role)?.Value;

        public static bool IsInRole(string role)
            => CurrentUser?.IsInRole(role) ?? false;
    }
}
