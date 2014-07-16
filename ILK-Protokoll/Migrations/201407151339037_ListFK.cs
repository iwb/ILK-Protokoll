using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class ListFK : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.L_IlkMeeting", new[] {"SessionType_ID"});
			RenameColumn(table: "dbo.L_IlkMeeting", name: "SessionType_ID", newName: "SessionTypeID");
			AlterColumn("dbo.L_IlkMeeting", "SessionTypeID", c => c.Int(nullable: false));
			CreateIndex("dbo.L_IlkMeeting", "SessionTypeID");
		}

		public override void Down()
		{
			DropIndex("dbo.L_IlkMeeting", new[] {"SessionTypeID"});
			AlterColumn("dbo.L_IlkMeeting", "SessionTypeID", c => c.Int());
			RenameColumn(table: "dbo.L_IlkMeeting", name: "SessionTypeID", newName: "SessionType_ID");
			CreateIndex("dbo.L_IlkMeeting", "SessionType_ID");
		}
	}
}