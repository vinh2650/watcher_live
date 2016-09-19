using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Authentication;

namespace Data.Mapping.Authentication
{


    public class UserRoleMapping : EntityTypeConfiguration<UserRole>
    {
        public UserRoleMapping()
        {
            // Primary Key
            this.HasKey(t => new { t.UserId, t.RoleId });
            
            // Table & Column Mappings
            this.ToTable("UserRole");
        }
    }
}
