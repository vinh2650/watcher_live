using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Authentication;

namespace Service.Interface.Authentication
{
    /// <summary>
    /// Service for refresh token
    /// </summary>
    public interface IRefreshTokenService
    {

        #region async method
        /// <summary>
        /// Create refresh token async
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task CreateRefreshTokenAsync(RefreshToken token);
        /// <summary>
        /// Create refresh token async
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UpdateRefreshTokenAsync(RefreshToken token);

        /// <summary>
        /// Remove refresh token by object
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task RemoveRefreshTokenAsync(RefreshToken token);

        /// <summary>
        /// Remove refresh token by id
        /// </summary>
        /// <param name="refreshTokenId"></param>
        /// <returns></returns>
        Task RemoveRefreshTokenAsync(string refreshTokenId);

        /// <summary>
        /// Get refresh token by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<RefreshToken> GetRefreshTokenByIdAsync(string id);

        /// <summary>
        /// Get list refresh token of user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task<List<RefreshToken>> GetRefreshTokenByUsernameAndAppIdAsync(string username, string appId);

        #endregion

        #region async method
        /// <summary>
        /// Create refresh token 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        void CreateRefreshToken(RefreshToken token);
        /// <summary>
        /// Create refresh token async
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        void UpdateRefreshToken(RefreshToken token);
        /// <summary>
        /// Remove refresh token by object
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        void RemoveRefreshToken(RefreshToken token);

        /// <summary>
        /// Remove refresh token by id
        /// </summary>
        /// <param name="refreshTokenId"></param>
        /// <returns></returns>
        void RemoveRefreshToken(string refreshTokenId);

        /// <summary>
        /// Get refresh token by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RefreshToken GetRefreshTokenById(string id);

        /// <summary>
        /// Get list refresh token of user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        List<RefreshToken> GetRefreshTokenByUsernameAndAppId(string username, string appId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="appId"></param>
        void ChangeAllRefreshToken(string username, string appId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        Task ChangeAllRefreshTokenAsync(string username, string appId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        void ChangeAllRefreshToken(string username  );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task ChangeAllRefreshTokenAsync(string username);

        #endregion


    }
}
