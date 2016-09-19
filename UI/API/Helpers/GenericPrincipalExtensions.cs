using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace API.Helpers
{
    /// <summary>
    /// Static class for getting principal
    /// </summary>
    public static class GenericPrincipalExtensions
    {
        /// <summary>
        /// Get value of claim
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claimName"></param>
        /// <returns></returns>
        public static string GetValueOfClaim(this IPrincipal user, string claimName)
        {
            if (user.Identity.IsAuthenticated)
            {
                var claimsIdentity = user.Identity as ClaimsIdentity;
                if (claimsIdentity == null) return "";
                foreach (var claim in claimsIdentity.Claims)
                {
                    if (claim.Type == claimName)
                        return claim.Value;
                }
                return "";
            }
            else
                return "";
        }
    }
}