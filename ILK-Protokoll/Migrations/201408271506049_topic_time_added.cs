using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class topic_time_added : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Topic", "Time", c => c.String(nullable: false));
			AlterColumn("dbo.Comment", "Content", c => c.String(nullable: false));
		}

		public override void Down()
		{
			AlterColumn("dbo.Comment", "Content", c => c.String());
			DropColumn("dbo.Topic", "Time");
		}
	}
}