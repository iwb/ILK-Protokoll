using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class TopicCreator : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Topic", "Creator_ID", c => c.Int(nullable: false, defaultValue: 1));
			CreateIndex("dbo.Topic", "Creator_ID");
			AddForeignKey("dbo.Topic", "Creator_ID", "dbo.User", "ID", cascadeDelete: false);
		}

		public override void Down()
		{
			DropForeignKey("dbo.Topic", "Creator_ID", "dbo.User");
			DropIndex("dbo.Topic", new[] {"Creator_ID"});
			DropColumn("dbo.Topic", "Creator_ID");
		}
	}
}