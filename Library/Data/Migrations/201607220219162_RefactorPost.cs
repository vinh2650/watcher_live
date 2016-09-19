namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactorPost : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Post", newName: "Notification");
            DropForeignKey("dbo.PostComment", "PostId", "dbo.Post");
            DropForeignKey("dbo.PostUserLinking", "PostId", "dbo.Post");
            DropIndex("dbo.PostComment", new[] { "PostId" });
            DropIndex("dbo.PostUserLinking", new[] { "PostId" });
            CreateTable(
                "dbo.NotificationComment",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        NotificationId = c.String(maxLength: 128),
                        OwnerId = c.String(),
                        Content = c.String(),
                        Subject = c.String(),
                        AttachmentList = c.String(),
                        CreatedDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notification", t => t.NotificationId)
                .Index(t => t.NotificationId);
            
            CreateTable(
                "dbo.NotificationUserLinking",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        NotificationId = c.String(maxLength: 128),
                        UserId = c.String(),
                        UserName = c.String(),
                        Email = c.String(),
                        UpdatedDateUtc = c.DateTime(),
                        CreatedDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notification", t => t.NotificationId)
                .Index(t => t.NotificationId);
            
            DropTable("dbo.PostComment");
            DropTable("dbo.PostUserLinking");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PostUserLinking",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PostId = c.String(maxLength: 128),
                        UserId = c.String(),
                        UserName = c.String(),
                        Email = c.String(),
                        UpdatedDateUtc = c.DateTime(),
                        CreatedDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PostComment",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PostId = c.String(maxLength: 128),
                        OwnerId = c.String(),
                        Content = c.String(),
                        Subject = c.String(),
                        AttachmentList = c.String(),
                        CreatedDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.NotificationUserLinking", "NotificationId", "dbo.Notification");
            DropForeignKey("dbo.NotificationComment", "NotificationId", "dbo.Notification");
            DropIndex("dbo.NotificationUserLinking", new[] { "NotificationId" });
            DropIndex("dbo.NotificationComment", new[] { "NotificationId" });
            DropTable("dbo.NotificationUserLinking");
            DropTable("dbo.NotificationComment");
            CreateIndex("dbo.PostUserLinking", "PostId");
            CreateIndex("dbo.PostComment", "PostId");
            AddForeignKey("dbo.PostUserLinking", "PostId", "dbo.Post", "Id");
            AddForeignKey("dbo.PostComment", "PostId", "dbo.Post", "Id");
            RenameTable(name: "dbo.Notification", newName: "Post");
        }
    }
}
