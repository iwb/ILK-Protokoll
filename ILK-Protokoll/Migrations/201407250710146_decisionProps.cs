using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class decisionProps : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.TopicLock", "SessionReport_ID", c => c.Int());
			AddColumn("dbo.Decision", "Text", c => c.String());
			AddColumn("dbo.Decision", "Report_ID", c => c.Int(nullable: false));
			CreateIndex("dbo.TopicLock", "SessionReport_ID");
			CreateIndex("dbo.Decision", "Report_ID");
			AddForeignKey("dbo.TopicLock", "SessionReport_ID", "dbo.SessionReport", "ID");
			AddForeignKey("dbo.Decision", "Report_ID", "dbo.SessionReport", "ID", cascadeDelete: true);
			DropColumn("dbo.Decision", "Name");
		}

		public override void Down()
		{
			AddColumn("dbo.Decision", "Name", c => c.String());
			DropForeignKey("dbo.Decision", "Report_ID", "dbo.SessionReport");
			DropForeignKey("dbo.TopicLock", "SessionReport_ID", "dbo.SessionReport");
			DropIndex("dbo.Decision", new[] {"Report_ID"});
			DropIndex("dbo.TopicLock", new[] {"SessionReport_ID"});
			DropColumn("dbo.Decision", "Report_ID");
			DropColumn("dbo.Decision", "Text");
			DropColumn("dbo.TopicLock", "SessionReport_ID");
		}
	}
}