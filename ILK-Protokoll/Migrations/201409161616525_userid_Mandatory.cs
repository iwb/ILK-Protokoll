using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class userid_Mandatory : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("dbo.UnreadState", "UserID", "dbo.User");
			DropIndex("dbo.UnreadState", new[] {"UserID"});
			AlterColumn("dbo.UnreadState", "UserID", c => c.Int(nullable: false));
			CreateIndex("dbo.UnreadState", "UserID");
			AddForeignKey("dbo.UnreadState", "UserID", "dbo.User", "ID", cascadeDelete: false);
		}

		public override void Down()
		{
			DropForeignKey("dbo.UnreadState", "UserID", "dbo.User");
			DropIndex("dbo.UnreadState", new[] {"UserID"});
			AlterColumn("dbo.UnreadState", "UserID", c => c.Int());
			CreateIndex("dbo.UnreadState", "UserID");
			AddForeignKey("dbo.UnreadState", "UserID", "dbo.User", "ID");
		}
	}
}