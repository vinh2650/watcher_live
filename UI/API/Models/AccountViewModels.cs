using System;
using System.Collections.Generic;

namespace API.Models
{
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }

    /// <summary>
    /// Store user info for post
    /// </summary>
    public class UserInfoForNotificationViewModel
    {
        /// <summary>
        /// Id of user
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Email of user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// first name of user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name of user
        /// </summary>
        public string LastName { get; set; }
    }
}
