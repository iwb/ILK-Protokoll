using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class listitem_guid_added : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.L_EmployeePresentation", "GUID", c => c.Guid());
			AddColumn("dbo.L_Conference", "GUID", c => c.Guid());
			AddColumn("dbo.L_Event", "GUID", c => c.Guid());
			AddColumn("dbo.L_Extension", "GUID", c => c.Guid());
			AddColumn("dbo.L_Holiday", "GUID", c => c.Guid());
			AddColumn("dbo.L_IlkDay", "GUID", c => c.Guid());
			AddColumn("dbo.L_IlkMeeting", "GUID", c => c.Guid());
			AddColumn("dbo.L_Opening", "GUID", c => c.Guid());
		}

		public override void Down()
		{
			DropColumn("dbo.L_Opening", "GUID");
			DropColumn("dbo.L_IlkMeeting", "GUID");
			DropColumn("dbo.L_IlkDay", "GUID");
			DropColumn("dbo.L_Holiday", "GUID");
			DropColumn("dbo.L_Extension", "GUID");
			DropColumn("dbo.L_Event", "GUID");
			DropColumn("dbo.L_Conference", "GUID");
			DropColumn("dbo.L_EmployeePresentation", "GUID");
		}
	}
}