using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class activeSession_sessiontypeid_added : DbMigration
	{
		public override void Up()
		{
			RenameColumn(table: "dbo.ActiveSession", name: "SessionType_ID", newName: "SessionTypeID");
			RenameIndex(table: "dbo.ActiveSession", name: "IX_SessionType_ID", newName: "IX_SessionTypeID");
		}

		public override void Down()
		{
			RenameIndex(table: "dbo.ActiveSession", name: "IX_SessionTypeID", newName: "IX_SessionType_ID");
			RenameColumn(table: "dbo.ActiveSession", name: "SessionTypeID", newName: "SessionType_ID");
		}
	}
}