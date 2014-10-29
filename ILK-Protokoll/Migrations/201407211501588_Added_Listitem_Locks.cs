using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class Added_Listitem_Locks : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.L_EmployeePresentation", "LockTime", c => c.DateTime(nullable: false));
			AddColumn("dbo.L_EmployeePresentation", "LockSessionID", c => c.Int());
			AddColumn("dbo.L_Conference", "LockTime", c => c.DateTime(nullable: false));
			AddColumn("dbo.L_Conference", "LockSessionID", c => c.Int());
			AddColumn("dbo.L_Event", "LockTime", c => c.DateTime(nullable: false));
			AddColumn("dbo.L_Event", "LockSessionID", c => c.Int());
			AddColumn("dbo.L_Extension", "LockTime", c => c.DateTime(nullable: false));
			AddColumn("dbo.L_Extension", "LockSessionID", c => c.Int());
			AddColumn("dbo.L_IlkDay", "LockTime", c => c.DateTime(nullable: false));
			AddColumn("dbo.L_IlkDay", "LockSessionID", c => c.Int());
			AddColumn("dbo.L_IlkMeeting", "LockTime", c => c.DateTime(nullable: false));
			AddColumn("dbo.L_IlkMeeting", "LockSessionID", c => c.Int());
			AddColumn("dbo.L_Opening", "LockTime", c => c.DateTime(nullable: false));
			AddColumn("dbo.L_Opening", "LockSessionID", c => c.Int());
			AddColumn("dbo.L_ProfHoliday", "LockTime", c => c.DateTime(nullable: false));
			AddColumn("dbo.L_ProfHoliday", "LockSessionID", c => c.Int());
		}

		public override void Down()
		{
			DropColumn("dbo.L_ProfHoliday", "LockSessionID");
			DropColumn("dbo.L_ProfHoliday", "LockTime");
			DropColumn("dbo.L_Opening", "LockSessionID");
			DropColumn("dbo.L_Opening", "LockTime");
			DropColumn("dbo.L_IlkMeeting", "LockSessionID");
			DropColumn("dbo.L_IlkMeeting", "LockTime");
			DropColumn("dbo.L_IlkDay", "LockSessionID");
			DropColumn("dbo.L_IlkDay", "LockTime");
			DropColumn("dbo.L_Extension", "LockSessionID");
			DropColumn("dbo.L_Extension", "LockTime");
			DropColumn("dbo.L_Event", "LockSessionID");
			DropColumn("dbo.L_Event", "LockTime");
			DropColumn("dbo.L_Conference", "LockSessionID");
			DropColumn("dbo.L_Conference", "LockTime");
			DropColumn("dbo.L_EmployeePresentation", "LockSessionID");
			DropColumn("dbo.L_EmployeePresentation", "LockTime");
		}
	}
}