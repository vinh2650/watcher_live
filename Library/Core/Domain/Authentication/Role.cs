using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Core.Domain.Authentication
{
      /// <summary>
    /// Represent class for role
    /// </summary>
    public partial class Role : BaseEntity,IRole
    {
        /// <summary>
        /// Name of role ,e.g SuperAdmin
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name in normalize of role, e.g Super Admin
        /// </summary>
        public string NormalizeName { get; set; }
        

        /// <summary>
        /// eager load user role
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; }
    }
    /// <summary>
    /// Role system name define 
    /// </summary>
    public static class RoleSystemName
    {
        public const string InsightusAdministrator = "InsightusAdministrator";
        public const string Administrator = "Administrator";
        public const string User = "User";
    }
}
