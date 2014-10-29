using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class AttachmentForEP : DbMigration
	{
		public override void Up()
		{
			RenameColumn(table: "dbo.Attachment", name: "EmployeePresentation_ID", newName: "EmployeePresentationID");
			RenameIndex(table: "dbo.Attachment", name: "IX_EmployeePresentation_ID", newName: "IX_EmployeePresentationID");
		}

		public override void Down()
		{
			RenameIndex(table: "dbo.Attachment", name: "IX_EmployeePresentationID", newName: "IX_EmployeePresentation_ID");
			RenameColumn(table: "dbo.Attachment", name: "EmployeePresentationID", newName: "EmployeePresentation_ID");
		}
	}
}