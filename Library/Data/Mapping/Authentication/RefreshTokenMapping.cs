using System.Data.Entity.ModelConfiguration;
using Core.Domain;
using Core.Domain.Authentication;

namespace Data.Mapping.Authentication
{
    public class RefreshTokenMapping : EntityTypeConfiguration<RefreshToken>
    {
        public RefreshTokenMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
           // Table & Column Mappings
            this.ToTable("RefreshToken");
        }
    }
}
