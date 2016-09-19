using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Common.Logs;
using Core.Domain.Authentication;
using Service.CachingLayer;
using Service.Interface.Authentication;

namespace Service.Implement.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class RoleService:BaseServiceWithLogging, IRoleService
    {
        private readonly DbContext _context;
        private readonly DbSet<Role> _roleDbSet;
        private readonly DbSet<UserRole> _userRoleDbSet;
        private readonly ICacheManager _cacheManager;

        private const string KeyForCacheRoleById = "DMS.Role.ById.{0}";
        private const string KeyForCacheRoleByApplicationAndName = "DMS.Role.ByAppIdAndName.{0}";


        public RoleService(DbContext context, ICacheManager cacheManager, INoisLoggingService noisLoggingService):base(noisLoggingService)
        {
            _context = context;
            _roleDbSet = _context.Set<Role>();
            _userRoleDbSet = _context.Set<UserRole>(); 
            _cacheManager = cacheManager;
        }

        #region role

        public Task<List<Role>> GetRolesAsync()
        {
            throw new NotImplementedException();
        }

        public List<Role> GetRoles()
        {
            var query = "Select * from [Role]";
            return _roleDbSet.SqlQuery(query).ToList();
        }

        public Task<List<Role>> GetRolesOfApplicationAsync(string applicationId)
        {
            throw new NotImplementedException();
        }

        public List<Role> GetRolesOfApplication(string applicationId)
        {
            throw new NotImplementedException();
        }

        public Task<Role> GetRoleByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var result = _cacheManager.Get(String.Format(KeyForCacheRoleById, id),
                () =>
                {
                    var query = "SELECT * FROM [Role] WHERE Id = @p0";
                    var res = _roleDbSet.SqlQuery(query, id).FirstOrDefault();
                    return res;
                });
            return Task.FromResult(result);
        }

        public Role GetRoleById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var result = _cacheManager.Get(String.Format(KeyForCacheRoleById, id),
                () =>
                {
                    var query = "SELECT * FROM [Role] WHERE Id = @p0";
                    var res = _roleDbSet.SqlQuery(query, id).FirstOrDefault();
                    return res;
                });
            return result;
        }

        public Task<Role> GetRoleByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var result = _cacheManager.Get(String.Format(KeyForCacheRoleByApplicationAndName, name),
                () =>
                {
                    var query = "SELECT * FROM [Role] WHERE Name=@p0";
                    var res = _roleDbSet.SqlQuery(query, name).FirstOrDefault();
                    return res;
                });
            return Task.FromResult(result);
        }

        public Role GetRoleByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var result = _cacheManager.Get(String.Format(KeyForCacheRoleByApplicationAndName, name),
                () =>
                {
                    var query = "SELECT * FROM [Role] WHERE Name=@p0";
                    var res = _roleDbSet.SqlQuery(query, name).FirstOrDefault();
                    return res;
                });
            return result;
        }


        public async Task CreateAsync(Role role)
        {
            try
            {
                _roleDbSet.Add(role);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public void Create(Role role)
        {
            try
            {
                _roleDbSet.Add(role);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public async Task DeleteAsync(Role role)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheRoleById, role.Id));
              
                _context.Database.ExecuteSqlCommand("DELETE [Role] WHERE Id = @p0", role.Id);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public void Delete(Role role)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheRoleById, role.Id));
               
                _context.Database.ExecuteSqlCommand("DELETE [Role] WHERE Id = @p0", role.Id);
                 _context.SaveChanges();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public async Task UpdateAsync(Role role)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheRoleById, role.Id));
               
                _context.Entry(role).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public void Update(Role role)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheRoleById, role.Id));
               
                _context.Entry(role).State = EntityState.Modified;
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        #endregion

        #region userrole

        public async Task AddRoleToUserAsync(string userId, string roleId, int organizationId)
        {
            if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(roleId))
                return;
            var newUserRole = new UserRole()
            {
                UserId = userId,
                RoleId = roleId
            };
            try
            {
                _userRoleDbSet.Add(newUserRole);
                await _context.SaveChangesAsync();
             }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public void AddRoleToUser(string userId, string roleId)
        {
            if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(roleId))
                return;
            var newUserRole = new UserRole()
            {
                UserId = userId,
                RoleId = roleId
            };
            try
            {
                _userRoleDbSet.Add(newUserRole);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public async Task RemoveRoleFromUserAsync(string userId, string roleId)
        {
            if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(roleId))
                return;
            
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE [dbo].[UserRole] where UserId = @p0 and RoleId=@p1",
                    userId, roleId);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public void RemoveRoleFromUser(string userId, string roleId)
        {
            if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(roleId))
                return;

            try
            {
                _context.Database.ExecuteSqlCommand("DELETE [dbo].[UserRole] where UserId = @p0 and RoleId=@p1",
                    userId, roleId);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public async Task RemoveAllRolesFromUserAsync(string userId)
        {
            if (String.IsNullOrEmpty(userId))
                return;

            try
            {
                _context.Database.ExecuteSqlCommand("DELETE [dbo].[UserRole] where UserId = @p0",
                    userId);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public void RemoveAllRolesFromUser(string userId)
        {
            if (String.IsNullOrEmpty(userId))
                return;

            try
            {
                _context.Database.ExecuteSqlCommand("DELETE [dbo].[UserRole] where UserId = @p0",
                    userId);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }
        
        public async Task<List<Role>> GetRolesOfUserAsync(string userId)
        {
            try
            {
                var query = "SELECT * FROM [Role]  r JOIN [UserRole] ur on r.Id = ur.RoleId Where ur.UserId = @p0";
                var res = await _roleDbSet.SqlQuery(query, userId).ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetRolesOfUserAsync method from RoleService: " + ex.Message, ex);
                throw ex;
            }           
        }

        public List<Role> GetRolesOfUser(string userId)
        {
            try
            {
                var query = "SELECT * FROM [Role]  r JOIN [UserRole] ur on r.Id = ur.RoleId Where ur.UserId = @p0";
                var res = _roleDbSet.SqlQuery(query, userId).ToList();
                return res;
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetRolesOfUser method from RoleService: " + ex.Message, ex);
                throw ex;
            }         
        }

        public bool CheckUserIsInRole(string userId, Role role)
        {
            try
            {
                if (String.IsNullOrEmpty(userId) || role == null)
                    return false;
                var res = _context.Database.SqlQuery<bool>("SELECT CASE WHEN EXISTS (SELECT * FROM [UserRole] ur JOIN [Role] r on r.Id = ur.RoleId WHERE ur.UserId = '@p0' AND r.Id = '@p1') THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END",
                   userId, role.Id).FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                LogError("There is error while call CheckUserIsInRole method from RoleService: " + ex.Message, ex);
                throw ex;
            }        
        }

        public async Task<bool> CheckUserIsInRoleAsync(string userId, Role role)
        {
            try
            {
                if (String.IsNullOrEmpty(userId) || role == null)
                    return false;
                var res = await _context.Database.SqlQuery<bool>("SELECT CASE WHEN EXISTS (SELECT * FROM [UserRole] ur JOIN [Role] r on r.Id = ur.RoleId WHERE ur.UserId = '@p0' AND r.Id = '@p1') THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END",
                    userId, role.Id).FirstOrDefaultAsync();
                return res;
            }
            catch (Exception ex)
            {
                LogError("There is error while call CheckUserIsInRoleAsync method from RoleService: " + ex.Message, ex);
                throw ex;
            }
        }

        #endregion
        public void AddRoleToUserByRoleName(string userId, string roleName)
        {
            try
            {
                var role = GetRoleByRoleName(roleName);
                if (role != null)
                {
                    AddRoleToUser(userId, role.Id);
                }
            }
            catch (Exception ex)
            {
                LogError("There is error while call AddRoleToUserByRoleName method from RoleService: " + ex.Message, ex);
                throw ex;
            }            
        }

        public async Task AddRoleToUserByRoleNameAsync(string userId, string roleName, int organizationId)
        {
            try
            {
                var role = GetRoleByRoleName(roleName);
                if (role != null)
                {
                    await AddRoleToUserAsync(userId, role.Id, organizationId);
                }
            }
            catch (Exception ex)
            {
                LogError("There is error while call AddRoleToUserByRoleNameAsync method from RoleService: " + ex.Message, ex);
                throw ex;
            }  
        }
        private Role GetRoleByRoleName(string roleName)
        {
            try
            {
                if (string.IsNullOrEmpty(roleName))
                {
                    return null;
                }
                var query = "SELECT * FROM [Role] WHERE Name = @p0";
                var res = _roleDbSet.SqlQuery(query, roleName).FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetRoleByRoleName method from RoleService: " + ex.Message, ex);
                throw ex;
            } 
        }

        public bool CheckUserIsInRoleByName(string userId, string roleName)
        {
            try
            {
                var role = GetRoleByRoleName(roleName);
                if (role != null)
                {
                    return CheckUserIsInRole(userId, role);
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError("There is error while call CheckUserIsInRoleByName method from RoleService: " + ex.Message, ex);
                throw ex;
            }         
        }

        public async Task<bool> CheckUserIsInRoleByNameAsync(string userId, string roleName)
        {
            try
            {
                var role = GetRoleByRoleName(roleName);
                if (role != null)
                {
                    return await CheckUserIsInRoleAsync(userId, role);
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError("There is error while call CheckUserIsInRoleByNameAsync method from RoleService: " + ex.Message, ex);
                throw ex;
            }
        }
    }
}
