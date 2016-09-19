using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin.Security.Infrastructure;
using Service.Interface.Authentication;

namespace API.Providers
{
    /// <summary>
    /// Represent custom refresh token provider
    /// </summary>
    public class CustomRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly Func<IRefreshTokenService> _refreshTokenServiceProvider;

        private IRefreshTokenService _refreshTokenService
        {
            get
            {
                return this._refreshTokenServiceProvider.Invoke();
            }
        }

        public CustomRefreshTokenProvider(Func<IRefreshTokenService> refreshTokenServiceProvider)
        {
            _refreshTokenServiceProvider = refreshTokenServiceProvider;
        }
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }
        //Create refresh token after validate user
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var refreshTokenId = context.Ticket.Properties.Dictionary["as:r_t_id"];

            if (string.IsNullOrEmpty(refreshTokenId))
            {
                return;
            }
            var token = _refreshTokenService.GetRefreshTokenById(refreshTokenId);
            
            context.Ticket.Properties.IssuedUtc = token.IssuesDateTimeUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiredDateTimeUtc;

            token.ProtectedTicket = context.SerializeTicket();

            await _refreshTokenService.UpdateRefreshTokenAsync(token);
            context.SetToken(token.Id);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = context.Token;
            var token = _refreshTokenService.GetRefreshTokenById(hashedTokenId);

            if (token != null)
            {
                
                if (DateTime.UtcNow < token.ExpiredDateTimeUtc)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(token.ProtectedTicket);
                    token.HasChangeClaim = false;
                    _refreshTokenService.UpdateRefreshToken(token);
                }
            }
        }
    }
}