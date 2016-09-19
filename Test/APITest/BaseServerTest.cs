    using System;
using System.Collections.Generic;
using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;
using System.Threading.Tasks;
    using System.Web.Http;
    using API;
    using API.Infrastructure;
    using API.Providers;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Common.Helpers;
    using Core.Domain.Authentication;
    using Microsoft.Owin.Security.DataProtection;
    using Microsoft.Owin.Testing;
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using Owin;

namespace APITest
{
    
    [TestFixture]
    public abstract class BaseServerTest
    {

        #region mock data
        private static string _salkKey;

        public static string SalkKey
        {
            get
            {
                if (string.IsNullOrEmpty(_salkKey))
                {
                    _salkKey = CommonSecurityHelper.CreateSaltKey(5);
                }
                return _salkKey;
            }
        }

        public User FakeLoginUser = new User()
        {
            Id = "de295fd9-5b1d-4c9d-925d-83865504a13a",
            UserName = "TestUser3",
            Email = "testuser3@test.com",
            IsEmailConfirmed = true,
            Active = true,
            SaltDigitCodeHash = SalkKey,
            DigitCodeHash = CommonSecurityHelper.CreatePasswordHash("123456", SalkKey)
        };
        #endregion

        private TestServer _server;




        [SetUp]
        public void Setup()
        {
            _server = TestServer.Create(app =>
            {
                var startup = new Startup();
                var httpConfig = new HttpConfiguration();
                //register DI here
                // this now returns ContainerBuilder instead of the container

                var builder = new ContainerBuilder();

                builder.RegisterAssemblyTypes(typeof(ApiController).Assembly);

                builder.RegisterApiControllers(typeof(WebApiApplication).Assembly);

                var dataProtectionProvider = new MachineKeyProtectionProvider();
                builder.Register<IDataProtectionProvider>(cc => dataProtectionProvider).InstancePerDependency();


                RegisterFakeBaseService(builder);


                InjectObject(builder);


                var container = builder.Build();

                WebApiConfig.Register(httpConfig);
                var resolver = new AutofacWebApiDependencyResolver(container);
                httpConfig.DependencyResolver = resolver;
                // DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
                // Register the Autofac middleware FIRST, then the Autofac MVC middleware.
                app.UseAutofacMiddleware(container);
                app.UseAutofacWebApi(httpConfig);
                startup.ConfigureAuth(app, resolver);
                app.UseWebApi(httpConfig);


            });
            GetAccessToken(_server);

        }

        private void RegisterFakeBaseService(ContainerBuilder builder)
        {
           
        }


        /// <summary>
        /// virtual function for inject service 
        /// </summary>
        /// <param name="builder"></param>
        public virtual void InjectObject(ContainerBuilder builder)
        {

        }

        private static string _accessToken;

        public static string AccessToken
        {
            get
            {
                return _accessToken;
            }
        }

        public void GetAccessToken(TestServer server)
        {
            var mUri = "/token";
            var tokenDetails = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", FakeLoginUser.UserName),
                new KeyValuePair<string, string>("password", "123456")
            };

            // var tokenPostData = new FormUrlEncodedContent(tokenDetails);

            var tokenResult = AsyncHelper.RunSync(() => PostFormWithBasicAuthorizationAsync(mUri, tokenDetails,
                "NDFhNzdhZGUtYjI5ZS00MmQzLWJhZDMtODYyYjk2ZWJlNDA5OjM3Y2ZiOTA1LTM2ZTUtNDBhMC1hYzg0LTUyMjcxMmM4NDMzNg=="));


            Assert.AreEqual(HttpStatusCode.OK, tokenResult.StatusCode);
            var res = tokenResult.Content.ReadAsStringAsync().Result;
            var body = JObject.Parse(res);
            _accessToken = (string)body["access_token"];
        }

        [TearDown]
        public void Teardown()
        {
            if (_server != null)
                _server.Dispose();
        }
        protected abstract string Uri { get; }
        private string UriForGetToken { get { return "/token"; } }
        protected virtual async Task<HttpResponseMessage> GetAsync()
        {
            return await _server.CreateRequest(Uri)
                .GetAsync();
        }

