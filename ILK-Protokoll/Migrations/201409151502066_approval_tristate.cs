using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class approval_tristate : DbMigration
	{
		public override void Up()
		{
			RenameColumn("dbo.L_Extension", name: "Approved", newName: "Approval");
			AlterColumn("dbo.L_Extension", "Approval", c => c.Int(nullable: false));

			RenameColumn("dbo.L_Conference", name: "Approved", newName: "Approval");
			AlterColumn("dbo.L_Conference", "Approval", c => c.Int(nullable: false));
		}

		public override void Down()
		{
			AlterColumn("dbo.L_Extension", "Approval", c => c.Boolean(nullable: false));
			RenameColumn("dbo.L_Extension", name: "Approval", newName: "Approved");

			AlterColumn("dbo.L_Conference", "Approval", c => c.Boolean(nullable: false));
			RenameColumn("dbo.L_Conference", name: "Approval", newName: "Approved");
		}
	}
}