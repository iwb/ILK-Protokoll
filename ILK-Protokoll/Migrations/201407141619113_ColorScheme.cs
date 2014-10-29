using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class UserColorScheme : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.User", "ColorScheme", c => c.Int(nullable: false));
		}

		public override void Down()
		{
			DropColumn("dbo.User", "ColorScheme");
		}
	}
}