using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class Topiclock_added : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("dbo.Topic", "ActiveSession_ID", "dbo.ActiveSession");
			DropIndex("dbo.Topic", new[] {"ActiveSession_ID"});
			DropIndex("dbo.TopicHistory", new[] {"TopicID"});
			CreateTable(
				"dbo.TopicLock",
				c => new
				{
					ID = c.Int(nullable: false),
					Action = c.Int(nullable: false),
					Session_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.ActiveSession", t => t.Session_ID)
				.ForeignKey("dbo.Topic", t => t.ID)
				.Index(t => t.ID)
				.Index(t => t.Session_ID);

			DropColumn("dbo.Topic", "ActiveSession_ID");
		}

		public override void Down()
		{
			AddColumn("dbo.Topic", "ActiveSession_ID", c => c.Int());
			DropForeignKey("dbo.TopicLock", "ID", "dbo.Topic");
			DropForeignKey("dbo.TopicLock", "Session_ID", "dbo.ActiveSession");
			DropIndex("dbo.TopicLock", new[] {"Session_ID"});
			DropIndex("dbo.TopicLock", new[] {"ID"});
			DropTable("dbo.TopicLock");
			CreateIndex("dbo.TopicHistory", "TopicID");
			CreateIndex("dbo.Topic", "ActiveSession_ID");
			AddForeignKey("dbo.Topic", "ActiveSession_ID", "dbo.ActiveSession", "ID");
		}
	}
}