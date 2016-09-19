using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// user  id
        /// </summary>
        [Index("IX_UserAndRole", 1, IsUnique = true)]
        public string UserId { get; set; }

        public User User { get; set; }


        /// <summary>
        /// role id
        /// </summary>
        [Index("IX_UserAndRole", 2, IsUnique = true)]
        public string RoleId { get; set; }

        public Role Role { get; set; }

    }
}
