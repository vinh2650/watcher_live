using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Authentication
{
    
    /// <summary>
    /// Represent application inside our solution
    /// </summary>
    public class Application : BaseEntity
    {
        #region general information


        /// <summary>
        /// Represent name of app
        /// </summary>
        [Required]
        public string Name { get; set; }


        /// <summary>
        /// Represent description of app
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Represent value for know this app is active or not
        /// </summary>
        public bool Active { get; set; }

        #endregion


        #region authentication 

        ///// <summary>
        ///// Represent AppId for use to authenticate an app and function behind
        ///// </summary>
        //[Required]
        //public string AppId { get; set; }


        /// <summary>
        /// Represent AppSecret for use to make pair (id/secret) of app
        /// </summary>
        [Required]
        public string AppSecret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string EncryptSecret { get; set; }

        /// <summary>
        /// Lifetime of refresh token
        /// </summary>
        public int RefreshTokenLifeTime { get; set; }


        /// <summary>
        /// Support for future with web develop
        /// </summary>
        public string AllowOrigin { get; set; }

        /// <summary>
        /// Application type
        /// </summary>
        public ApplicationType Type { get; set; }

        #endregion


      
    }

    /// <summary>
    /// Type of application
    /// </summary>
    public enum ApplicationType
    {
        /// <summary>
        /// Native application will connect to our system
        /// </summary>
        Native = 0,
        /// <summary>
        /// Javascript application will connect to our system
        /// </summary>
        Javascript = 1
    }
}
