using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using API.Helpers;
using Autofac.Core.Lifetime;
using Service.Interface.Authentication;

namespace API.Filters
{
    /// <summary>
    /// Use for checking user with claim
    /// </summary>
    public class ValidateRefreshTokenEverywhereAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private const string Allowanonymousattribute = "AllowAnonymousAttribute";
        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);
            if (((ReflectedHttpActionDescriptor)actionContext.ActionDescriptor).MethodInfo.CustomAttributes.Any(t => t.AttributeType.Name == Allowanonymousattribute))
            {
                return true;
            }

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (SkipAuthorization(actionContext))
            {
                return;
            }
            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
            if (principal == null || principal.Identity == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            else
            {
                var refreshTokenClaim = principal.Claims.FirstOrDefault(p => p.Type == ClaimName.RefreshTokenKey);
                if (refreshTokenClaim == null)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
                else
                {
                    var owinContext = actionContext.Request.GetOwinContext();
                    var scope = owinContext.Get<LifetimeScope>("autofac:OwinLifetimeScope");
                    var refreshTokenService = (IRefreshTokenService)scope.GetService(typeof(IRefreshTokenService));

                    var currentRefreshToken = refreshTokenClaim.Value;

                    var refreshToken = refreshTokenService.GetRefreshTokenById(currentRefreshToken);
                    if (refreshToken == null)
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    else
                    {
                        if (DateTime.UtcNow > refreshToken.ExpiredDateTimeUtc)
                            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        if (refreshToken.HasChangeClaim)
                            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    }
                }

            }
        }
    }
}