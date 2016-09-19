using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Authentication;

namespace Data.Mapping.Authentication
{
    public class UserClaimMapping : EntityTypeConfiguration<UserClaim>
    {

        public UserClaimMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Table & Column Mappings
            this.ToTable("UserClaim");

        }
    }
}
