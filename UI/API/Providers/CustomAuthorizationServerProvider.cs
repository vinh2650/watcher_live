using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Helpers;
using Common.Helpers;
using Core.Domain.Authentication;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Service.Interface.Authentication;

namespace API.Providers
{
    /// <summary>
    /// Custom Authorization Server Provider
    /// </summary>
    public class CustomAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        
        private readonly Func<IAmsApplicationService> _amsApplicationServiceFactory;
        private readonly Func<IUserService> _userServiceFactory;
        private readonly Func<IRefreshTokenService> _refreshTokenServiceFactory;

        private IAmsApplicationService _amsApplicationService
        {
            get
            {
                return this._amsApplicationServiceFactory.Invoke();
            }
        }
   
        private IUserService _userService
        {
            get
            {
                return this._userServiceFactory.Invoke();
            }
        }
        private IRefreshTokenService _refreshTokenService
        {
            get
            {
                return this._refreshTokenServiceFactory.Invoke();
            }
        }


       
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="amsApplicationService"></param>
        /// <param name="userService"></param>
        /// <param name="refreshTokenService"></param>
        public CustomAuthorizationServerProvider(Func<IAmsApplicationService> amsApplicationService, 
            Func<IUserService> userService,
            Func<IRefreshTokenService> refreshTokenService)
        {
            _amsApplicationServiceFactory = amsApplicationService;
            _userServiceFactory = userService;
            _refreshTokenServiceFactory = refreshTokenService;
        }
        /// <summary>
        /// Validate CLient
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            
            string clientId;
            string clientSecret;
         
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                //context.Validated();
                context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }
            
            var application = _amsApplicationService.GetById(context.ClientId);

            if (application == null)
            {
                context.SetError("invalid_clientId", string.Format("Application '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }

            if (application.Type == ApplicationType.Native)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else
                {

                    if (application.AppSecret != CommonSecurityHelper.GetHash(clientSecret))
                    {
                        context.SetError("invalid_clientId", "Client secret is invalid.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            if (!application.Active)
            {
                context.SetError("invalid_clientId", "application is inactive.");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set<string>("as:clientAllowedOrigin", application.AllowOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", application.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }
        
        /// <summary>
        /// Grant Resource 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin") ?? "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            //generate refresh token and and as claim here
            if (string.IsNullOrEmpty(context.ClientId))
            {
                return;
            }
            //validate user
            
            var user = _userService.GetUserByUsername(context.UserName);
            if (user == null)
            {
                context.SetError("invalid_grant", "The credential is incorrect.");
                return;
            }

            if (!user.IsEmailConfirmed)
            {
                context.SetError("invalid_grant", "Email is not comfirm");
                return;
            }

            if (user.Banned)
            {
                context.SetError("invalid_grant", "User Invalid");
                return;
            }

            var verifyCode = CommonSecurityHelper.CreatePasswordHash(context.Password, user.SaltDigitCodeHash);

            if (verifyCode != user.DigitCodeHash)
            {
                context.SetError("invalid_grant", "The credential is incorrect.");
                return;
            }

            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");
           
            var token = new RefreshToken()
            {

                AppId = context.ClientId,
                Username = context.UserName,
                IssuesDateTimeUtc = DateTime.UtcNow,
                ExpiredDateTimeUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime)),
                ProtectedTicket =  Guid.NewGuid().ToString()
            };

            
            //add refresh token here
            await _refreshTokenService.CreateRefreshTokenAsync(token);
            
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimName.UsernameKey, context.UserName));
            identity.AddClaim(new Claim(ClaimName.UseridKey, user.Id));
            identity.AddClaim(new Claim(ClaimName.RefreshTokenKey, token.Id));
            identity.AddClaim(new Claim(ClaimName.AppId, context.ClientId));

            //todo work later for role based on claim
            //var currentRoleName = await _userService.GetRoleByClaimAsync(user.Id);
            //if (currentRoleName != null)
            //{
            //    var roleClaim = new Claim(ClaimTypes.Role, currentRoleName);
            //    identity.AddClaim(roleClaim);

            //}


            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { 
                        "as:client_id", context.ClientId ?? string.Empty
                    },
                    { 
                        "userName", context.UserName
                    },
                     { 
                        "userId", user.Id
                    },
                     { 
                        "as:r_t_id", token.Id
                    },
                    {
                        "firstName", (!string.IsNullOrEmpty(user.FirstName) ? user.FirstName : "")
                    },
                    {
                        "lastName", (!string.IsNullOrEmpty(user.LastName) ? user.LastName : "")
                    },
                    {
                        "email", (!string.IsNullOrEmpty(user.Email) ? user.Email : "")
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
            
        }
        /// <summary>
        /// Token endpoint
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Grant refresh token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                //return null;
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var currentUserId = context.Ticket.Properties.Dictionary["userId"];

           var user = await _userService.GetUserByIdAsync(currentUserId);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user is no longer exist.");
                return;
            }
            if (user.Banned)
            {
                context.SetError("invalid_grant", "User has been banned");
                return;
            }
            if (!user.IsEmailConfirmed)
            {
                context.SetError("invalid_grant", "Email is not comfirm");
                return;
            }

            //var existLibraryClaim = newIdentity.Claims.FirstOrDefault(p => p.Type == ClaimName.OperatorId);
            //if(existLibraryClaim != null)
            //    newIdentity.RemoveClaim(existLibraryClaim);

            //var currentRoleName = await _userService.GetRoleByClaimAsync(user.Id);
            //if (currentRoleName!=null)
            //{
            //    var roleClaim = new Claim(ClaimTypes.Role, currentRoleName);
            //    newIdentity.AddClaim(roleClaim);
            //    if (context.Ticket.Properties.Dictionary.ContainsKey("role"))
            //    {
            //        context.Ticket.Properties.Dictionary["role"] = currentRoleName;
            //    }
            //    else
            //    {
            //        context.Ticket.Properties.Dictionary.Add("role", currentRoleName);
            //    }

            //}


            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
           
            context.Validated(newTicket);
        }
    }


    
   
}