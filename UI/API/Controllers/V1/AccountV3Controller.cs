using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using API.Helpers;
using API.Models;
using API.Models.Business;
using Common.Helpers;
using Core;
using Core.Domain.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Service.Interface.Authentication;
using Swashbuckle.Swagger.Annotations;

namespace API.Controllers.V1
{
    /// <summary>
    /// Account Controller
    /// </summary>
    [RoutePrefix("api/v1/account")]
    public class AccountV3Controller : BaseApiController
    {
        #region fields
        private readonly UserManager<User> _userManager;
        private readonly IAmsApplicationService _amsApplicationService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        #endregion

        #region ctor

        /// <summary>
        /// Account Controller
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="amsApplicationService"></param>
        /// <param name="userService"></param>
        /// <param name="roleService"></param>
        public AccountV3Controller(
            UserManager<User> userManager,
            IAmsApplicationService amsApplicationService,
            IUserService userService,
            IRoleService roleService
            )
        {
            _userManager = userManager;
            _amsApplicationService = amsApplicationService;
            _userService = userService;
            _roleService = roleService;
        }
        #endregion

        #region return result

        /// <summary>
        /// Register result
        /// </summary>
        public class RegisterResponse : JsonResult
        {
            /// <summary>
            /// Temp user submission Id
            /// </summary>
            public string UserSubmissionId { get; set; }

            /// <summary>
            /// Redirect Url to Payment on Chargify
            /// </summary>
            public string RedirectUrl { get; set; }
        }

        /// <summary>
        /// Request new password result
        /// </summary>
        public class ForgetPasswordResponse : JsonResult
        {

        }
        /// <summary>
        /// Confirm Email response
        /// </summary>
        public class ConfirmEmailResponse : JsonResult
        {

        }

        /// <summary>
        /// InviteUser Response
        /// </summary>
        public class InviteUserResponse : JsonResult
        {

        }

        /// <summary>
        /// RegisterUserInvite Response
        /// </summary>
        public class RegisterUserInviteResponse : JsonResult
        {

        }

        /// <summary>
        /// GetAllUser response 
        /// </summary>
        public class GetAllUserResponse : JsonResult
        {
            /// <summary>
            /// user list
            /// </summary>
            public List<UserSimpleInfo> Data { get; set; }
        }
        #endregion

        #region Utilities
        private string GetAppId()
        {
            var listApplications = _amsApplicationService.GetAllApplications();

            if (listApplications.Count == 0)
            {
                return null;
            }

            return listApplications[0].Id;
        }
        #endregion

        #region public method
        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        [Route("logout")]
        [SwaggerResponse(200, ResponseSuccesss)]
        [SwaggerResponse(401, PermissionDontHave)]
        [SwaggerResponse(500, ServerError)]
        [SwaggerResponse(404, PageNotFound)]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }



