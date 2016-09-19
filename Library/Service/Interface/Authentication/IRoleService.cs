using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Authentication;

namespace Service.Interface.Authentication
{
    

    /// <summary>
    /// Interface for role service
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// get all role async
        /// </summary>
        /// <returns></returns>
        Task<List<Role>> GetRolesAsync();

        /// <summary>
        /// get all role
        /// </summary>
        /// <returns></returns>
        List<Role> GetRoles();

        /// <summary>
        /// get all role of 1 application async
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        Task<List<Role>> GetRolesOfApplicationAsync(string applicationId);

        /// <summary>
        ///  get all role of 1 application 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        List<Role> GetRolesOfApplication(string applicationId);

        /// <summary>
        ///  get role by id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Role> GetRoleByIdAsync(string id);

        /// <summary>
        ///  get all role of 1 application 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Role GetRoleById(string id);

        /// <summary>
        ///  get all role of 1 application async by name 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Role> GetRoleByNameAsync(string name);
        /// <summary>
        ///  get all role of 1 application by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Role GetRoleByName(string name);
        /// <summary>
        /// add 1 role to user async
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task AddRoleToUserAsync(string userId, string roleId, int organizationId);

        /// <summary>
        /// add 1 role to user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        void AddRoleToUser(string userId, string roleId);

        /// <summary>
        /// remove 1 role out of user async
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task RemoveRoleFromUserAsync(string userId, string roleId);

        /// <summary>
        /// remove 1 role out of user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        void RemoveRoleFromUser(string userId, string roleId);

        /// <summary>
        /// remove all role out of user async
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task RemoveAllRolesFromUserAsync(string userId);
        /// <summary>
        /// remove all role out of user
        /// </summary>
        /// <param name="userId"></param>
        void RemoveAllRolesFromUser(string userId);

        /// <summary>
        /// create role async
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task CreateAsync(Role role);

        /// <summary>
        /// create role
        /// </summary>
        /// <param name="role"></param>
        void Create(Role role);
        /// <summary>
        /// delete async
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task DeleteAsync(Role role);
        /// <summary>
        /// delete
        /// </summary>
        /// <param name="role"></param>
        void Delete(Role role);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task UpdateAsync(Role role);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        void Update(Role role);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Role>> GetRolesOfUserAsync(string userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<Role> GetRolesOfUser(string userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        bool CheckUserIsInRole(string userId, Role role);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<bool> CheckUserIsInRoleAsync(string userId, Role role);


        /// <summary>
        /// add 1 role to user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        void AddRoleToUserByRoleName(string userId, string roleName);

        /// <summary>
        /// add 1 role to user async
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task AddRoleToUserByRoleNameAsync(string userId, string roleName, int organizationId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        bool CheckUserIsInRoleByName(string userId, string roleName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<bool> CheckUserIsInRoleByNameAsync(string userId, string roleName);

    }
}
