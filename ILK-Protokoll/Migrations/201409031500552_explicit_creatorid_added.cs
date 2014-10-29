using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class explicit_creatorid_added : DbMigration
	{
		public override void Up()
		{
			RenameColumn(table: "dbo.Topic", name: "Creator_ID", newName: "CreatorID");
			RenameIndex(table: "dbo.Topic", name: "IX_Creator_ID", newName: "IX_CreatorID");
		}

		public override void Down()
		{
			RenameIndex(table: "dbo.Topic", name: "IX_CreatorID", newName: "IX_Creator_ID");
			RenameColumn(table: "dbo.Topic", name: "CreatorID", newName: "Creator_ID");
		}
	}
}