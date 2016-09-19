using System;
using API.Providers;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Data;
using Service.Implement.Authentication;
using Service.Interface.Authentication;

namespace API
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Startup
    {
        private static AmsApplicationService createamsApplicationService(AutofacWebApiDependencyResolver resolver)
        {
            var amsApplicationService = resolver.GetService(typeof(IAmsApplicationService)) as AmsApplicationService;
            return amsApplicationService;
        }
        /// <summary>
        /// 
        /// </summary>
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="resolver"></param>
        public void ConfigureAuth(IAppBuilder app, AutofacWebApiDependencyResolver resolver)
        {
            Func<IAmsApplicationService> amsApplicationService = () =>
              resolver.GetService(typeof(IAmsApplicationService)) as IAmsApplicationService;
            Func<IRefreshTokenService> refreshTokenService = () =>
              resolver.GetService(typeof(IRefreshTokenService)) as IRefreshTokenService;

            Func<IUserService> userService = () =>
              resolver.GetService(typeof(IUserService)) as IUserService;

            var authorizationServerProvider = new CustomAuthorizationServerProvider(amsApplicationService, userService, refreshTokenService);

            var refreshTokenProvider = new CustomRefreshTokenProvider(refreshTokenService);

            // var minuteExpireAccessToken = int.Parse(ConfigurationManager.AppSettings["AccessTokenLifeTimeInMinute"]);
            OAuthOptions = new OAuthAuthorizationServerOptions()
            {

                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(365),
                Provider = authorizationServerProvider,
                RefreshTokenProvider = refreshTokenProvider
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            //first call to DB genarator data
            //var context = new NoisContext();
            //context.Database.ExecuteSqlCommand("Select top 1 Id from [User]");
        }
    }
}
