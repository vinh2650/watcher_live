using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Logs;
using Core.Domain.Authentication;
using Service.CachingLayer;
using Service.Interface.Authentication;

namespace Service.Implement.Authentication
{
    public class UserService:BaseServiceWithLogging, IUserService
    {
        private readonly DbContext _context;
        private readonly DbSet<User> _userDbSet;
        private readonly DbSet<UserClaim> _userClaimDbSet;
        private readonly ICacheManager _cacheManager;
        
       

        private const string KeyForCachePerRoleOnClaimOfUser = "Ams.RoleOnClaim.OfUser.UserId.{0}";


        public UserService(DbContext context, ICacheManager cacheManager,INoisLoggingService noisLoggingService):base(noisLoggingService)
        {
            _context = context;
            _cacheManager = cacheManager;
            _userDbSet = _context.Set<User>();
            _userClaimDbSet = _context.Set<UserClaim>();
          }

        #region user
        public async Task CreateUserAsync(User user)
        {
            try
            {
                _userDbSet.Add(user);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public async Task CreateUsersAsync(List<User> users)
        {
            try
            {
                _userDbSet.AddRange(users);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public async Task RemoveUserAsync(User user)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE [User] WHERE Id = @p0", user.Id);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                // Create and execute raw SQL query.
                var query = "SELECT * FROM [User] WHERE Id = @p0";
                var result = await _userDbSet.SqlQuery(query, id).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetUserByIdAsync method from UserService: " + ex.Message, ex);
                throw ex;
            }          
        }

        public async Task<User> GetUserByUsernameAsync(string username, string appId)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return null;
                }

                // Create and execute raw SQL query.
                var query = "SELECT * FROM [User] WHERE Username = @p0 AND ApplicationId=@p1";
                var result = await _userDbSet.SqlQuery(query, username, appId).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetUserByUsernameAsync method from UserService: " + ex.Message, ex);
                throw ex;
            }
            
        }

