using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class unreadState_marker : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.UnreadState",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					TopicID = c.Int(nullable: false),
					UserID = c.Int(),
					LatestChange = c.DateTime(nullable: false),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Topic", t => t.TopicID, cascadeDelete: true)
				.ForeignKey("dbo.User", t => t.UserID)
				.Index(t => t.TopicID)
				.Index(t => t.UserID);
		}

		public override void Down()
		{
			DropForeignKey("dbo.UnreadState", "UserID", "dbo.User");
			DropForeignKey("dbo.UnreadState", "TopicID", "dbo.Topic");
			DropIndex("dbo.UnreadState", new[] {"UserID"});
			DropIndex("dbo.UnreadState", new[] {"TopicID"});
			DropTable("dbo.UnreadState");
		}
	}
}