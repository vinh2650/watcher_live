namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AmsApplication",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Active = c.Boolean(nullable: false),
                        AppSecret = c.String(nullable: false),
                        EncryptSecret = c.String(nullable: false),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowOrigin = c.String(),
                        Type = c.Int(nullable: false),
                        CreatedDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RefreshToken",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Username = c.String(nullable: false),
                        AppId = c.String(nullable: false),
                        IssuesDateTimeUtc = c.DateTime(nullable: false),
                        ExpiredDateTimeUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(),
                        HasChangeClaim = c.Boolean(nullable: false),
                        CreatedDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        NormalizeName = c.String(),
                        CreatedDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 40),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => new { t.UserId, t.RoleId }, unique: true, name: "IX_UserAndRole");
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 40),
                        ApplicationId = c.String(maxLength: 40),
                        UserName = c.String(),
                        Email = c.String(),
                        DigitCodeHash = c.String(),
                        SaltDigitCodeHash = c.String(),
                        SecurityStamp = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        AvatarUrl = c.String(),
                        OriginalAvatarUrl = c.String(),
                        HasThumbnailAvatar = c.Boolean(nullable: false),
                        LockoutEnabled = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Banned = c.Boolean(nullable: false),
                        BirthDate = c.DateTime(),
                        LastSignIn = c.DateTime(),
                        LockoutEnd = c.DateTimeOffset(nullable: false, precision: 7),
                        AccessFailedCount = c.Int(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        IsEmailConfirmed = c.Boolean(nullable: false),
                        OrganizationDestinationId = c.String(),
                        ParentId = c.String(),
                        Phone = c.String(),
                        CreatedDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserHistoryPath",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 40),
                        HistoryPath = c.String(),
                        CreatedDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Relationship",
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
            
            CreateTable(
                "dbo.UserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            DropForeignKey("dbo.Relationship", "FromUserId", "dbo.User");
            DropForeignKey("dbo.UserHistoryPath", "UserId", "dbo.User");
            DropIndex("dbo.Relationship", new[] { "FromUserId" });
            DropIndex("dbo.UserHistoryPath", new[] { "UserId" });
            DropIndex("dbo.UserRole", "IX_UserAndRole");
            DropTable("dbo.UserClaim");
            DropTable("dbo.Relationship");
            DropTable("dbo.UserHistoryPath");
            DropTable("dbo.User");
            DropTable("dbo.UserRole");
            DropTable("dbo.Role");
            DropTable("dbo.RefreshToken");
            DropTable("dbo.AmsApplication");
        }
    }
}
