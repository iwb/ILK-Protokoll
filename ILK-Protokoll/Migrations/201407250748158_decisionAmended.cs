using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class decisionAmended : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("dbo.TopicLock", "SessionReport_ID", "dbo.SessionReport");
			DropIndex("dbo.TopicLock", new[] {"SessionReport_ID"});
			AddColumn("dbo.Decision", "Type", c => c.Int(nullable: false));
			DropColumn("dbo.TopicLock", "SessionReport_ID");
		}

		public override void Down()
		{
			AddColumn("dbo.TopicLock", "SessionReport_ID", c => c.Int());
			DropColumn("dbo.Decision", "Type");
			CreateIndex("dbo.TopicLock", "SessionReport_ID");
			AddForeignKey("dbo.TopicLock", "SessionReport_ID", "dbo.SessionReport", "ID");
		}
	}
}