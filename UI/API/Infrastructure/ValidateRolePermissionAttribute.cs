using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using Autofac.Core.Lifetime;

namespace API.Infrastructure
{
    /// <summary>
    /// Use for checking user has role in claim or not
    /// </summary>
    public class ValidateRolePermissionAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        /// <summary>
        /// RoleClaim
        /// </summary>
        public string RoleClaim { get; set; }

        private void DisplayError(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(
                HttpStatusCode.Unauthorized, "You dont have permission for execute this action",
                actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }

        /// <summary>
        /// OnActionExecuting
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;
            if (principal == null || principal.Identity == null)
            {
                DisplayError(actionContext);
                return;
            }

            var scope = actionContext.Request.GetOwinContext().Get<LifetimeScope>("autofac:OwinLifetimeScope");
            var workContext = (IWorkContext)scope.GetService(typeof (IWorkContext));

            var currentUser = workContext.CurrentUser;
            var currentRoles = workContext.CurrentRoles;

            if (currentUser == null || currentRoles == null || currentRoles.Count == 0)
            {
                DisplayError(actionContext);
                return;
            }

            // if RoleClaim is empty, it is ok
            if (string.IsNullOrEmpty(RoleClaim))
                return;

            var listRole = RoleClaim.Split(',');
            if (!currentRoles.Any(r => listRole.Contains(r.Name, StringComparer.OrdinalIgnoreCase)))
            {
                DisplayError(actionContext);
            }
        }
    }
}