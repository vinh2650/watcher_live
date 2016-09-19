using System.Collections.Generic;
using Core.Domain.Authentication;
using Microsoft.Owin;
using Service.Interface.Authentication;

namespace API.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class WorkContext : IWorkContext
    {
        private readonly IOwinContext _owinContext;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        private User _currentUser;
        private string _currentUsername;
        private List<Role> _currentRole;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owinContext"></param>
        /// <param name="userService"></param>
        /// <param name="roleService"></param>
        public WorkContext(
            IOwinContext owinContext, 
            IUserService userService, 
            IRoleService roleService)
        {
            _owinContext = owinContext;
            _userService = userService;
            _roleService = roleService;
        }



        /// <summary>
        /// Current user, use for getting quickly in code
        /// </summary>
        public User CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    if (_owinContext.Authentication != null && _owinContext.Authentication.User.Identity.IsAuthenticated)
                    {
                        var currentUsername = _owinContext.Authentication.User.Identity.Name;
                        _currentUser = _userService.GetUserByUsername(currentUsername);
                    }
                }

                return _currentUser;
            }
        }

        /// <summary>
        /// current user name
        /// </summary>
        public string CurrentUsername
        {
            get
            {
                if (_currentUsername == null)
                {
                    if (_owinContext.Authentication != null && _owinContext.Authentication.User.Identity.IsAuthenticated)
                    {
                        _currentUsername = _owinContext.Authentication.User.Identity.Name;
                    }
                }
                return _currentUsername;
            }
        }

        //todo need to recheck again for what impact
        /// <summary>
        /// the roles of current user.
        /// </summary>
        public List<Role> CurrentRoles
        {
            get
            {
                if (_currentRole == null)
                {
                    var currentUser = CurrentUser;
                    if (currentUser != null)
                    {
                        _currentRole = _roleService.GetRolesOfUser(CurrentUser.Id);
                    }
                }
                return _currentRole;
            }
        }
    }
}