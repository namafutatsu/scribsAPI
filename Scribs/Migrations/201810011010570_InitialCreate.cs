namespace Scribs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Access",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CTime = c.DateTime(nullable: false),
                        MTime = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Token = c.String(nullable: false, maxLength: 255),
                        Secret = c.String(nullable: false, maxLength: 255),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Mail = c.String(nullable: false, maxLength: 255),
                        Password = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Access", "UserId", "dbo.User");
            DropIndex("dbo.User", new[] { "Name" });
            DropIndex("dbo.Access", new[] { "UserId" });
            DropTable("dbo.User");
            DropTable("dbo.Access");
        }
    }
}
