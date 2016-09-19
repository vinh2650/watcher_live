namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_RelationshipRequest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RelationshipRequest",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FromUserId = c.String(nullable: false, maxLength: 40),
                        ToUserId = c.String(),
                        Type = c.Int(nullable: false),
                        CreatedDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.FromUserId, cascadeDelete: true)
                .Index(t => t.FromUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RelationshipRequest", "FromUserId", "dbo.User");
            DropIndex("dbo.RelationshipRequest", new[] { "FromUserId" });
            DropTable("dbo.RelationshipRequest");
        }
    }
}
