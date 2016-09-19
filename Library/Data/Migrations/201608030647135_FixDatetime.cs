namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDatetime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OndemandSaRequest", "OriginalStartTimeUtcKpi", c => c.DateTime(nullable: false));
            AddColumn("dbo.OndemandSaRequest", "OriginalStartTimeUtcNetworkAlarm", c => c.DateTime(nullable: false));
            DropColumn("dbo.OndemandSaRequest", "OriginalStartTimeUtc");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OndemandSaRequest", "OriginalStartTimeUtc", c => c.DateTime(nullable: false));
            DropColumn("dbo.OndemandSaRequest", "OriginalStartTimeUtcNetworkAlarm");
            DropColumn("dbo.OndemandSaRequest", "OriginalStartTimeUtcKpi");
        }
    }
}
