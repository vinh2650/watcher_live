using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Business;

namespace Data.Mapping.Business
{
    class UserHistoryPathMapping: EntityTypeConfiguration<UserHistoryPath>
    {
        public UserHistoryPathMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Table & Column Mappings
            this.ToTable("UserHistoryPath");
        }
    }
}
