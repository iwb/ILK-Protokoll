using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class tags_added : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.TagTopic",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					TopicID = c.Int(nullable: false),
					TagID = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Tag", t => t.TagID, cascadeDelete: true)
				.ForeignKey("dbo.Topic", t => t.TopicID, cascadeDelete: true)
				.Index(t => t.TopicID)
				.Index(t => t.TagID);

			CreateTable(
				"dbo.Tag",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					Name = c.String(nullable: false),
					BGColor = c.Int(nullable: false),
					TxtColor = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.ID);
		}

		public override void Down()
		{
			DropForeignKey("dbo.TagTopic", "TopicID", "dbo.Topic");
			DropForeignKey("dbo.TagTopic", "TagID", "dbo.Tag");
			DropIndex("dbo.TagTopic", new[] {"TagID"});
			DropIndex("dbo.TagTopic", new[] {"TopicID"});
			DropTable("dbo.Tag");
			DropTable("dbo.TagTopic");
		}
	}
}