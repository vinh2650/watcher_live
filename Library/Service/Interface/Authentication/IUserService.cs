using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Domain.Authentication;

namespace Service.Interface.Authentication
{
    /// <summary>
    /// Represent class for user service
    /// </summary>
    public interface IUserService
    {
        #region async method

        #region normal method
        /// <summary>
        /// Create user async
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task CreateUserAsync(User user);

        /// <summary>
        /// Create user async
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task CreateUsersAsync(List<User> users);

        /// <summary>
        /// Create user async
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task UpdateUserAsync(User user);

        /// <summary>
        /// Remove user by object
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task RemoveUserAsync(User user);

       
        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<User> GetUserByIdAsync(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<User> GetUserByUsernameAsync(string username, string appId);
        #endregion

        #region claim
        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ClaimsIdentity> FindByUserIdAsync(string userId);

        /// <summary>
        ///  Deletes all claims from a user given a userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> DeleteAllClaimsOfUserAsync(string userId);

        /// <summary>
        /// Deletes a claim from a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userClaim"></param>
        /// <returns></returns>
        Task<int> DeleteClaimOfUserAsync(User user, Claim userClaim);

        /// <summary>
        /// Inserts a new claim in UserClaims table
        /// </summary>
        /// <param name="userClaim"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> InsertClaimForUserAsync(Claim userClaim, string userId);
        #endregion

        #endregion

        #region
        #region normal method
        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        void CreateUser(User user);
        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        void UpdateUser(User user);

        /// <summary>
        /// Remove user by object
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        void RemoveUser(string userId);


        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        User GetUserById(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        User GetUserByUsername(string username,string appId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        List<User> GetAllUser(string appId);

        Task<List<User>> GetAllUserAsync(string appId);

            #endregion

        #region claim
        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ClaimsIdentity FindByUserId(string userId);

        
        /// <summary>
        ///  Deletes all claims from a user given a userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int DeleteAllClaimsOfUser(string userId);

        /// <summary>
        /// Deletes a claim from a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userClaim"></param>
        /// <returns></returns>
        int DeleteClaimOfUser(User user, Claim userClaim);

        /// <summary>
        /// Inserts a new claim in UserClaims table
        /// </summary>
        /// <param name="userClaim"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        int InsertClaimForUser(Claim userClaim, string userId);
        #endregion
        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        User GetUserByEmail(string email);


        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User> GetUserByEmailAsync(string email);
        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        User GetUserByUsername(string username);


        /// <summary>
        /// Get user by Id
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<User> GetUserByUsernameAsync(string username);

        #region role by claim

        /// <summary>
        /// Get role by claim async
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetRoleByClaimAsync(string userId);

        /// <summary>
        /// Get role by claim
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetRoleByClaim(string userId);


        /// <summary>
        /// set role to claim async
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task SetRoleToClaimAsync(string userId, string roleName);

        /// <summary>
        ///  set role to claim 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        void SetRoleToClaim(string userId, string roleName);

        #endregion

        #endregion

        #region custom function

        List<UserSimpleInfo> GetAllUser(string organizationId, bool? isEmailConfirmed);

        #endregion

        List<User> GetUsersForNotification(string userId = null, string keyword = null);
        List<User> GetUsersByEmails(List<string> idsList);
        
        void UpdateUserInfo(string id, string firstName, string lastName, string phoneNumber, List<string> permissions, int organizationId);
    }

    /// <summary>
    /// simple info of User
    /// </summary>
    public class UserSimpleInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// FirstName
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// LastName
        /// </summary>
        public string LastName { get; set; }
    }
}
