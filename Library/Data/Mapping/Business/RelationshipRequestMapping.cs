using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Business;

namespace Data.Mapping.Business
{
    class RelationshipRequestMapping: EntityTypeConfiguration<RelationshipRequest>
    {
        public RelationshipRequestMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Table & Column Mappings
            this.ToTable("RelationshipRequest");
        }
    }
}
