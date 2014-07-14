using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class TopicEditor : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.TopicHistory", "EditorID", c => c.Int(nullable: false, defaultValue: 1));
		}

		public override void Down()
		{
			DropColumn("dbo.TopicHistory", "EditorID");
		}
	}
}