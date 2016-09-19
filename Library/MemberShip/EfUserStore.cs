using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Authentication;
using Microsoft.AspNet.Identity;
using Service.Interface.Authentication;

namespace Membership
{
    public class EfUserStore:IUserStore<User>,
                         IUserClaimStore<User>,
                        IUserSecurityStampStore<User>,
                        IUserRoleStore<User>, 
                        IUserPasswordStore<User>,
                        IUserLockoutStore<User,string>,
                        IUserTwoFactorStore<User,string>,
                        IUserEmailStore<User,string>
    {

        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public EfUserStore(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        public void Dispose()
        {
            
        }

        public Task CreateAsync(User user)
        {
            return _userService.CreateUserAsync(user);
        }

        public Task UpdateAsync(User user)
        {
            return _userService.UpdateUserAsync(user);
        }

        public Task DeleteAsync(User user)
        {
            return _userService.RemoveUserAsync(user);
        }

        public Task<User> FindByIdAsync(string userId)
        {
             var result = _userService.GetUserById(userId);
             return Task.FromResult(result);
        }

        public async Task<User> FindByNameAsync(string userName)
        {
            var result = await _userService.GetUserByUsernameAsync(userName);
            return result;
        }
        
        public async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            ClaimsIdentity identity = await _userService.FindByUserIdAsync(user.Id);
            var res = identity.Claims.ToList();
                        return res;
        }

        public Task AddClaimAsync(User user, Claim claim)
        {
            if(user == null)
                throw new ArgumentNullException("user");
            if(claim==null)
                throw new ArgumentNullException("claim");

            _userService.InsertClaimForUser(claim, user.Id);
            return Task.FromResult<object>(null);
        }

        public Task RemoveClaimAsync(User user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            _userService.DeleteClaimOfUser(user, claim);

            return Task.FromResult<object>(null);
        }

        public Task SetSecurityStampAsync(User user, string stamp)
        {
            user.SecurityStamp = stamp;
           // _userService.UpdateUser(user);
            return Task.FromResult<string>(stamp);
        }

        public Task<string> GetSecurityStampAsync(User user)
        {
            var securityStamp = user.SecurityStamp;
            return Task.FromResult<string>(securityStamp);
        }

        public async Task AddToRoleAsync(User user, string roleName)
        {
            //await _roleService.AddRoleToUserByRoleNameAsync(user.Id, roleName);

        }

        public async Task RemoveFromRoleAsync(User user, string roleName)
        {
            await _roleService.RemoveRoleFromUserAsync(user.Id, roleName);
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            var roles = await _roleService.GetRolesOfUserAsync(user.Id);
            return roles.Select(p => p.Name).ToList();
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName)
        {

           var res=   await _roleService.CheckUserIsInRoleByNameAsync(user.Id, roleName);
            return res;
        }

        
        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            user.SaltDigitCodeHash = passwordHash;
            


            // _userService.UpdateUser(user);
            return Task.FromResult(passwordHash);
            
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
          
            var passwordHash = user.SaltDigitCodeHash;
            return Task.FromResult(passwordHash);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(!String.IsNullOrEmpty(user.SaltDigitCodeHash))  ;
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(User user)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(User user)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(User user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(User user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(User user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetEmailAsync(User user, string email)
        {
            user.Email = email;
            return Task.FromResult(email);
        }

        public Task<string> GetEmailAsync(User user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user)
        {
            return Task.FromResult(user.IsEmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            user.IsEmailConfirmed = confirmed;
            return Task.FromResult(confirmed);
        }

        public Task<User> FindByEmailAsync(string email)
        {
            var res = _userService.GetUserByEmailAsync(email);
            return res;
        }
    }
}
