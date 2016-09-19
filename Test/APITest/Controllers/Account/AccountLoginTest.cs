using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Common.Helpers;
using Core.Domain.Authentication;
using Microsoft.AspNet.Identity;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace APITest.Controllers.Account
{
    [TestFixture]
    public class when_some_one_login : BaseServerTest
    {
        public User FakeLoginUserNotConfirmEmal = new User()
        {
            Id = "2487c7a0-be5e-416f-ba51-39d334d7cdca",
            UserName = "TestUser1",
            Email = "testuser1@test.com",
            IsEmailConfirmed = false,
            Active = true,
            SaltDigitCodeHash = SalkKey,
            DigitCodeHash = CommonSecurityHelper.CreatePasswordHash("123456", SalkKey)
        };

        public User FakeLoginUserNotActive = new User()
        {
            Id = "de295fd9-5b1d-4c9d-925d-83865504a13a",
            UserName = "TestUser2",
            Email = "testuser2@test.com",
            IsEmailConfirmed = true,
            Active = false,
            SaltDigitCodeHash = SalkKey,
            DigitCodeHash = CommonSecurityHelper.CreatePasswordHash("123456", SalkKey)
        };

        #region fields

        private string uriBase = "/account";
        private string tokenBase = "";
        private string _uri = string.Empty;

        protected override string Uri => _uri;

        #endregion

        /// <summary>
        /// Inject object
        /// </summary>
        /// <param name="builder"></param>
        public override void InjectObject(ContainerBuilder builder)
        {
            

        }



        #region login

        [Category("LoginTest")]
        [Test]
        public async Task should_return_bad_request_if_missing_basic_authorize()
        {
            _uri = tokenBase + "/token";
            var tokenDetails = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", "khanh.vu@newoceaninfosys.com"),
                new KeyValuePair<string, string>("password", "asdasd")
            };
            var response = await PostFormAsync(tokenDetails);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        }

        [Category("LoginTest")]
        [Test]
        public async Task should_return_bad_request_if_basic_authorize_is_incorrect()
        {
            _uri = tokenBase + "/token";
            var tokenDetails = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", "khanh.vu@newoceaninfosys.com"),
                new KeyValuePair<string, string>("password", "asdasd")
            };
            var response = await PostFormWithBasicAuthorizationAsync(tokenDetails, "c2FkYTphc2QxMjEy");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        }

        [Category("LoginTest")]
        [Test]
        public async Task should_return_bad_request_if_invalid_credential()
        {
            _uri = tokenBase + "/token";
            var tokenDetails = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", "khanh.vu@newoceaninfosys.com"),
                new KeyValuePair<string, string>("password", "asdasd")
            };
            var response =
                await
                    PostFormWithBasicAuthorizationAsync(tokenDetails,
                        "NjY0OTA0OWMtMDI4NS00ODc2LWI2OWEtZmUzZDA1YzhkNTU1Ojc5OTM1Zjk5LWFmNzQtNDhmMC04NjM0LWY0YTQ3ZmZlNDI2Yg==");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Category("LoginTest")]
        [Test]
        public async Task should_return_bad_request_if_user_is_not_active()
        {
            _uri = tokenBase + "/token";
            var tokenDetails = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", FakeLoginUserNotActive.UserName),
                new KeyValuePair<string, string>("password", "123456")
            };
            var response =
                await
                    PostFormWithBasicAuthorizationAsync(tokenDetails,
                        "NjY0OTA0OWMtMDI4NS00ODc2LWI2OWEtZmUzZDA1YzhkNTU1Ojc5OTM1Zjk5LWFmNzQtNDhmMC04NjM0LWY0YTQ3ZmZlNDI2Yg==");
            var res = response.Content.ReadAsStringAsync().Result;
            var body = JObject.Parse(res);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("invalid_grant", (string)body["error"]);
            Assert.AreEqual("User is not active", (string)body["error_description"]);

        }

        [Category("LoginTest")]
        [Test]
        public async Task should_return_bad_request_if_user_not_confirm_email()
        {
            _uri = tokenBase + "/token";
            var tokenDetails = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", FakeLoginUserNotConfirmEmal.UserName),
                new KeyValuePair<string, string>("password", "123456")
            };
            var response =
                await
                    PostFormWithBasicAuthorizationAsync(tokenDetails,
                        "NjY0OTA0OWMtMDI4NS00ODc2LWI2OWEtZmUzZDA1YzhkNTU1Ojc5OTM1Zjk5LWFmNzQtNDhmMC04NjM0LWY0YTQ3ZmZlNDI2Yg==");
            var res = response.Content.ReadAsStringAsync().Result;
            var body = JObject.Parse(res);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("invalid_grant", (string)body["error"]);
            Assert.AreEqual("Email is not comfirm", (string)body["error_description"]);
        }

        [Category("LoginTest")]
        [Test]
        public async Task should_return_bad_request_if_user_credential_is_incorrect()
        {
            _uri = tokenBase + "/token";
            var tokenDetails = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", FakeLoginUser.UserName),
                new KeyValuePair<string, string>("password", "1234567")
            };
            var response =
                await
                    PostFormWithBasicAuthorizationAsync(tokenDetails,
                        "NjY0OTA0OWMtMDI4NS00ODc2LWI2OWEtZmUzZDA1YzhkNTU1Ojc5OTM1Zjk5LWFmNzQtNDhmMC04NjM0LWY0YTQ3ZmZlNDI2Yg==");
            var res = response.Content.ReadAsStringAsync().Result;
            var body = JObject.Parse(res);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("invalid_grant", (string)body["error"]);
            Assert.AreEqual("The credential is incorrect.", (string)body["error_description"]);
        }

        [Category("LoginTest")]
        [Test]
        public async Task should_return_bad_request_if_username_not_found_in_system()
        {
            _uri = tokenBase + "/token";
            var tokenDetails = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", "johndoe"),
                new KeyValuePair<string, string>("password", "123456")
            };
            var response =
                await
                    PostFormWithBasicAuthorizationAsync(tokenDetails,
                        "NjY0OTA0OWMtMDI4NS00ODc2LWI2OWEtZmUzZDA1YzhkNTU1Ojc5OTM1Zjk5LWFmNzQtNDhmMC04NjM0LWY0YTQ3ZmZlNDI2Yg==");
            var res = response.Content.ReadAsStringAsync().Result;
            var body = JObject.Parse(res);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("invalid_grant", (string)body["error"]);
            Assert.AreEqual("Not exist this user.", (string)body["error_description"]);
        }

        [Category("LoginTest")]
        [Test]
        public async Task should_return_token_if_valid_data()
        {
            _uri = tokenBase + "/token";
            var tokenDetails = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", FakeLoginUser.UserName),
                new KeyValuePair<string, string>("password", "123456")
            };
            var response =
                await
                    PostFormWithBasicAuthorizationAsync(tokenDetails,
                        "NjY0OTA0OWMtMDI4NS00ODc2LWI2OWEtZmUzZDA1YzhkNTU1Ojc5OTM1Zjk5LWFmNzQtNDhmMC04NjM0LWY0YTQ3ZmZlNDI2Yg==");
            var res = response.Content.ReadAsStringAsync().Result;
            var body = JObject.Parse(res);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull((string)body["access_token"]);
        }

        [Category("LoginTest")]
        [Test]
        public async Task should_return_new_access_token_when_get_by_refresh_token()
        {

            _uri = tokenBase + "/token";
            var tokenDetails = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", FakeLoginUser.UserName),
                new KeyValuePair<string, string>("password", "123456")
            };
            var response =
                await
                    PostFormWithBasicAuthorizationAsync(tokenDetails,
                        "NjY0OTA0OWMtMDI4NS00ODc2LWI2OWEtZmUzZDA1YzhkNTU1Ojc5OTM1Zjk5LWFmNzQtNDhmMC04NjM0LWY0YTQ3ZmZlNDI2Yg==");
            var res = response.Content.ReadAsStringAsync().Result;
            var body = JObject.Parse(res);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var accessToken = (string)body["access_token"];
            var refreshToken = (string)body["refresh_token"];
            Assert.NotNull(accessToken);
            Assert.NotNull(refreshToken);
            var tokenDetailsNew = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            };
            var response2 =
                await
                    PostFormWithBasicAuthorizationAsync(tokenDetailsNew,
                        "NjY0OTA0OWMtMDI4NS00ODc2LWI2OWEtZmUzZDA1YzhkNTU1Ojc5OTM1Zjk5LWFmNzQtNDhmMC04NjM0LWY0YTQ3ZmZlNDI2Yg==");
            var res2 = response2.Content.ReadAsStringAsync().Result;
            var body2 = JObject.Parse(res2);

            var accessTokenNew = (string)body2["access_token"];
            var refreshTokenNew = (string)body2["refresh_token"];
            Assert.NotNull(accessTokenNew);
            Assert.NotNull(refreshTokenNew);

        }

        #endregion

    }
}
