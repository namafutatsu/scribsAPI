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
            
            CreateTable(
                "dbo.SheetTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectKey = c.String(nullable: false, maxLength: 36),
                        Name = c.String(nullable: false, maxLength: 50),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ProjectKey)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Sheet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectKey = c.String(nullable: false, maxLength: 36),
                        Name = c.String(nullable: false, maxLength: 50),
                        SheetTemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SheetTemplate", t => t.SheetTemplateId, cascadeDelete: true)
                .Index(t => t.ProjectKey)
                .Index(t => t.SheetTemplateId);
            
            CreateTable(
                "dbo.SheetField",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        SheetId = c.Int(nullable: false),
                        SheetTemplateFieldId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sheet", t => t.SheetId, cascadeDelete: true)
                .ForeignKey("dbo.SheetTemplateField", t => t.SheetTemplateFieldId)
                .Index(t => t.SheetId)
                .Index(t => t.SheetTemplateFieldId);
            
            CreateTable(
                "dbo.SheetTemplateField",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Index = c.Int(nullable: false),
                        Label = c.String(nullable: false, maxLength: 255),
                        SheetTemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SheetTemplate", t => t.SheetTemplateId, cascadeDelete: true)
                .Index(t => t.SheetTemplateId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Access", "UserId", "dbo.User");
            DropForeignKey("dbo.SheetTemplate", "UserId", "dbo.User");
            DropForeignKey("dbo.Sheet", "SheetTemplateId", "dbo.SheetTemplate");
            DropForeignKey("dbo.SheetField", "SheetTemplateFieldId", "dbo.SheetTemplateField");
            DropForeignKey("dbo.SheetTemplateField", "SheetTemplateId", "dbo.SheetTemplate");
            DropForeignKey("dbo.SheetField", "SheetId", "dbo.Sheet");
            DropIndex("dbo.SheetTemplateField", new[] { "SheetTemplateId" });
            DropIndex("dbo.SheetField", new[] { "SheetTemplateFieldId" });
            DropIndex("dbo.SheetField", new[] { "SheetId" });
            DropIndex("dbo.Sheet", new[] { "SheetTemplateId" });
            DropIndex("dbo.Sheet", new[] { "ProjectKey" });
            DropIndex("dbo.SheetTemplate", new[] { "UserId" });
            DropIndex("dbo.SheetTemplate", new[] { "ProjectKey" });
            DropIndex("dbo.User", new[] { "Name" });
            DropIndex("dbo.Access", new[] { "UserId" });
            DropTable("dbo.SheetTemplateField");
            DropTable("dbo.SheetField");
            DropTable("dbo.Sheet");
            DropTable("dbo.SheetTemplate");
            DropTable("dbo.User");
            DropTable("dbo.Access");
        }
    }
}
