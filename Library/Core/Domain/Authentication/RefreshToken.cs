using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Authentication
{
    /// <summary>
    /// Represent Refresh token
    /// </summary>
    public class RefreshToken:BaseEntity
    {
        
        /// <summary>
        /// Represent Username
        /// </summary>
        [Required]
        public string Username { get; set; }
        
        /// <summary>
        /// Represent application Id
        /// </summary>
        [Required]
        public string AppId { get; set; }

        /// <summary>
        /// datetime issue this refresh token
        /// </summary>
        public DateTime IssuesDateTimeUtc { get; set; }

        /// <summary>
        /// datetime expired this refresh token
        /// </summary>
        public DateTime ExpiredDateTimeUtc { get; set; }

        /// <summary>
        /// Represent ticket has been signed
        /// </summary>
        public string ProtectedTicket { get; set; }

        /// <summary>
        /// Flag for knowing claim of user has been changed
        /// </summary>
        public bool HasChangeClaim { get; set; }
    }
}
