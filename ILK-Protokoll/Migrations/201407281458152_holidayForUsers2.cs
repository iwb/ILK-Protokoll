using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class holidayForUsers2 : DbMigration
	{
		public override void Up()
		{
			RenameColumn(table: "dbo.L_Holiday", name: "Person_ID", newName: "PersonID");
			RenameIndex(table: "dbo.L_Holiday", name: "IX_Person_ID", newName: "IX_PersonID");
		}

		public override void Down()
		{
			RenameIndex(table: "dbo.L_Holiday", name: "IX_PersonID", newName: "IX_Person_ID");
			RenameColumn(table: "dbo.L_Holiday", name: "PersonID", newName: "Person_ID");
		}
	}
}