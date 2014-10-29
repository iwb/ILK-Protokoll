using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class Topiclock_req : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("dbo.TopicLock", "Session_ID", "dbo.ActiveSession");
			DropIndex("dbo.TopicLock", new[] {"Session_ID"});
			RenameColumn(table: "dbo.TopicLock", name: "ID", newName: "TopicID");
			RenameIndex(table: "dbo.TopicLock", name: "IX_ID", newName: "IX_TopicID");
			AlterColumn("dbo.TopicLock", "Session_ID", c => c.Int(nullable: false));
			CreateIndex("dbo.TopicLock", "Session_ID");
			AddForeignKey("dbo.TopicLock", "Session_ID", "dbo.ActiveSession", "ID", cascadeDelete: true);
		}

		public override void Down()
		{
			DropForeignKey("dbo.TopicLock", "Session_ID", "dbo.ActiveSession");
			DropIndex("dbo.TopicLock", new[] {"Session_ID"});
			AlterColumn("dbo.TopicLock", "Session_ID", c => c.Int());
			RenameIndex(table: "dbo.TopicLock", name: "IX_TopicID", newName: "IX_ID");
			RenameColumn(table: "dbo.TopicLock", name: "TopicID", newName: "ID");
			CreateIndex("dbo.TopicLock", "Session_ID");
			AddForeignKey("dbo.TopicLock", "Session_ID", "dbo.ActiveSession", "ID");
		}
	}
}