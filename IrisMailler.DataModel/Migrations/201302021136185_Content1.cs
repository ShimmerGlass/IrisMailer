namespace IrisMailler.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Content1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contents", "FolderNodeId", c => c.Int(nullable: false));
            AddForeignKey("dbo.Contents", "FolderNodeId", "dbo.FolderNodes", "Id", cascadeDelete: true);
            CreateIndex("dbo.Contents", "FolderNodeId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Contents", new[] { "FolderNodeId" });
            DropForeignKey("dbo.Contents", "FolderNodeId", "dbo.FolderNodes");
            DropColumn("dbo.Contents", "FolderNodeId");
        }
    }
}
