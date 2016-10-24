using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Authentication;
using Service.CachingLayer;
using Service.Interface.Authentication;

namespace Service.Implement.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly DbContext _context;
        private readonly DbSet<RefreshToken> _dbSet;

        private readonly ICacheManager _cacheManager;

        private const string KeyForCacheRefreshById = "AMS.RefreshToken.Id.{0}";
        private const string KeyForCacheRefreshByUsernameAndAppId = "AMS.RefreshToken.AppId.{0}.Username.{1}";

        private const string KeyForCacheRefreshByUsername = "AMS.RefreshToken.Username.{0}";
        public RefreshTokenService(DbContext context, ICacheManager cacheManager)
        {
            _context = context;
            _cacheManager = cacheManager;
            _dbSet = _context.Set<RefreshToken>();
        }

        public async Task CreateRefreshTokenAsync(RefreshToken token)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsernameAndAppId, token.Username, token.AppId));


                _dbSet.Add(token);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken token)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsernameAndAppId, token.Username, token.AppId));
                _cacheManager.Remove(String.Format(KeyForCacheRefreshById, token.Id));

                _context.Entry(token).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task RemoveRefreshTokenAsync(RefreshToken token)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsernameAndAppId, token.Username, token.AppId));
                _cacheManager.Remove(String.Format(KeyForCacheRefreshById, token.Id));

                _context.Database.ExecuteSqlCommand("DELETE RefreshToken WHERE Id = @p0", token.Id);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task RemoveRefreshTokenAsync(string refreshTokenId)
        {
            try
            {
                var refreshToken = GetRefreshTokenById(refreshTokenId);
                if (refreshToken != null)
                {
                    _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsernameAndAppId, refreshToken.Username, refreshToken.AppId));
                    _cacheManager.Remove(String.Format(KeyForCacheRefreshById, refreshTokenId));

                    _context.Database.ExecuteSqlCommand("DELETE RefreshToken WHERE Id = @p0", refreshTokenId);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RefreshToken> GetRefreshTokenByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var result = _cacheManager.Get(String.Format(KeyForCacheRefreshById, id),
                   () =>
                   {
                       var query = "SELECT * FROM RefreshToken WHERE Id = @p0";
                       var res = _dbSet.SqlQuery(query, id).FirstOrDefault();
                       return res;
                   });



                // Create and execute raw SQL query.
                //var query = "SELECT * FROM RefreshToken WHERE Id = @p0";
                //var result = await _dbSet.SqlQuery(query, id).SingleOrDefaultAsync();

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<RefreshToken>> GetRefreshTokenByUsernameAndAppIdAsync(string username, string appId)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return null;
                }


                var result = _cacheManager.Get(String.Format(KeyForCacheRefreshByUsernameAndAppId, username, appId),
                    () =>
                    {
                        var query = "SELECT * FROM RefreshToken WHERE Username = @p0 AND AppId= @p1";
                        var res = _context.Database.SqlQuery<RefreshToken>(query, username, appId).ToList();
                        return res;
                    });



                // Create and execute raw SQL query.
                //var query = "SELECT * FROM RefreshToken WHERE Username = @p0 AND AppId= @p1";
                //var result = _context.Database.SqlQuery<RefreshToken>(query, username, appId);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateRefreshToken(RefreshToken token)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsernameAndAppId, token.Username, token.AppId));
                _dbSet.Add(token);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateRefreshToken(RefreshToken token)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsernameAndAppId, token.Username, token.AppId));
                _cacheManager.Remove(String.Format(KeyForCacheRefreshById, token.Id));

                _context.Entry(token).State = EntityState.Modified;
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void RemoveRefreshToken(RefreshToken token)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsernameAndAppId, token.Username, token.AppId));
                _cacheManager.Remove(String.Format(KeyForCacheRefreshById, token.Id));
                _context.Database.ExecuteSqlCommand("DELETE RefreshToken WHERE Id = @p0", token.Id);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveRefreshToken(string refreshTokenId)
        {
            try
            {
                var refreshToken = GetRefreshTokenById(refreshTokenId);
                if (refreshToken != null)
                {
                    _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsernameAndAppId, refreshToken.Username, refreshToken.AppId));
                    _cacheManager.Remove(String.Format(KeyForCacheRefreshById, refreshTokenId));
                    _context.Database.ExecuteSqlCommand("DELETE RefreshToken WHERE Id = @p0", refreshTokenId);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RefreshToken GetRefreshTokenById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var result = _cacheManager.Get(String.Format(KeyForCacheRefreshById, id),
                   () =>
                   {
                       var query = "SELECT * FROM RefreshToken WHERE Id = @p0";
                       var res = _dbSet.SqlQuery(query, id).FirstOrDefault();
                       return res;
                   });
                // Create and execute raw SQL query.
                //var query = "SELECT * FROM RefreshToken WHERE Id = @p0";
                //var result = await _dbSet.SqlQuery(query, id).SingleOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RefreshToken> GetRefreshTokenByUsernameAndAppId(string username, string appId)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return null;
                }

                // Create and execute raw SQL query.
                var result = _cacheManager.Get(String.Format(KeyForCacheRefreshByUsernameAndAppId, username, appId),
                  () =>
                  {
                      var query = "SELECT * FROM RefreshToken WHERE Username = @p0 AND AppId= @p1";
                      var res = _context.Database.SqlQuery<RefreshToken>(query, username, appId).ToList();
                      return res;
                  });
                // Create and execute raw SQL query.
                //var query = "SELECT * FROM RefreshToken WHERE Username = @p0 AND AppId= @p1";
                //var result = _context.Database.SqlQuery<RefreshToken>(query, username, appId);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void ChangeAllRefreshToken(string username, string appId)
        {
            try
            {
                //update in DB
                _context.Database.ExecuteSqlCommand("UPDATE RefreshToken SET HasChangeClaim = 1 WHERE Username = @p0 AND AppId= @p1", username, appId);
                //clear cache
                var listRefreshToken = GetRefreshTokenByUsernameAndAppId(username, appId);
                foreach (var refreshToken in listRefreshToken)
                {
                    _cacheManager.Remove(String.Format(KeyForCacheRefreshById, refreshToken.Id));
                }
                _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsernameAndAppId, username, appId));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task ChangeAllRefreshTokenAsync(string username, string appId)
        {
            try
            {
                await _context.Database.ExecuteSqlCommandAsync("UPDATE RefreshToken SET HasChangeClaim = 1 WHERE Username = @p0 AND AppId= @p1", username, appId);
                //clear cache
                var listRefreshToken = await GetRefreshTokenByUsernameAndAppIdAsync(username, appId);
                foreach (var refreshToken in listRefreshToken)
                {
                    _cacheManager.Remove(String.Format(KeyForCacheRefreshById, refreshToken.Id));
                }

                _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsernameAndAppId, username, appId));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<RefreshToken> GetRefreshTokenByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            // Create and execute raw SQL query.
            var result = _cacheManager.Get(String.Format(KeyForCacheRefreshByUsername, username),
              () =>
              {
                  var query = "SELECT * FROM RefreshToken WHERE Username = @p0";
                  var res = _context.Database.SqlQuery<RefreshToken>(query, username).ToList();
                  return res;
              });



            // Create and execute raw SQL query.
            //var query = "SELECT * FROM RefreshToken WHERE Username = @p0 AND AppId= @p1";
            //var result = _context.Database.SqlQuery<RefreshToken>(query, username, appId);

            return result;

        }
        public void ChangeAllRefreshToken(string username)
        {
            try
            {
                //update in DB
                _context.Database.ExecuteSqlCommand("UPDATE RefreshToken SET HasChangeClaim = 1 WHERE Username = @p0", username);
                //clear cache
                var listRefreshToken = GetRefreshTokenByUsername(username);
                foreach (var refreshToken in listRefreshToken)
                {
                    _cacheManager.Remove(String.Format(KeyForCacheRefreshById, refreshToken.Id));
                }
                _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsername, username));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task ChangeAllRefreshTokenAsync(string username)
        {
            try
            {
                //update in DB
                await _context.Database.ExecuteSqlCommandAsync("UPDATE RefreshToken SET HasChangeClaim = 1 WHERE Username = @p0", username);
                //clear cache
                var listRefreshToken = GetRefreshTokenByUsername(username);
                foreach (var refreshToken in listRefreshToken)
                {
                    _cacheManager.Remove(String.Format(KeyForCacheRefreshById, refreshToken.Id));
                }
                _cacheManager.Remove(String.Format(KeyForCacheRefreshByUsername, username));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
