using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class latest_Revision_optional : DbMigration
	{
		public override void Up()
		{
			DropTable("dbo.Attachment");

			DropForeignKey("dbo.Document", "LatestRevisionID", "dbo.Revision");
			DropIndex("dbo.Document", new[] {"LatestRevisionID"});
			AlterColumn("dbo.Document", "LatestRevisionID", c => c.Int());
			CreateIndex("dbo.Document", "LatestRevisionID");
			AddForeignKey("dbo.Document", "LatestRevisionID", "dbo.Revision", "ID");
		}

		public override void Down()
		{
			DropForeignKey("dbo.Document", "LatestRevisionID", "dbo.Revision");
			DropIndex("dbo.Document", new[] {"LatestRevisionID"});
			AlterColumn("dbo.Document", "LatestRevisionID", c => c.Int(nullable: false));
			CreateIndex("dbo.Document", "LatestRevisionID");
			AddForeignKey("dbo.Document", "LatestRevisionID", "dbo.Revision", "ID", cascadeDelete: false);

			CreateTable("dbo.Attachment",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					TopicID = c.Int(),
					EmployeePresentationID = c.Int(),
					Deleted = c.DateTime(),
					DisplayName = c.String(nullable: false),
					SafeName = c.String(nullable: false),
					Extension = c.String(nullable: false),
					UploaderID = c.Int(nullable: false),
					Created = c.DateTime(nullable: false),
					FileSize = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.ID);
		}
	}
}