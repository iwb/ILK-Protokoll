using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class SessionTypeID : DbMigration
	{
		public override void Up()
		{
			DropIndex("dbo.L_IlkDay", new[] {"SessionType_ID"});
			RenameColumn(table: "dbo.L_IlkDay", name: "SessionType_ID", newName: "SessionTypeID");
			AlterColumn("dbo.L_IlkDay", "SessionTypeID", c => c.Int(nullable: false));
			CreateIndex("dbo.L_IlkDay", "SessionTypeID");
		}

		public override void Down()
		{
			DropIndex("dbo.L_IlkDay", new[] {"SessionTypeID"});
			AlterColumn("dbo.L_IlkDay", "SessionTypeID", c => c.Int());
			RenameColumn(table: "dbo.L_IlkDay", name: "SessionTypeID", newName: "SessionType_ID");
			CreateIndex("dbo.L_IlkDay", "SessionType_ID");
		}
	}
}