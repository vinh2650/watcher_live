namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserRoleFromUser : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.User", "TwoFactorEnabled");
            DropColumn("dbo.User", "OrganizationDestinationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "OrganizationDestinationId", c => c.String());
            AddColumn("dbo.User", "TwoFactorEnabled", c => c.Boolean(nullable: false));
        }
    }
}
