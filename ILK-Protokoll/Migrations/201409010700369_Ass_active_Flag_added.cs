using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class Ass_active_Flag_added : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Assignment", "Active", c => c.Boolean(nullable: false, defaultValue: true));
		}

		public override void Down()
		{
			DropColumn("dbo.Assignment", "Active");
		}
	}
}