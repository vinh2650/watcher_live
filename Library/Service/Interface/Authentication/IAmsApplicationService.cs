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
        Task<AmsApplication> GetByIdAsync(string id);




        /// <summary>
        /// Create application async
        /// </summary>
        /// <param name="amsApplication"></param>
        /// <returns></returns>
        Task CreateApplicationAsync(AmsApplication amsApplication);

        /// <summary>
        /// Update application async
        /// </summary>
        /// <param name="amsApplication"></param>
        /// <returns></returns>
        Task UpdateApplicationAsync(AmsApplication amsApplication);

        /// <summary>
        /// Delete application async
        /// </summary>
        /// <param name="amsApplication"></param>
        /// <returns></returns>
        Task DeleteApplicationAsync(AmsApplication amsApplication);


        #endregion

        /// <summary>
        /// Get application by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AmsApplication GetById(string id);
        /// <summary>
        /// Create application
        /// </summary>
        /// <param name="amsApplication"></param>
        void CreateApplication(AmsApplication amsApplication);

        /// <summary>
        /// Update application
        /// </summary>
        /// <param name="amsApplication"></param>
        void UpdateApplication(AmsApplication amsApplication);

        /// <summary>
        /// Delete application
        /// </summary>
        /// <param name="amsApplication"></param>

        void DeleteApplication(AmsApplication amsApplication);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<AmsApplication> GetAllApplications();
    }
}
