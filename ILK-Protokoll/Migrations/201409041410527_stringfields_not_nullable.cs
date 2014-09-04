using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class stringfields_not_nullable : DbMigration
	{
		public override void Up()
		{
			Sql(@"UPDATE [dbo].[L_Conference] SET [Funding] = '' WHERE [Funding] IS NULL");
			Sql(@"UPDATE [dbo].[L_Event] SET [Time] = '' WHERE [Time] IS NULL");
			Sql(@"UPDATE [dbo].[L_Event] SET [Place] = '' WHERE [Place] IS NULL");
			Sql(@"UPDATE [dbo].[L_IlkDay] SET [Place] = '' WHERE [Place] IS NULL");
			Sql(@"UPDATE [dbo].[L_IlkMeeting] SET [Comments] = '' WHERE [Comments] IS NULL");

			AlterColumn("dbo.L_Conference", "Funding", c => c.String(nullable: false, defaultValue: ""));
			AlterColumn("dbo.L_Event", "Time", c => c.String(nullable: false, defaultValue: ""));
			AlterColumn("dbo.L_Event", "Place", c => c.String(nullable: false, defaultValue: ""));
			AlterColumn("dbo.L_IlkDay", "Place", c => c.String(nullable: false, defaultValue: ""));
			AlterColumn("dbo.L_IlkMeeting", "Comments", c => c.String(nullable: false, defaultValue: ""));
		}

		public override void Down()
		{
			AlterColumn("dbo.L_IlkMeeting", "Comments", c => c.String());
			AlterColumn("dbo.L_IlkDay", "Place", c => c.String());
			AlterColumn("dbo.L_Event", "Place", c => c.String());
			AlterColumn("dbo.L_Event", "Time", c => c.String());
			AlterColumn("dbo.L_Conference", "Funding", c => c.String());
		}
	}
}