        protected virtual async Task<HttpResponseMessage> GetAsyncWithBearerCode(string bearerCode)
        {
            var header = "Bearer " + bearerCode;
            return await _server.CreateRequest(Uri)
                .AddHeader("Authorization", header)
                .GetAsync();

        }

        protected virtual async Task<HttpResponseMessage> PostAsync<TModel>(TModel model)
        {
            return await _server.CreateRequest(Uri)
                .And(request => request.Content = new ObjectContent(typeof(TModel), model, new JsonMediaTypeFormatter()))
                .PostAsync();
        }
        protected virtual async Task<HttpResponseMessage> PutAsync<TModel>(TModel model)
        {
            return await _server.CreateRequest(Uri)
                .And(request => request.Content = new ObjectContent(typeof(TModel), model, new JsonMediaTypeFormatter()))
                .SendAsync("PUT");
        }
        protected virtual async Task<HttpResponseMessage> PostFormAsync(List<KeyValuePair<string, string>> model)
        {
            return await _server.CreateRequest(Uri)
                .And(request => request.Content = new FormUrlEncodedContent(model))

            .PostAsync();
        }


        protected virtual async Task<HttpResponseMessage> PostWithBasicAuthorizationAsync<TModel>(TModel model, string basicCode)
        {
            return await _server.CreateRequest(Uri)
                .And(request => request.Content = new ObjectContent(typeof(TModel), model, new JsonMediaTypeFormatter()))
                .AddHeader("Authorization", "Basic " + basicCode)
                .PostAsync();
        }
        protected virtual async Task<HttpResponseMessage> PostFormWithBasicAuthorizationAsync(List<KeyValuePair<string, string>> model, string basicCode)
        {
            return await _server.CreateRequest(Uri)
                .And(request => request.Content = new FormUrlEncodedContent(model))
                .AddHeader("Authorization", "Basic " + basicCode)
                .PostAsync();
        }
        protected virtual async Task<HttpResponseMessage> PostFormWithBasicAuthorizationAsync(string url, List<KeyValuePair<string, string>> model, string basicCode)
        {
            return await _server.CreateRequest(url)
                .And(request => request.Content = new FormUrlEncodedContent(model))
                .AddHeader("Authorization", "Basic " + basicCode)
                .PostAsync();
        }
        protected virtual async Task<HttpResponseMessage> PostWithTokenAuthorizationAsync<TModel>(TModel model, string accessToken)
        {
            return await _server.CreateRequest(Uri)
                .And(request => request.Content = new ObjectContent(typeof(TModel), model, new JsonMediaTypeFormatter()))
                .AddHeader("Authorization", "Bearer " + accessToken)
                .PostAsync();
        }

        protected virtual async Task<HttpResponseMessage> PutWithTokenAuthorizationAsync<TModel>(TModel model, string accessToken)
        {
            return await _server.CreateRequest(Uri)
                .And(
                    request => request.Content = new ObjectContent(typeof(TModel), model, new JsonMediaTypeFormatter()))
                .AddHeader("Authorization", "Bearer " + accessToken)
                .SendAsync("PUT");
        }
        protected virtual async Task<HttpResponseMessage> PostFile(MultipartFormDataContent content)
        {
            return await _server.CreateRequest(Uri)
                .And(x => x.Content = content)
                .PostAsync();
        }
        protected virtual async Task<HttpResponseMessage> PostFileWithTokenAuthorizationAsync(HttpContent content, string contentType, string accessToken)
        {
            content.Headers.Add("Content-Type", contentType);
            return await _server.CreateRequest(Uri)
                .And(x => x.Content = new MultipartFormDataContent { content })
                .AddHeader("Content-Type", "multipart/form-data")
                  .AddHeader("Authorization", "Bearer " + accessToken)
                .PostAsync();
        }
        protected virtual async Task<HttpResponseMessage> DeleteWithTokenAuthorizationAsync<TModel>(TModel model, string accessToken)
        {
            return await _server.CreateRequest(Uri)
                .And(
                    request => request.Content = new ObjectContent(typeof(TModel), model, new JsonMediaTypeFormatter()))
                .AddHeader("Authorization", "Bearer " + accessToken)
                .SendAsync("DELETE");
        }
    }
}