        /// <summary>
        /// get user info
        /// </summary>
        /// <returns></returns>
        [Route("getuserinfo")]
        [SwaggerResponse(200, ResponseSuccesss)]
        [SwaggerResponse(401, PermissionDontHave)]
        [SwaggerResponse(500, ServerError)]
        [SwaggerResponse(404, PageNotFound)]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserInfo()
        {
            try
            {
                //check current user is null
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);
                var currentUser = _userService.GetUserById(currentUserId);

                if (currentUser == null)
                {
                    return Error("Unauthorized", HttpStatusCode.Unauthorized);
                }

                //get roles of user
                var roles = _roleService.GetRolesOfUser(currentUser.Id);

                //prepair result
                var resultData = new UserInfo
                {
                    Id = currentUser.Id,
                    Email = currentUser.Email,
                    LastName = currentUser.LastName,
                    FirstName = currentUser.FirstName,
                    UserName = currentUser.UserName,
                    PhoneNumber = currentUser.Phone,
                    Permissions = roles.Select(m => new Permission()
                    {
                        RoleId = m.Id,
                        RoleName = m.Name
                    }).ToList(),
                    OndemandStatus = null,
                };


                return Success(resultData);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }

        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <remarks>
        ///  For native device, add "<span style='font-weight: bold;'>Authorization: Basic xxx</span>" into Header of request.<br />
        ///  For browser, add "<span style='font-weight: bold;'>client_id</span>" and "<span style='font-weight: bold;'>client_secret</span>" into body of request. <br />
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("forgotpassword")]
        [SwaggerResponse(200, ResponseSuccesss)]
        [SwaggerResponse(401, PermissionDontHave)]
        [SwaggerResponse(500, ServerError)]
        [SwaggerResponse(404, PageNotFound)]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordResquest model)
        {
            return await CheckBaseToken(async (application) =>
            {
                var user = await _userService.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    return Error("Email is not exist.");
                }
                if (!(await _userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return Error("Email has not been confirmed before");
                }

                // Send an email with this link
                string code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);

                var fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                var fromName = ConfigurationManager.AppSettings["FromName"];
                //var domain = ConfigurationManager.AppSettings["BaseUrl"];

                //var restPasswordTemplate = _emailService.GetMailTemplate("ForgotPassword.html");

                //var callbackUrl = ConfigurationManager.AppSettings["BaseUrl"] +
                //                  string.Format(Constant.ResetPasswordUrl, user.Id,
                //                      StringHelper.Base64ForUrlEncode(code));
                //restPasswordTemplate = restPasswordTemplate.Replace("#resetUrl", callbackUrl);

                //await _emailService.SendEmailAsync(fromEmail,
                //    fromName,
                //    new List<string>() {user.Email},
                //    null,
                //    null,
                //    "Reset password instructions",
                //    restPasswordTemplate);

                return Ok();
            });
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="code">code</param>
        /// <returns></returns>
        [HttpGet]
        [Route("resetpassword")]
        [SwaggerResponse(200, ResponseSuccesss)]
        [SwaggerResponse(401, PermissionDontHave)]
        [SwaggerResponse(500, ServerError)]
        [SwaggerResponse(404, PageNotFound)]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ResetPassword(string userId, string code)
        {
            return await CheckBaseToken(async (application) =>
            {
                if (userId == null || code == null)
                {
                    return Error("Token invalid.");
                }
                var user = _userService.GetUserById(userId);
                if (user == null)
                {
                    return Error("Token invalid.");
                }
                code = StringHelper.Base64ForUrlDecode(code);
                if (!await _userManager.UserTokenProvider.ValidateAsync("ResetPassword",
                    code, _userManager, user))
                {
                    return Error("Token invalid.");
                }

                return Ok();
            });
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <remarks>
        ///  For native device, add "<span style='font-weight: bold;'>Authorization: Basic xxx</span>" into Header of request.<br />
        ///  For browser, add "<span style='font-weight: bold;'>client_id</span>" and "<span style='font-weight: bold;'>client_secret</span>" into body of request. <br />
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resetpassword")]
        [SwaggerResponse(200, ResponseSuccesss)]
        [SwaggerResponse(401, PermissionDontHave)]
        [SwaggerResponse(500, ServerError)]
        [SwaggerResponse(404, PageNotFound)]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordRequest model)
        {
            return await CheckBaseToken(async (application) =>
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return Error("User not exist");
                }
                var result = await _userManager.ResetPasswordAsync(user.Id,
                    StringHelper.Base64ForUrlDecode(model.Code), model.Password);
                if (!result.Succeeded)
                {
                    return Error(result.Errors.FirstOrDefault());
                }

                //update salt code
                string saltKey = CommonSecurityHelper.CreateSaltKey(5);
                var fuser = _userService.GetUserById(user.Id);

                fuser.SaltDigitCodeHash = saltKey;
                fuser.DigitCodeHash = CommonSecurityHelper.CreatePasswordHash(model.Password, saltKey);
                _userService.UpdateUser(fuser);

                // Send mail
                var fromEmail = ConfigurationManager.AppSettings["FromEmail"];
                var fromName = ConfigurationManager.AppSettings["FromName"];
                var supportEmail = ConfigurationManager.AppSettings["SupportEmail"];
                var domain = ConfigurationManager.AppSettings["BaseUrl"];
                //var passwordChangedTemplate = _emailService.GetMailTemplate("PasswordChanged.html");

                //var userFullName = string.Format("{0} {1}", user.FirstName, user.LastName);
                //passwordChangedTemplate = passwordChangedTemplate.Replace("#username", userFullName);

                //passwordChangedTemplate = passwordChangedTemplate.Replace("#supportemail", supportEmail);

                //var changePassordLink = string.Format("/{0}{1}", domain, Constant.ChangePasswordUrl);
                //passwordChangedTemplate = passwordChangedTemplate.Replace("#changepasswordlink", changePassordLink);
                //try
                //{
                //    await _emailService.SendEmailAsync(fromEmail,
                //        fromName,
                //        new List<string>() {user.Email},
                //        null,
                //        null,
                //        "Password changed",
                //        passwordChangedTemplate);
                //}
                //catch (Exception ex)
                //{
                //    Trace.TraceError(ex.Message);
                //}

                return Ok();
            });
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("changepassword")]
        [HttpPost]
        [SwaggerResponse(200, ResponseSuccesss)]
        [SwaggerResponse(401, PermissionDontHave)]
        [SwaggerResponse(500, ServerError)]
        [SwaggerResponse(404, PageNotFound)]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            //validate model
            var errorMessages = ValidateModel(ModelState);
            if (errorMessages.Count > 0)
            {
                return Error(errorMessages);
            }

            var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);
            var currentUser = _userService.GetUserById(currentUserId);

            //validate old password
            var verifyCode = CommonSecurityHelper.CreatePasswordHash(model.OldPassword, currentUser.SaltDigitCodeHash);

            if (verifyCode != currentUser.DigitCodeHash)
            {
                return Error("Current password is not correct.");
            }

            var result = await _userManager.RemovePasswordAsync(currentUser.Id);
            if (!result.Succeeded)
            {
                return Error(result.Errors);
            }
            var resultChangePass = await _userManager.AddPasswordAsync(currentUser.Id, model.NewPassword);

            if (!resultChangePass.Succeeded)
            {
                return Error(result.Errors);
            }
            //update salt code
            string saltKey = CommonSecurityHelper.CreateSaltKey(5);
            var fuser = _userService.GetUserById(currentUser.Id);

            fuser.SaltDigitCodeHash = saltKey;
            fuser.DigitCodeHash = CommonSecurityHelper.CreatePasswordHash(model.NewPassword, saltKey);
            _userService.UpdateUser(fuser);

            // Send mail
            var fromEmail = ConfigurationManager.AppSettings["FromEmail"];
            var fromName = ConfigurationManager.AppSettings["FromName"];
            var supportEmail = ConfigurationManager.AppSettings["SupportEmail"];
            var domain = ConfigurationManager.AppSettings["BaseUrl"];
            //var passwordChangedTemplate = _emailService.GetMailTemplate("PasswordChanged.html");

            //passwordChangedTemplate = passwordChangedTemplate.Replace("#username",
            //    string.Format("{0} {1}", currentUser.FirstName, currentUser.LastName));
            //passwordChangedTemplate = passwordChangedTemplate.Replace("#supportemail", supportEmail);
            //passwordChangedTemplate = passwordChangedTemplate.Replace("#changepasswordlink",
            //    string.Format("{0}{1}", domain, Constant.ChangePasswordUrl));

            //await _emailService.SendEmailAsync(fromEmail,
            //    fromName,
            //    new List<string> {currentUser.Email},
            //    null,
            //    null,
            //    "Password changed",
            //    passwordChangedTemplate);

            return Ok();
        }


        /// <summary>
        /// update User Profile
        /// </summary>
        /// <param name="userUpdateViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updateprofile")]
        [SwaggerResponse(200, ResponseSuccesss)]
        [SwaggerResponse(204, NoContent)]
        [SwaggerResponse(401, PermissionDontHave)]
        [SwaggerResponse(500, ServerError)]
        [SwaggerResponse(404, PageNotFound)]
        public async Task<IHttpActionResult> UpdateProfile(UserUpdate userUpdateViewModel)
        {
            //validate model
            var errorMessages = ValidateModel(ModelState);
            if (errorMessages.Count > 0)
            {
                return Error(errorMessages);
            }

            var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);
            var currentUser = _userService.GetUserById(currentUserId);

            if (currentUser == null)
            {
                return Error("Unauthorized", System.Net.HttpStatusCode.Unauthorized);
            }

            //get current user by id
            var user = _userService.GetUserById(currentUser.Id);

            user.FirstName = userUpdateViewModel.FirstName;
            user.LastName = userUpdateViewModel.LastName;
            user.Phone = userUpdateViewModel.PhoneNumber;

            try
            {
                await _userService.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }

            return Updated();
        }

        /// <summary>
        /// update user profile avatar
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("updateprofileavatar")]
        [SwaggerResponse(200, ResponseSuccesss)]
        [SwaggerResponse(401, PermissionDontHave)]
        [SwaggerResponse(500, ServerError)]
        [SwaggerResponse(404, PageNotFound)]
        public async Task<IHttpActionResult> UpdateProfileAvatar()
        {
            var request = Request.Headers;
            var fileId = "file";
            //var userId = !string.IsNullOrEmpty(request.Form[0])
            //    ? request.Form["userId"]
            //    : _workContext.CurrentUser.Id;
            var userId = User.GetValueOfClaim(ClaimName.UseridKey);
            try
            {
                // This endpoint only supports multipart form data
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }

                //validate first
                //var results = await _mediaService.ValidateWhenUploadFilesAsync(Request.Content, UploadMimeType.Image, fileId);
                //foreach (var result in results)
                //{
                //    if (result.Success)
                //    {
                //        var image = new Bitmap(result.LocalFile);
                //        var thumbnailWidth = image.Width;
                //        var thumbnailHeight = image.Height;
                //        if (thumbnailWidth > thumbnailHeight)
                //        {
                //            thumbnailWidth = thumbnailHeight;
                //        }
                //        else if (thumbnailHeight > thumbnailWidth)
                //        {
                //            thumbnailHeight = thumbnailWidth;
                //        }
                //        var filePath = string.Format("{0}/1", userId);

                //        // Resize image before upload
                //        var settings = new ResizeSettings
                //        {
                //            Width = thumbnailWidth,
                //            Height = thumbnailHeight,
                //            Format = "jpg",
                //            Mode = FitMode.Crop
                //        };

                //        using (var ms = new MemoryStream())
                //        {
                //            ImageBuilder.Current.Build(result.LocalFile, ms, settings);
                //            ms.Seek(0, SeekOrigin.Begin);
                //            _mediaService.SaveFile(result, filePath, ms);

                //            return Success(new
                //            {
                //                Status = true,
                //                FileUrl = string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["MediaBaseUrl"], ConfigurationManager.AppSettings["AvatarFolderName"], filePath),
                //                FileId = result.FileName,
                //                FileName = result.LocalName.Replace("\"", "")
                //            });
                //        }
                //    }

                //}
                //// Call service to perform upload, then check result to return as content

                return Error("File Invalid");
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        #endregion

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizationValidFunc"></param>
        /// <returns></returns>
        protected async Task<IHttpActionResult> CheckBaseToken(
            Func<AmsApplication, Task<IHttpActionResult>> authorizationValidFunc)
        {
            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["Authorization"];
            if (authHeader == null)
            {
                return Error("Missing app credential");
            }
            else
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);
                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                    StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    try
                    {
                        var encoding = Encoding.GetEncoding("iso-8859-1");
                        var credentials = encoding.GetString(Convert.FromBase64String(authHeaderVal.Parameter));

                        int separator = credentials.IndexOf(':');
                        string clientAppId = credentials.Substring(0, separator);
                        string clientAppSecret = credentials.Substring(separator + 1);
                        var application = _amsApplicationService.GetById(clientAppId);
                        if (application == null)
                        {
                            return Error("App credential is invalid");
                        }
                        else
                        {
                            if (application.AppSecret != CommonSecurityHelper.GetHash(clientAppSecret))
                            {
                                return Error("Missing app credential");
                            }

                            //validate model
                            var errorMessages = ValidateModel(ModelState);
                            if (errorMessages.Count > 0)
                            {
                                return Error(errorMessages);
                            }

                            return await authorizationValidFunc(application);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Error(ex);
                    }
                }
                else
                {
                    return Error("Missing app credential");
                }
            }
        }

        #endregion
    }
}
