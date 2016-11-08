using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Authentication;

namespace Service.Interface.Authentication
{
    public interface IAmsApplicationService
    {
        #region async

        /// <summary>
        /// Get application by id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Application> GetByIdAsync(string id);




        /// <summary>
        /// Create application async
        /// </summary>
        /// <param name="amsApplication"></param>
        /// <returns></returns>
        Task CreateApplicationAsync(Application amsApplication);

        /// <summary>
        /// Update application async
        /// </summary>
        /// <param name="amsApplication"></param>
        /// <returns></returns>
        Task UpdateApplicationAsync(Application amsApplication);

        /// <summary>
        /// Delete application async
        /// </summary>
        /// <param name="amsApplication"></param>
        /// <returns></returns>
        Task DeleteApplicationAsync(Application amsApplication);


        #endregion

        /// <summary>
        /// Get application by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Application GetById(string id);
        /// <summary>
        /// Create application
        /// </summary>
        /// <param name="amsApplication"></param>
        void CreateApplication(Application amsApplication);

        /// <summary>
        /// Update application
        /// </summary>
        /// <param name="amsApplication"></param>
        void UpdateApplication(Application amsApplication);

        /// <summary>
        /// Delete application
        /// </summary>
        /// <param name="amsApplication"></param>

        void DeleteApplication(Application amsApplication);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<Application> GetAllApplications();
    }
}
