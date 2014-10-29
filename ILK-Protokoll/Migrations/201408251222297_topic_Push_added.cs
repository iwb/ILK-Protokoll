using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class topic_Push_added : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.PushNotification",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					UserID = c.Int(nullable: false),
					TopicID = c.Int(nullable: false),
					Confirmed = c.Boolean(nullable: false),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Topic", t => t.TopicID, cascadeDelete: true)
				.ForeignKey("dbo.User", t => t.UserID, cascadeDelete: false)
				.Index(t => t.UserID)
				.Index(t => t.TopicID);
		}

		public override void Down()
		{
			DropForeignKey("dbo.PushNotification", "UserID", "dbo.User");
			DropForeignKey("dbo.PushNotification", "TopicID", "dbo.Topic");
			DropIndex("dbo.PushNotification", new[] {"TopicID"});
			DropIndex("dbo.PushNotification", new[] {"UserID"});
			DropTable("dbo.PushNotification");
		}
	}
}