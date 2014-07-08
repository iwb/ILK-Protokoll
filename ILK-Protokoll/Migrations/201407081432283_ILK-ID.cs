using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class ILKID : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.L_Conference", new[] {"Ilk_ID"});
			DropIndex("dbo.L_EmployeePresentation", new[] {"Ilk_ID"});
			DropIndex("dbo.L_IlkDay", new[] {"Organizer_ID"});
			DropIndex("dbo.L_IlkMeeting", new[] {"Organizer_ID"});
			RenameColumn(table: "dbo.L_Conference", name: "Ilk_ID", newName: "IlkID");
			RenameColumn(table: "dbo.L_EmployeePresentation", name: "Ilk_ID", newName: "IlkID");
			RenameColumn(table: "dbo.L_IlkDay", name: "Organizer_ID", newName: "OrganizerID");
			RenameColumn(table: "dbo.L_IlkMeeting", name: "Organizer_ID", newName: "OrganizerID");
			AlterColumn("dbo.L_Conference", "IlkID", c => c.Int(nullable: false));
			AlterColumn("dbo.L_EmployeePresentation", "IlkID", c => c.Int(nullable: false));
			AlterColumn("dbo.L_IlkDay", "OrganizerID", c => c.Int(nullable: false));
			AlterColumn("dbo.L_IlkMeeting", "OrganizerID", c => c.Int(nullable: false));
			CreateIndex("dbo.L_Conference", "IlkID");
			CreateIndex("dbo.L_EmployeePresentation", "IlkID");
			CreateIndex("dbo.L_IlkDay", "OrganizerID");
			CreateIndex("dbo.L_IlkMeeting", "OrganizerID");
		}

		public override void Down()
		{
			DropIndex("dbo.L_IlkMeeting", new[] {"OrganizerID"});
			DropIndex("dbo.L_IlkDay", new[] {"OrganizerID"});
			DropIndex("dbo.L_EmployeePresentation", new[] {"IlkID"});
			DropIndex("dbo.L_Conference", new[] {"IlkID"});
			AlterColumn("dbo.L_IlkMeeting", "OrganizerID", c => c.Int());
			AlterColumn("dbo.L_IlkDay", "OrganizerID", c => c.Int());
			AlterColumn("dbo.L_EmployeePresentation", "IlkID", c => c.Int());
			AlterColumn("dbo.L_Conference", "IlkID", c => c.Int());
			RenameColumn(table: "dbo.L_IlkMeeting", name: "OrganizerID", newName: "Organizer_ID");
			RenameColumn(table: "dbo.L_IlkDay", name: "OrganizerID", newName: "Organizer_ID");
			RenameColumn(table: "dbo.L_EmployeePresentation", name: "IlkID", newName: "Ilk_ID");
			RenameColumn(table: "dbo.L_Conference", name: "IlkID", newName: "Ilk_ID");
			CreateIndex("dbo.L_IlkMeeting", "Organizer_ID");
			CreateIndex("dbo.L_IlkDay", "Organizer_ID");
			CreateIndex("dbo.L_EmployeePresentation", "Ilk_ID");
			CreateIndex("dbo.L_Conference", "Ilk_ID");
		}
	}
}