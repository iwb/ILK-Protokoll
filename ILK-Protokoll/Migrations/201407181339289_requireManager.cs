using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class requireManager : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("dbo.ActiveSession", "Manager_ID", "dbo.User");
			DropForeignKey("dbo.ActiveSession", "SessionType_ID", "dbo.SessionType");
			DropIndex("dbo.ActiveSession", new[] {"Manager_ID"});
			DropIndex("dbo.ActiveSession", new[] {"SessionType_ID"});
			AlterColumn("dbo.ActiveSession", "Manager_ID", c => c.Int(nullable: false));
			AlterColumn("dbo.ActiveSession", "SessionType_ID", c => c.Int(nullable: false));
			CreateIndex("dbo.ActiveSession", "Manager_ID");
			CreateIndex("dbo.ActiveSession", "SessionType_ID");
			AddForeignKey("dbo.ActiveSession", "Manager_ID", "dbo.User", "ID", cascadeDelete: true);
			AddForeignKey("dbo.ActiveSession", "SessionType_ID", "dbo.SessionType", "ID", cascadeDelete: true);
		}

		public override void Down()
		{
			DropForeignKey("dbo.ActiveSession", "SessionType_ID", "dbo.SessionType");
			DropForeignKey("dbo.ActiveSession", "Manager_ID", "dbo.User");
			DropIndex("dbo.ActiveSession", new[] {"SessionType_ID"});
			DropIndex("dbo.ActiveSession", new[] {"Manager_ID"});
			AlterColumn("dbo.ActiveSession", "SessionType_ID", c => c.Int());
			AlterColumn("dbo.ActiveSession", "Manager_ID", c => c.Int());
			CreateIndex("dbo.ActiveSession", "SessionType_ID");
			CreateIndex("dbo.ActiveSession", "Manager_ID");
			AddForeignKey("dbo.ActiveSession", "SessionType_ID", "dbo.SessionType", "ID");
			AddForeignKey("dbo.ActiveSession", "Manager_ID", "dbo.User", "ID");
		}
	}
}