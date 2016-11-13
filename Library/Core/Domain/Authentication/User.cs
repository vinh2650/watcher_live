using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Domain.Business;
using Microsoft.AspNet.Identity;

namespace Core.Domain.Authentication
{
    /// <summary>
    /// Represent class for user
    /// </summary>
    public partial class User : BaseEntity, IUser
    {
        /// <summary>
        /// ApllicationId
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's digit code hash
        /// </summary>
        public string DigitCodeHash { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SaltDigitCodeHash { get; set; }

        /// <summary>
        /// User's security stamp
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OriginalAvatarUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasThumbnailAvatar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Banned { get; set; }

        /// <summary>
        /// Birthday
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastSignIn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset LockoutEnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// indicate user was created by other user
        /// </summary>
        public string ParentId { get; set; }

        ///// <summary>
        ///// eager load user role
        ///// </summary>
        //public ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Relation ship
        /// </summary>
        public ICollection<Relationship> Relationships { get; set; }

        /// <summary>
        /// Relation ship request
        /// </summary>
        public ICollection<RelationshipRequest> RelationshipRequests { get; set; }

        /// <summary>
        /// History Path
        /// </summary>
        public ICollection<UserHistoryPath> HistoryPaths { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public string GetDisplayName()
        {
            var name = GetFullName();
            return string.IsNullOrEmpty(name) ? Email : name;
        }

        public string GetFullName()
        {
            var name = string.IsNullOrEmpty(FirstName)
                ? LastName
                : (string.IsNullOrEmpty(LastName) ? FirstName : string.Format("{0} {1}", FirstName, LastName));
            return name;
        }

    }

}
