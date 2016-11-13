using System.Data.Entity.ModelConfiguration;
using Core.Domain.Authentication;

namespace Data.Mapping.Authentication
{
    public class UserMapping : EntityTypeConfiguration<User>
    {

        public UserMapping()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            // Table & Column Mappings
            this.ToTable("User");
            //this.HasMany(t => t.UserRoles).WithRequired(t => t.User).HasForeignKey(t => t.UserId);

            this.HasMany(t => t.Relationships).WithRequired(t => t.User).HasForeignKey(t => t.FromUserId);

            this.HasMany(t => t.RelationshipRequests).WithRequired(t => t.User).HasForeignKey(t => t.FromUserId);

            this.HasMany(t => t.HistoryPaths).WithRequired(t => t.User).HasForeignKey(t => t.UserId);

            //this.HasMany(t => t.Participants).WithRequired(t => t.User).HasForeignKey(t => t.UserId);

            Property(p => p.Id).HasMaxLength(40);
            Property(p => p.ApplicationId).HasMaxLength(40);
        }
    }
}
