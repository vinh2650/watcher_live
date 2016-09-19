using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logs;
using Core.Domain.Authentication;
using Service.CachingLayer;
using Service.Interface.Authentication;

namespace Service.Implement.Authentication
{
    public class AmsApplicationService: BaseServiceWithLogging,IAmsApplicationService
    {
        private readonly DbContext _context;
        private readonly DbSet<AmsApplication> _dbSet;
        private readonly ICacheManager _cacheManager;

        private const string KeyForCacheDms = "Dms.Application.Id.{0}";

        public AmsApplicationService(DbContext context, ICacheManager cacheManager,INoisLoggingService noisLoggingService) : base(noisLoggingService)
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
                LogError("There is error while call GetByIdAsync method from AmsApplicationService: " + ex.Message, ex);
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
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
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
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
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
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
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
                LogError("There is error while call GetById method from AmsApplicationService: " + ex.Message, ex);
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
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
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
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
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
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Trace.TraceError("There is error while updating data: " + dex.InnerException);
                LogError("There is error while updating data: " + ex.Message, ex);
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
                LogError("There is error while call GetAllApplications method from AmsApplicationService: " + ex.Message, ex);
                throw ex;
            }
        }
    }
}
