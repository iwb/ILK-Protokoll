using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class Assignments_FKeys : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.Assignment", new[] {"Owner_ID"});
			RenameColumn(table: "dbo.Assignment", name: "Topic_ID", newName: "TopicID");
			RenameColumn(table: "dbo.Assignment", name: "Owner_ID", newName: "OwnerID");
			RenameIndex(table: "dbo.Assignment", name: "IX_Topic_ID", newName: "IX_TopicID");
			AlterColumn("dbo.Assignment", "OwnerID", c => c.Int(nullable: false));
			CreateIndex("dbo.Assignment", "OwnerID");
		}

		public override void Down()
		{
			DropIndex("dbo.Assignment", new[] {"OwnerID"});
			AlterColumn("dbo.Assignment", "OwnerID", c => c.Int());
			RenameIndex(table: "dbo.Assignment", name: "IX_TopicID", newName: "IX_Topic_ID");
			RenameColumn(table: "dbo.Assignment", name: "OwnerID", newName: "Owner_ID");
			RenameColumn(table: "dbo.Assignment", name: "TopicID", newName: "Topic_ID");
			CreateIndex("dbo.Assignment", "Owner_ID");
		}
	}
}