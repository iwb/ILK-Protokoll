using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class userids_added : DbMigration
	{
		public override void Up()
		{
			RenameColumn(table: "dbo.ActiveSession", name: "Manager_ID", newName: "ManagerID");
			RenameColumn(table: "dbo.Vote", name: "Topic_ID", newName: "TopicID");
			RenameColumn(table: "dbo.Attachment", name: "Uploader_ID", newName: "UploaderID");
			RenameColumn(table: "dbo.SessionReport", name: "Manager_ID", newName: "ManagerID");
			RenameColumn(table: "dbo.Vote", name: "Voter_ID", newName: "VoterID");
			RenameIndex(table: "dbo.ActiveSession", name: "IX_Manager_ID", newName: "IX_ManagerID");
			RenameIndex(table: "dbo.Attachment", name: "IX_Uploader_ID", newName: "IX_UploaderID");
			RenameIndex(table: "dbo.SessionReport", name: "IX_Manager_ID", newName: "IX_ManagerID");
			RenameIndex(table: "dbo.Vote", name: "IX_Voter_ID", newName: "IX_VoterID");
			RenameIndex(table: "dbo.Vote", name: "IX_Topic_ID", newName: "IX_TopicID");
		}

		public override void Down()
		{
			RenameIndex(table: "dbo.Vote", name: "IX_TopicID", newName: "IX_Topic_ID");
			RenameIndex(table: "dbo.Vote", name: "IX_VoterID", newName: "IX_Voter_ID");
			RenameIndex(table: "dbo.SessionReport", name: "IX_ManagerID", newName: "IX_Manager_ID");
			RenameIndex(table: "dbo.Attachment", name: "IX_UploaderID", newName: "IX_Uploader_ID");
			RenameIndex(table: "dbo.ActiveSession", name: "IX_ManagerID", newName: "IX_Manager_ID");
			RenameColumn(table: "dbo.Vote", name: "VoterID", newName: "Voter_ID");
			RenameColumn(table: "dbo.SessionReport", name: "ManagerID", newName: "Manager_ID");
			RenameColumn(table: "dbo.Attachment", name: "UploaderID", newName: "Uploader_ID");
			RenameColumn(table: "dbo.Vote", name: "TopicID", newName: "Topic_ID");
			RenameColumn(table: "dbo.ActiveSession", name: "ManagerID", newName: "Manager_ID");
		}
	}
}