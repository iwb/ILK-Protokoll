using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class refactoruserSettingsAddMailFreq : DbMigration
	{
		public override void Up()
		{
			RenameColumn("dbo.User", name: "ColorScheme", newName: "Settings_ColorScheme");
			AddColumn("dbo.User", "Settings_ReportOccasions", c => c.Int(nullable: false));
		}

		public override void Down()
		{
			RenameColumn("dbo.User", name: "Settings_ColorScheme", newName: "ColorScheme");
			DropColumn("dbo.User", "Settings_ReportOccasions");
		}
	}
}