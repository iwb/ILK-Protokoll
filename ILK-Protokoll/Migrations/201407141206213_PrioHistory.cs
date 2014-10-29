using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class PrioHistory : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.TopicHistory", "Priority", c => c.Int(nullable: false));
			AlterColumn("dbo.TopicHistory", "Title", c => c.String(nullable: false));
			AlterColumn("dbo.TopicHistory", "Description", c => c.String(nullable: false));
			AlterColumn("dbo.TopicHistory", "Proposal", c => c.String(nullable: false));
		}

		public override void Down()
		{
			AlterColumn("dbo.TopicHistory", "Proposal", c => c.String());
			AlterColumn("dbo.TopicHistory", "Description", c => c.String());
			AlterColumn("dbo.TopicHistory", "Title", c => c.String());
			DropColumn("dbo.TopicHistory", "Priority");
		}
	}
}