        public Task<ClaimsIdentity> FindByUserIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }

                // Create and execute raw SQL query.
                var query = "SELECT * FROM UserClaim WHERE UserId = @p0";
                var listOfClaim = _context.Database.SqlQuery<UserClaim>(query, userId);
                var claims = new ClaimsIdentity();
                foreach (var userClaim in listOfClaim)
                {
                    claims.AddClaim(new Claim(userClaim.ClaimType, userClaim.ClaimValue));
                }
                return Task.FromResult(claims);
            }
            catch (Exception ex)
            {
                LogError("There is error while call FindByUserIdAsync method from UserService: " + ex.Message, ex);
                throw ex;
            }
        }

        public void CreateUser(User user)
        {
            try
            {
                _userDbSet.Add(user);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public void RemoveUser(string userId)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE [User] WHERE Id = @p0", userId);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
        }

        public User GetUserById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                //return _userDbSet.Include(p => p.UserSnappTags).FirstOrDefault(p => p.Id == id);
                // Create and execute raw SQL query.
                var query = "SELECT * FROM [User] u WHERE u.Id = @p0 ";
                var result = _userDbSet.SqlQuery(query, id).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetUserById method from UserService: " + ex.Message, ex);
                throw ex;
            }
        }

        public User GetUserByUsername(string username, string appId)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return null;
                }
                // Create and execute raw SQL query.
                var query = "SELECT * FROM [User] WHERE Username = @p0 AND ApplicationId=@p1";
                var result = _userDbSet.SqlQuery(query, username, appId).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetUserByUsername method from UserService: " + ex.Message, ex);
                throw ex;
            }          
        }

        public List<User> GetAllUser(string appId)
        {
            try
            {
                return _userDbSet.Where(p => p.ApplicationId == appId).ToList();
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetAllUser method from UserService: " + ex.Message, ex);
                throw ex;
            }
        }

        public Task<List<User>> GetAllUserAsync(string appId)
        {
            try
            {
                return _userDbSet.Where(p => p.ApplicationId == appId).ToListAsync();
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetAllUserAsync method from UserService: " + ex.Message, ex);
                throw ex;
            }
        }

        public ClaimsIdentity FindByUserId(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }

                // Create and execute raw SQL query.
                var query = "SELECT * FROM UserClaim WHERE UserId = @p0";
                var listOfClaim = _context.Database.SqlQuery<UserClaim>(query, userId);
                var claims = new ClaimsIdentity();
                foreach (var userClaim in listOfClaim)
                {
                    claims.AddClaim(new Claim(userClaim.ClaimType, userClaim.ClaimValue));
                }
                return claims;
            }
            catch (Exception ex)
            {
                LogError("There is error while call FindByUserId method from UserService: " + ex.Message, ex);
                throw ex;
            }    
        }
        #endregion

        #region claim

        public async Task<int> DeleteAllClaimsOfUserAsync(string userId)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE UserClaim WHERE UserId = @p0", userId);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
                return 0;
            }
        }

        public async Task<int> DeleteClaimOfUserAsync(User user, Claim userClaim)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE UserClaim WHERE UserId = @p0 AND ClaimType= @p1 AND ClaimValue= @p2", user.Id, userClaim.Type, userClaim.Value);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
                return 0;
            }
        }

        public async Task<int> InsertClaimForUserAsync(Claim userClaim, string userId)
        {
            var newUserClaim = new UserClaim()
            {
                ClaimType = userClaim.Type,
                ClaimValue = userClaim.Value,
                UserId = userId
            };
            try
            {
                _userClaimDbSet.Add(newUserClaim);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
                return 0;
            }

        }



        public int DeleteAllClaimsOfUser(string userId)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE UserClaim WHERE UserId = @p0", userId);
                _context.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
                return 0;
            }
        }

        public int DeleteClaimOfUser(User user, Claim userClaim)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE UserClaim WHERE UserId = @p0 AND ClaimType= @p1 AND ClaimValue= @p2", user.Id, userClaim.Type, userClaim.Value);
                _context.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
                return 0;
            }
        }

        public int InsertClaimForUser(Claim userClaim, string userId)
        {
            var newUserClaim = new UserClaim()
            {
                ClaimType = userClaim.Type,
                ClaimValue = userClaim.Value,
                UserId = userId
            };
            try
            {
                _userClaimDbSet.Add(newUserClaim);
                _context.SaveChanges();
                return 1;
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
                return 0;
            }
        }

        #endregion

        public User GetUserByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return null;
                }
                // Create and execute raw SQL query.
                var query = "SELECT * FROM [User] WHERE Email = @p0";
                var result = _userDbSet.SqlQuery(query, email).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetUserByEmail method from UserService: " + ex.Message, ex);
                throw ex;
            }
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return null;
                }
                // Create and execute raw SQL query.
                var query = "SELECT * FROM [User] WHERE Email = @p0";
                var result = _userDbSet.SqlQuery(query, email).FirstOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetUserByEmailAsync method from UserService: " + ex.Message, ex);
                throw ex;
            }
        }
        public User GetUserByUsername(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return null;
                }
                // Create and execute raw SQL query.
                var query = "SELECT * FROM [User] WHERE Username = @p0";
                var result = _userDbSet.SqlQuery(query, username).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetUserByUsername method from UserService: " + ex.Message, ex);
                throw ex;
            }        
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return null;
                }
                // Create and execute raw SQL query.
                var query = "SELECT * FROM [User] WHERE Username = @p0";
                var result = _userDbSet.SqlQuery(query, username).FirstOrDefault();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetUserByUsernameAsync method from UserService: " + ex.Message, ex);
                throw ex;
            }   
        }

        public Task<string> GetRoleByClaimAsync(string userId)
        {
            try
            {
                var keyCache = String.Format(KeyForCachePerRoleOnClaimOfUser, userId);
                var res = _cacheManager.Get(keyCache, () =>
                {
                    string roleName = _context.Database.SqlQuery<string>("SELECT [ClaimValue] FROM[dbo].[UserClaim] Where UserId = @p0 AND ClaimType = @p1", userId, ClaimTypes.Role).FirstOrDefault<string>();
                    return roleName;
                });
                return Task.FromResult(res);
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetRoleByClaimAsync method from UserService: " + ex.Message, ex);
                throw ex;
            }           
        }

        public string GetRoleByClaim(string userId)
        {
            try
            {
                var keyCache = String.Format(KeyForCachePerRoleOnClaimOfUser, userId);
                var res = _cacheManager.Get(keyCache, () =>
                {
                    string roleName = _context.Database.SqlQuery<string>("SELECT [ClaimValue] FROM[dbo].[UserClaim] Where UserId = @p0 AND ClaimType = @p1", userId, ClaimTypes.Role).FirstOrDefault<string>();
                    return roleName;
                });
                return res;
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetRoleByClaimAsync method from UserService: " + ex.Message, ex);
                throw ex;
            }           
        }

        public async Task SetRoleToClaimAsync(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName)) return;
            var currentRole = await GetRoleByClaimAsync(userId);
            if (!string.IsNullOrEmpty(currentRole))
            {
              
                try
                {
                     _context.Database.ExecuteSqlCommand("UPDATE [UserClaim] SET ClaimValue = @p0 WHERE UserId = @p1 AND ClaimType= @p2", roleName, userId,ClaimTypes.Role);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                    LogError("There is error while updating data: " + ex.Message, ex);
                }
                //update old claim
                _cacheManager.Remove(String.Format(KeyForCachePerRoleOnClaimOfUser, userId));
                return;

            }
            var newUserClaim = new UserClaim()
            {
                ClaimType = ClaimTypes.Role,
                ClaimValue = roleName,
                UserId = userId
            };
            try
            {
                _userClaimDbSet.Add(newUserClaim);
                await _context.SaveChangesAsync();
              
            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }

            _cacheManager.Remove(String.Format(KeyForCachePerRoleOnClaimOfUser, userId));

        }

        public void SetRoleToClaim(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName)) return;
            var currentRole = GetRoleByClaim(userId);
            if (!string.IsNullOrEmpty(currentRole))
            {

                try
                {
                    _context.Database.ExecuteSqlCommand("UPDATE [UserClaim] SET ClaimValue = @p0 WHERE UserId = @p1 AND ClaimType= @p2", roleName, userId, ClaimTypes.Role);

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                    LogError("There is error while updating data: " + ex.Message, ex);
                }
                //update old claim
                _cacheManager.Remove(String.Format(KeyForCachePerRoleOnClaimOfUser, userId));
                return;

            }
            var newUserClaim = new UserClaim()
            {
                ClaimType = ClaimTypes.Role,
                ClaimValue = roleName,
                UserId = userId
            };
            try
            {
                _userClaimDbSet.Add(newUserClaim);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
            }
            _cacheManager.Remove(String.Format(KeyForCachePerRoleOnClaimOfUser, userId));

        }
        
        public void UpdateUserInfo(string id, string firstName, string lastName, string phoneNumber, List<string> permissions, int organizationId)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("User id is not exists");
            }

            try
            {
                var query = "update [User] " +
                            " Set FirstName = @p1, LastName = @p2, PhoneNumber = @p3" +
                            " From [User] u " +
                            " Where u.Id = @p0";

                var queryFormat = " if not exists (select top 1 u.UserId from [UserRole] u where u.UserId = @p0 and u.[RoleId] = '{0}' and u.OrganizationId = '{1}')" +
                                  " insert into [UserRole] Values(@p0,'{0}','{1}')";

                //remove roles
                query += " delete [UserRole] where ',' + @p4 + ',' not like '%,' + RoleId + ',%' and UserId = @p0";

                //insert roles
                foreach (var permission in permissions)
                {
                    query += string.Format(queryFormat, permission, organizationId);
                }

                _context.Database.ExecuteSqlCommand(query, id, firstName, lastName, phoneNumber,
                    string.Join(",", permissions));
            }
            catch (Exception ex)
            {
                LogError("There is error while call UpdateUserInfo method from UserService: " + ex.Message, ex);
                throw ex;
            }
        }

        #region custom function

        public List<UserSimpleInfo> GetAllUser(string organizationId, bool? isEmailConfirmed)
        {
            try
            {
                if (string.IsNullOrEmpty(organizationId))
                    return new List<UserSimpleInfo>();

                var query =
                    "SELECT u.Id , u.UserName , u.Email , u.FirstName, u.LastName FROM [User] u WHERE u.OrganizationId=@p0";
                if (isEmailConfirmed.HasValue)
                    query += " AND u.IsEmailConfirmed=@p1";
                var result = _context.Database.SqlQuery<UserSimpleInfo>(query, organizationId, isEmailConfirmed).ToList();

                return result;
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetAllUser method from UserService: " + ex.Message, ex);
                throw ex;
            }  
        }

        public List<User> GetUsersForNotification(string userId = null, string keyword = null)
        {
            try
            {
                var query = "Select * From [User] u Where u.Active = 1 and u.Banned = 0";

                if (!string.IsNullOrEmpty(userId))
                {
                    query += " and u.Id <> '" + userId + "'";
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    query += " and (u.UserName like '%" + keyword + "%' or u.Email like '%" + keyword + "%' or u.FirstName like '%" + keyword + "%' or u.LastName like '%" + keyword + "%')";
                }

                return _context.Database.SqlQuery<User>(query).ToList();
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetUsersForNotification method from UserService: " + ex.Message, ex);
                throw ex;
            }           
        }

        public List<User> GetUsersByEmails(List<string> idsList)
        {
            try
            {
                if (idsList.Count == 0)
                {
                    return new List<User>();
                }

                var queryFormat = "Select * From [User] u Where u.Active = 1 and u.Banned = 0 and ',{0},' like '%,' + Email + ',%'";
                var query = string.Format(queryFormat, String.Join(",", idsList));

                return _context.Database.SqlQuery<User>(query).ToList();
            }
            catch (Exception ex)
            {
                LogError("There is error while call GetUsersByEmails method from UserService: " + ex.Message, ex);
                throw ex;
            } 
        } 
        #endregion

    }
}
