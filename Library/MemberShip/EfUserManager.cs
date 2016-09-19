using System;
using Core.Domain.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Membership
{
    public class EfUserManager : UserManager<User>
    {
        public EfUserManager(IUserStore<User> userStore, IDataProtectionProvider _dataProtectionProvider)
            : base(userStore)
        {
            // Configure validation logic for usernames
            UserValidator = new UserValidator<User>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false,

            };
            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6
                //RequireNonLetterOrDigit = true,
                //RequireDigit = true,
                //RequireLowercase = true,
                //RequireUppercase = true,
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            var dataProtectionProvider = _dataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("AMS"))
                {
                    TokenLifespan = TimeSpan.FromHours(6)
                };

            }

        }



        //public override Task<IList<Claim>> GetClaimsAsync(string userId)
        //{

        //    return base.GetClaimsAsync(userId);
        //}


    }
}
