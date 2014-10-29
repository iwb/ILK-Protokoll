using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class Rename_sensibly : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.SessionReport",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					AdditionalAttendees = c.String(),
					Notes = c.String(),
					Start = c.DateTime(nullable: false),
					End = c.DateTime(nullable: false),
					Manager_ID = c.Int(nullable: false),
					SessionType_ID = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.User", t => t.Manager_ID, cascadeDelete: true)
				.ForeignKey("dbo.SessionType", t => t.SessionType_ID, cascadeDelete: true)
				.Index(t => t.Manager_ID)
				.Index(t => t.SessionType_ID);

			AddColumn("dbo.User", "SessionReport_ID", c => c.Int());
			CreateIndex("dbo.User", "SessionReport_ID");
			AddForeignKey("dbo.User", "SessionReport_ID", "dbo.SessionReport", "ID");
			DropTable("dbo.Record");
		}

		public override void Down()
		{
			CreateTable(
				"dbo.Record",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					TopicID = c.Int(),
					Name = c.String(),
					Created = c.DateTime(nullable: false),
				})
				.PrimaryKey(t => t.ID);

			DropForeignKey("dbo.SessionReport", "SessionType_ID", "dbo.SessionType");
			DropForeignKey("dbo.User", "SessionReport_ID", "dbo.SessionReport");
			DropForeignKey("dbo.SessionReport", "Manager_ID", "dbo.User");
			DropIndex("dbo.SessionReport", new[] {"SessionType_ID"});
			DropIndex("dbo.SessionReport", new[] {"Manager_ID"});
			DropIndex("dbo.User", new[] {"SessionReport_ID"});
			DropColumn("dbo.User", "SessionReport_ID");
			DropTable("dbo.SessionReport");
		}
	}
}