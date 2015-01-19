namespace ILK_Protokoll.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class lists_LastChanged : DbMigration
	{
		public override void Up()
		{
			RenameColumn("dbo.L_EmployeePresentation", "Created", "LastChanged");
			RenameColumn("dbo.L_Conference", "Created", "LastChanged");
			RenameColumn("dbo.L_Event", "Created", "LastChanged");
			RenameColumn("dbo.L_Extension", "Created", "LastChanged");
			RenameColumn("dbo.L_Holiday", "Created", "LastChanged");
			RenameColumn("dbo.L_IlkDay", "Created", "LastChanged");
			RenameColumn("dbo.L_IlkMeeting", "Created", "LastChanged");
			RenameColumn("dbo.L_Opening", "Created", "LastChanged");
		}

		public override void Down()
		{
			RenameColumn("dbo.L_EmployeePresentation", "LastChanged", "Created");
			RenameColumn("dbo.L_Conference", "LastChanged", "Created");
			RenameColumn("dbo.L_Event", "LastChanged", "Created");
			RenameColumn("dbo.L_Extension", "LastChanged", "Created");
			RenameColumn("dbo.L_Holiday", "LastChanged", "Created");
			RenameColumn("dbo.L_IlkDay", "LastChanged", "Created");
			RenameColumn("dbo.L_IlkMeeting", "LastChanged", "Created");
			RenameColumn("dbo.L_Opening", "LastChanged", "Created");
		}
	}
}