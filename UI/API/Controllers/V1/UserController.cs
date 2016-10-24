using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using API.Helpers;
using API.Models.Business;
using Common.Helpers;
using Core.Domain.Authentication;
using Microsoft.AspNet.Identity;
using Service.Interface.Authentication;
using Swashbuckle.Swagger.Annotations;

namespace API.Controllers.V1
{
    /// <summary>
    /// User controller
    /// </summary>
    [RoutePrefix("api/v1/user")]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IRoleService _roleService;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="roleService"></param>
        /// <param name="userManager"></param>
        public UserController(IUserService userService,
            IRoleService roleService,
            UserManager<User> userManager)
        {
            _userService = userService;
            _roleService = roleService;
            _userManager = userManager;
        }


        /// <summary>
        /// Register a new user
        /// </summary>
        /// <returns></returns>
        [Route("register")]
        [AllowAnonymous]
        [SwaggerResponse(200, ResponseSuccesss)]
        [SwaggerResponse(401, PermissionDontHave)]
        [SwaggerResponse(500, ServerError)]
        [SwaggerResponse(404, PageNotFound)]
        public IHttpActionResult Regsiter([FromBody]UserCreate model)
        {
            //init params
            var saltKey = CommonSecurityHelper.CreateSaltKey(5);
            //var currentPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            var userRole = _roleService.GetRoleByName(RoleSystemName.User);

            if (model.Passwords != model.ConfirmPasswords)
                return Error("Your passwords does match");

            var currentPassword = model.Passwords;

            //validate model
            var errorMessages = ValidateModel(ModelState);
            if (errorMessages.Count > 0)
            {
                return Error(errorMessages);
            }

            //check user by email
            var user = _userService.GetUserByEmail(model.Email);
            if (user != null)
                return Error("User has exists");

            //prepair user
            user = new User()
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                OrganizationDestinationId = model.OrganizationDestinationId,
                Phone = model.PhoneNumber,
                DigitCodeHash = CommonSecurityHelper.CreatePasswordHash(currentPassword, saltKey),
                SaltDigitCodeHash = saltKey,
                Active = true,
                IsEmailConfirmed = true,
                ParentId = User.GetValueOfClaim(ClaimName.UseridKey)
            };

            //create user
            try
            {
                _userService.CreateUser(user);

                //add role to user
                _roleService.AddRoleToUser(user.Id, userRole.Id);

                ////get template email
                //var headerEmail = "New account has been created for you";
                //var userFullName = $"{user.FirstName} {user.LastName}";
                //var changePassordLink = $"{domain}{Constant.ChangePasswordUrl}";

                // do not forget it ! If you forget, the reset password feature will be broke
                _userManager.UpdateSecurityStamp(user.Id);
            }
            catch (Exception ex)
            {
                return Error(ex);
            }

            //prepair result
            var result = new UserCreateResult()
            {
                Id = user.Id,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                OrganizationDestinationId = model.OrganizationDestinationId,
                PhoneNumber = model.PhoneNumber,
                Role = RoleSystemName.User
            };
            return Success(result);
        }

        /// <summary>
        /// Get list of user by list ids
        /// </summary>
        /// <returns></returns>
        [Route("ids")]
        [HttpPost]
        public IHttpActionResult GetUsersByListId([FromBody]GetListUserModel model)
        {
            try
            {
                var res = new List<User>();
                var err = new List<String>();

                foreach (var id in model.Ids)
                {
                    var findUser = _userService.GetUserById(id);
                    if (findUser != null)
                    {
                        res.Add(findUser);
                    }
                    else
                    {
                        err.Add(id);
                    }
                }
                if (err.Any())
                    return Error(err);

                return Success(res);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// Get list of user by list ids
        /// </summary>
        /// <returns></returns>
        [Route("{userId}")]
        [HttpPost]
        public IHttpActionResult GetUserById([FromUri]string userId)
        {
            try
            {
                var findUser = _userService.GetUserById(userId);
                if (findUser != null)
                    return Success(findUser);
                
                return Error("UserId not exist");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [Route("byname/{name}")]
        [HttpPost]
        public IHttpActionResult GetUserByName([FromUri]string name)
        {
            try
            {
                //var findUser = _userService.GetUserById();
                //if (findUser != null)
                //    return Success(findUser);

                return Error("UserId not exist");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
