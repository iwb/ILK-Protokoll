using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class removeFutureDate : DbMigration
	{
		public override void Up()
		{
			AlterColumn("dbo.L_EmployeePresentation", "Employee", c => c.String(nullable: false));
			AlterColumn("dbo.L_Conference", "Description", c => c.String(nullable: false));
			AlterColumn("dbo.L_Conference", "Employee", c => c.String(nullable: false));
			AlterColumn("dbo.L_Event", "Organizer", c => c.String(nullable: false));
			AlterColumn("dbo.L_Event", "Description", c => c.String(nullable: false));
			AlterColumn("dbo.L_Extension", "Employee", c => c.String(nullable: false));
			AlterColumn("dbo.L_Extension", "Comment", c => c.String(nullable: false));
			AlterColumn("dbo.L_IlkDay", "Topics", c => c.String(nullable: false));
			AlterColumn("dbo.L_IlkDay", "Participants", c => c.String(nullable: false));
			AlterColumn("dbo.L_IlkMeeting", "Place", c => c.String(nullable: false));
			AlterColumn("dbo.L_Opening", "Project", c => c.String(nullable: false));
			AlterColumn("dbo.L_Opening", "TG", c => c.String(nullable: false));
			AlterColumn("dbo.L_Opening", "Description", c => c.String(nullable: false));
			AlterColumn("dbo.L_ProfHoliday", "Occasion", c => c.String(nullable: false));
		}

		public override void Down()
		{
			AlterColumn("dbo.L_ProfHoliday", "Occasion", c => c.String());
			AlterColumn("dbo.L_Opening", "Description", c => c.String());
			AlterColumn("dbo.L_Opening", "TG", c => c.String());
			AlterColumn("dbo.L_Opening", "Project", c => c.String());
			AlterColumn("dbo.L_IlkMeeting", "Place", c => c.String());
			AlterColumn("dbo.L_IlkDay", "Participants", c => c.String());
			AlterColumn("dbo.L_IlkDay", "Topics", c => c.String());
			AlterColumn("dbo.L_Extension", "Comment", c => c.String());
			AlterColumn("dbo.L_Extension", "Employee", c => c.String());
			AlterColumn("dbo.L_Event", "Description", c => c.String());
			AlterColumn("dbo.L_Event", "Organizer", c => c.String());
			AlterColumn("dbo.L_Conference", "Employee", c => c.String());
			AlterColumn("dbo.L_Conference", "Description", c => c.String());
			AlterColumn("dbo.L_EmployeePresentation", "Employee", c => c.String());
		}
	}
}