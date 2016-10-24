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
    public class AmsApplicationService : IAmsApplicationService
    {
        private readonly DbContext _context;
        private readonly DbSet<AmsApplication> _dbSet;
        private readonly ICacheManager _cacheManager;

        private const string KeyForCacheDms = "Dms.Application.Id.{0}";

        public AmsApplicationService(DbContext context, ICacheManager cacheManager)
        {
            _context = context;
            _cacheManager = cacheManager;
            _dbSet = _context.Set<AmsApplication>();

        }

        public async Task<AmsApplication> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var result = _cacheManager.Get(String.Format(KeyForCacheDms, id),
                    () =>
                    {
                        var query = "SELECT * FROM AmsApplication WHERE Id = @p0";
                        var res = _dbSet.SqlQuery(query, id).FirstOrDefault();
                        return res;
                    });

                // Create and execute raw SQL query.
                //var query = "SELECT * FROM DmsApplication WHERE Id = @p0";
                //var result = await _dbSet.SqlQuery(query, id).SingleOrDefaultAsync();

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task CreateApplicationAsync(AmsApplication amsApplication)
        {
            try
            {
                _dbSet.Add(amsApplication);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task UpdateApplicationAsync(AmsApplication amsApplication)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheDms, amsApplication.Id));

                _context.Entry(amsApplication).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteApplicationAsync(AmsApplication amsApplication)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheDms, amsApplication.Id));

                _context.Database.ExecuteSqlCommand("DELETE AmsApplication WHERE Id = @p0", amsApplication.Id);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AmsApplication GetById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }
                var result = _cacheManager.Get(String.Format(KeyForCacheDms, id),
                    () =>
                    {
                        var query = "SELECT * FROM AmsApplication WHERE Id = @p0";
                        var res = _dbSet.SqlQuery(query, id).FirstOrDefault();
                        return res;
                    });
                // Create and execute raw SQL query.
                //var query = "SELECT * FROM DmsApplication WHERE Id = @p0";
                //var result =  _dbSet.SqlQuery(query, id).SingleOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void CreateApplication(AmsApplication amsApplication)
        {
            try
            {
                _dbSet.Add(amsApplication);
                _context.SaveChanges();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateApplication(AmsApplication amsApplication)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheDms, amsApplication.Id));

                _context.Entry(amsApplication).State = EntityState.Modified;
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteApplication(AmsApplication amsApplication)
        {
            try
            {
                _cacheManager.Remove(String.Format(KeyForCacheDms, amsApplication.Id));

                _context.Database.ExecuteSqlCommand("DELETE AmsApplication WHERE Id = @p0", amsApplication.Id);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AmsApplication> GetAllApplications()
        {
            try
            {
                // Create and execute raw SQL query.
                var query = "SELECT * FROM AmsApplication";
                var result = _context.Database.SqlQuery<AmsApplication>(query);
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
