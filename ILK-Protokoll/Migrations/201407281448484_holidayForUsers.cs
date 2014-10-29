using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class holidayForUsers : DbMigration
	{
		public override void Up()
		{
			RenameTable(name: "dbo.L_ProfHoliday", newName: "L_Holiday");
			AddColumn("dbo.L_Holiday", "Person_ID", c => c.Int(nullable: false, defaultValue: 1));
			CreateIndex("dbo.L_Holiday", "Person_ID");
			AddForeignKey("dbo.L_Holiday", "Person_ID", "dbo.User", "ID", cascadeDelete: true);
			DropColumn("dbo.L_Holiday", "Professor");
		}

		public override void Down()
		{
			AddColumn("dbo.L_Holiday", "Professor", c => c.Int(nullable: false));
			DropForeignKey("dbo.L_Holiday", "Person_ID", "dbo.User");
			DropIndex("dbo.L_Holiday", new[] {"Person_ID"});
			DropColumn("dbo.L_Holiday", "Person_ID");
			RenameTable(name: "dbo.L_Holiday", newName: "L_ProfHoliday");
		}
	}
}