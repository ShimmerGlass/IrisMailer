namespace IrisMailler.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class folderType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Campaigns", "FolderRoot_Id", "dbo.FolderRoots");
            DropForeignKey("dbo.Campaigns", "FolderNode_Id", "dbo.FolderNodes");
            DropForeignKey("dbo.FolderNodes", "Root_Id", "dbo.FolderRoots");
            DropForeignKey("dbo.Communications", "FolderRoot_Id", "dbo.FolderRoots");
            DropForeignKey("dbo.Communications", "FolderNode_Id", "dbo.FolderNodes");
            DropForeignKey("dbo.Messages", "FolderRoot_Id", "dbo.FolderRoots");
            DropIndex("dbo.Campaigns", new[] { "FolderRoot_Id" });
            DropIndex("dbo.Campaigns", new[] { "FolderNode_Id" });
            DropIndex("dbo.FolderNodes", new[] { "Root_Id" });
            DropIndex("dbo.Communications", new[] { "FolderRoot_Id" });
            DropIndex("dbo.Communications", new[] { "FolderNode_Id" });
            DropIndex("dbo.Messages", new[] { "FolderRoot_Id" });
            AddColumn("dbo.FolderNodes", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.DataSources", "FolderNode_Id", c => c.Int());
            AlterColumn("dbo.Campaigns", "FolderNode_Id", c => c.Int());
            AlterColumn("dbo.Communications", "FolderNode_Id", c => c.Int());
            AddForeignKey("dbo.Campaigns", "FolderNode_Id", "dbo.FolderNodes", "Id");
            AddForeignKey("dbo.DataSources", "FolderNode_Id", "dbo.FolderNodes", "Id");
            AddForeignKey("dbo.Communications", "FolderNode_Id", "dbo.FolderNodes", "Id");
            CreateIndex("dbo.Campaigns", "FolderNode_Id");
            CreateIndex("dbo.DataSources", "FolderNode_Id");
            CreateIndex("dbo.Communications", "FolderNode_Id");
            DropColumn("dbo.Campaigns", "FolderRoot_Id");
            DropColumn("dbo.FolderNodes", "Root_Id");
            DropColumn("dbo.Communications", "FolderRoot_Id");
            DropColumn("dbo.Messages", "FolderRoot_Id");
            DropTable("dbo.FolderRoots");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.FolderRoots",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Messages", "FolderRoot_Id", c => c.Int());
            AddColumn("dbo.Communications", "FolderRoot_Id", c => c.Int(nullable: false));
            AddColumn("dbo.FolderNodes", "Root_Id", c => c.Int());
            AddColumn("dbo.Campaigns", "FolderRoot_Id", c => c.Int(nullable: false));
            DropIndex("dbo.Communications", new[] { "FolderNode_Id" });
            DropIndex("dbo.DataSources", new[] { "FolderNode_Id" });
            DropIndex("dbo.Campaigns", new[] { "FolderNode_Id" });
            DropForeignKey("dbo.Communications", "FolderNode_Id", "dbo.FolderNodes");
            DropForeignKey("dbo.DataSources", "FolderNode_Id", "dbo.FolderNodes");
            DropForeignKey("dbo.Campaigns", "FolderNode_Id", "dbo.FolderNodes");
            AlterColumn("dbo.Communications", "FolderNode_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Campaigns", "FolderNode_Id", c => c.Int(nullable: false));
            DropColumn("dbo.DataSources", "FolderNode_Id");
            DropColumn("dbo.FolderNodes", "Type");
            CreateIndex("dbo.Messages", "FolderRoot_Id");
            CreateIndex("dbo.Communications", "FolderNode_Id");
            CreateIndex("dbo.Communications", "FolderRoot_Id");
            CreateIndex("dbo.FolderNodes", "Root_Id");
            CreateIndex("dbo.Campaigns", "FolderNode_Id");
            CreateIndex("dbo.Campaigns", "FolderRoot_Id");
            AddForeignKey("dbo.Messages", "FolderRoot_Id", "dbo.FolderRoots", "Id");
            AddForeignKey("dbo.Communications", "FolderNode_Id", "dbo.FolderNodes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Communications", "FolderRoot_Id", "dbo.FolderRoots", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FolderNodes", "Root_Id", "dbo.FolderRoots", "Id");
            AddForeignKey("dbo.Campaigns", "FolderNode_Id", "dbo.FolderNodes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Campaigns", "FolderRoot_Id", "dbo.FolderRoots", "Id", cascadeDelete: true);
        }
    }
}
