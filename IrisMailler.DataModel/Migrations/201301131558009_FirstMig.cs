namespace IrisMailler.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Campaigns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        FolderRoot_Id = c.Int(nullable: false),
                        FolderNode_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FolderRoots", t => t.FolderRoot_Id, cascadeDelete: true)
                .ForeignKey("dbo.FolderNodes", t => t.FolderNode_Id, cascadeDelete: true)
                .Index(t => t.FolderRoot_Id)
                .Index(t => t.FolderNode_Id);
            
            CreateTable(
                "dbo.FolderRoots",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FolderNodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Path = c.String(),
                        ParentNode_Id = c.Int(),
                        Root_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FolderNodes", t => t.ParentNode_Id)
                .ForeignKey("dbo.FolderRoots", t => t.Root_Id)
                .Index(t => t.ParentNode_Id)
                .Index(t => t.Root_Id);
            
            CreateTable(
                "dbo.DataSources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConnectionString = c.String(),
                        TableName = c.String(),
                        DataSchemaXml = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Communications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        State = c.Int(nullable: false),
                        SpeedLimit = c.Int(nullable: false),
                        ScheduleString = c.String(),
                        FolderRoot_Id = c.Int(nullable: false),
                        FolderNode_Id = c.Int(nullable: false),
                        Campaign_Id = c.Int(),
                        DataSource_Id = c.Int(),
                        Message_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FolderRoots", t => t.FolderRoot_Id, cascadeDelete: true)
                .ForeignKey("dbo.FolderNodes", t => t.FolderNode_Id, cascadeDelete: true)
                .ForeignKey("dbo.Campaigns", t => t.Campaign_Id)
                .ForeignKey("dbo.DataSources", t => t.DataSource_Id)
                .ForeignKey("dbo.Messages", t => t.Message_Id)
                .Index(t => t.FolderRoot_Id)
                .Index(t => t.FolderNode_Id)
                .Index(t => t.Campaign_Id)
                .Index(t => t.DataSource_Id)
                .Index(t => t.Message_Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        Subject = c.String(),
                        MailFrom = c.String(),
                        NameFrom = c.String(),
                        ReplyTo = c.String(),
                        HtmlBody = c.String(storeType: "ntext"),
                        TextBody = c.String(storeType: "ntext"),
                        FolderRoot_Id = c.Int(),
                        FolderNode_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FolderRoots", t => t.FolderRoot_Id)
                .ForeignKey("dbo.FolderNodes", t => t.FolderNode_Id)
                .Index(t => t.FolderRoot_Id)
                .Index(t => t.FolderNode_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Messages", new[] { "FolderNode_Id" });
            DropIndex("dbo.Messages", new[] { "FolderRoot_Id" });
            DropIndex("dbo.Communications", new[] { "Message_Id" });
            DropIndex("dbo.Communications", new[] { "DataSource_Id" });
            DropIndex("dbo.Communications", new[] { "Campaign_Id" });
            DropIndex("dbo.Communications", new[] { "FolderNode_Id" });
            DropIndex("dbo.Communications", new[] { "FolderRoot_Id" });
            DropIndex("dbo.FolderNodes", new[] { "Root_Id" });
            DropIndex("dbo.FolderNodes", new[] { "ParentNode_Id" });
            DropIndex("dbo.Campaigns", new[] { "FolderNode_Id" });
            DropIndex("dbo.Campaigns", new[] { "FolderRoot_Id" });
            DropForeignKey("dbo.Messages", "FolderNode_Id", "dbo.FolderNodes");
            DropForeignKey("dbo.Messages", "FolderRoot_Id", "dbo.FolderRoots");
            DropForeignKey("dbo.Communications", "Message_Id", "dbo.Messages");
            DropForeignKey("dbo.Communications", "DataSource_Id", "dbo.DataSources");
            DropForeignKey("dbo.Communications", "Campaign_Id", "dbo.Campaigns");
            DropForeignKey("dbo.Communications", "FolderNode_Id", "dbo.FolderNodes");
            DropForeignKey("dbo.Communications", "FolderRoot_Id", "dbo.FolderRoots");
            DropForeignKey("dbo.FolderNodes", "Root_Id", "dbo.FolderRoots");
            DropForeignKey("dbo.FolderNodes", "ParentNode_Id", "dbo.FolderNodes");
            DropForeignKey("dbo.Campaigns", "FolderNode_Id", "dbo.FolderNodes");
            DropForeignKey("dbo.Campaigns", "FolderRoot_Id", "dbo.FolderRoots");
            DropTable("dbo.Messages");
            DropTable("dbo.Communications");
            DropTable("dbo.DataSources");
            DropTable("dbo.FolderNodes");
            DropTable("dbo.FolderRoots");
            DropTable("dbo.Campaigns");
        }
    }
}
