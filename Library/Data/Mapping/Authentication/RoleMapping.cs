using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Authentication;

namespace Data.Mapping.Authentication
{

    public class RoleMapping : EntityTypeConfiguration<Role>
    {

        public RoleMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            this.HasMany(t => t.UserRoles).WithRequired(t => t.Role).HasForeignKey(t => t.RoleId);
            // Table & Column Mappings
            this.ToTable("Role");

        }
    }
}
