using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class comment_AuthorID_explicit : DbMigration
	{
		public override void Up()
		{
			RenameColumn(table: "dbo.Comment", name: "Author_ID", newName: "AuthorID");
			RenameIndex(table: "dbo.Comment", name: "IX_Author_ID", newName: "IX_AuthorID");
		}

		public override void Down()
		{
			RenameIndex(table: "dbo.Comment", name: "IX_AuthorID", newName: "IX_Author_ID");
			RenameColumn(table: "dbo.Comment", name: "AuthorID", newName: "Author_ID");
		}
	}
}