using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class InitialCreate : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.ActiveSession",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					AdditionalAttendees = c.String(),
					Notes = c.String(),
					Start = c.DateTime(nullable: false),
					Manager_ID = c.Int(),
					SessionType_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.User", t => t.Manager_ID)
				.ForeignKey("dbo.SessionType", t => t.SessionType_ID)
				.Index(t => t.Manager_ID)
				.Index(t => t.SessionType_ID);

			CreateTable(
				"dbo.Topic",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					OwnerID = c.Int(nullable: false),
					SessionTypeID = c.Int(nullable: false),
					TargetSessionTypeID = c.Int(),
					Title = c.String(nullable: false),
					Description = c.String(nullable: false),
					Proposal = c.String(nullable: false),
					Priority = c.Int(nullable: false),
					Created = c.DateTime(nullable: false),
					ValidFrom = c.DateTime(nullable: false),
					ActiveSession_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.User", t => t.OwnerID, cascadeDelete: true)
				.ForeignKey("dbo.SessionType", t => t.SessionTypeID, cascadeDelete: true)
				.ForeignKey("dbo.SessionType", t => t.TargetSessionTypeID)
				.ForeignKey("dbo.ActiveSession", t => t.ActiveSession_ID)
				.Index(t => t.OwnerID)
				.Index(t => t.SessionTypeID)
				.Index(t => t.TargetSessionTypeID)
				.Index(t => t.ActiveSession_ID);

			CreateTable(
				"dbo.Assignment",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					Type = c.Int(nullable: false),
					Title = c.String(nullable: false),
					Description = c.String(nullable: false),
					DueDate = c.DateTime(nullable: false),
					ReminderSent = c.Boolean(nullable: false),
					IsDone = c.Boolean(nullable: false),
					Owner_ID = c.Int(),
					Topic_ID = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.User", t => t.Owner_ID)
				.ForeignKey("dbo.Topic", t => t.Topic_ID, cascadeDelete: true)
				.Index(t => t.Owner_ID)
				.Index(t => t.Topic_ID);

			CreateTable(
				"dbo.User",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					Guid = c.Guid(nullable: false),
					ShortName = c.String(nullable: false),
					LongName = c.String(),
					EmailAddress = c.String(),
					IsActive = c.Boolean(nullable: false),
					ActiveSession_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.ActiveSession", t => t.ActiveSession_ID)
				.Index(t => t.Guid, unique: true, name: "guid_index")
				.Index(t => t.ActiveSession_ID);

			CreateTable(
				"dbo.SessionType",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					Name = c.String(),
					LastDate = c.DateTime(),
				})
				.PrimaryKey(t => t.ID);

			CreateTable(
				"dbo.Attachment",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					TopicID = c.Int(),
					Deleted = c.DateTime(),
					DisplayName = c.String(nullable: false),
					SafeName = c.String(nullable: false),
					Extension = c.String(nullable: false),
					Created = c.DateTime(nullable: false),
					FileSize = c.Int(nullable: false),
					Uploader_ID = c.Int(nullable: false),
					EmployeePresentation_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Topic", t => t.TopicID)
				.ForeignKey("dbo.User", t => t.Uploader_ID, cascadeDelete: true)
				.ForeignKey("dbo.L_EmployeePresentation", t => t.EmployeePresentation_ID)
				.Index(t => t.TopicID)
				.Index(t => t.Uploader_ID)
				.Index(t => t.EmployeePresentation_ID);

			CreateTable(
				"dbo.Comment",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					TopicID = c.Int(nullable: false),
					Created = c.DateTime(nullable: false),
					Content = c.String(),
					Author_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.User", t => t.Author_ID)
				.ForeignKey("dbo.Topic", t => t.TopicID, cascadeDelete: true)
				.Index(t => t.TopicID)
				.Index(t => t.Author_ID);

			CreateTable(
				"dbo.Decision",
				c => new
				{
					ID = c.Int(nullable: false),
					Name = c.String(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Topic", t => t.ID)
				.Index(t => t.ID);

			CreateTable(
				"dbo.Vote",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					Kind = c.Int(nullable: false),
					Topic_ID = c.Int(),
					Voter_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Topic", t => t.Topic_ID)
				.ForeignKey("dbo.User", t => t.Voter_ID)
				.Index(t => t.Topic_ID)
				.Index(t => t.Voter_ID);

			CreateTable(
				"dbo.L_Conference",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					StartDate = c.DateTime(nullable: false),
					EndDate = c.DateTime(nullable: false),
					Description = c.String(),
					Employee = c.String(),
					Funding = c.String(),
					Approved = c.Boolean(nullable: false),
					Created = c.DateTime(nullable: false),
					Ilk_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.User", t => t.Ilk_ID)
				.Index(t => t.Ilk_ID);

			CreateTable(
				"dbo.L_EmployeePresentation",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					Employee = c.String(),
					Prof = c.Int(nullable: false),
					LastPresentation = c.DateTime(nullable: false),
					Selected = c.Boolean(nullable: false),
					Created = c.DateTime(nullable: false),
					Ilk_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.User", t => t.Ilk_ID)
				.Index(t => t.Ilk_ID);

			CreateTable(
				"dbo.L_Event",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					StartDate = c.DateTime(nullable: false),
					EndDate = c.DateTime(nullable: false),
					Time = c.String(),
					Place = c.String(),
					Organizer = c.String(),
					Description = c.String(),
					Created = c.DateTime(nullable: false),
				})
				.PrimaryKey(t => t.ID);

			CreateTable(
				"dbo.L_Extension",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					Employee = c.String(),
					EndDate = c.DateTime(nullable: false),
					ExtensionNumber = c.Int(nullable: false),
					Comment = c.String(),
					Approved = c.Boolean(nullable: false),
					Created = c.DateTime(nullable: false),
				})
				.PrimaryKey(t => t.ID);

			CreateTable(
				"dbo.L_IlkDay",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					Start = c.DateTime(nullable: false),
					End = c.DateTime(nullable: false),
					Place = c.String(),
					Topics = c.String(),
					Participants = c.String(),
					Created = c.DateTime(nullable: false),
					Organizer_ID = c.Int(),
					SessionType_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.User", t => t.Organizer_ID)
				.ForeignKey("dbo.SessionType", t => t.SessionType_ID)
				.Index(t => t.Organizer_ID)
				.Index(t => t.SessionType_ID);

			CreateTable(
				"dbo.L_IlkMeeting",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					Start = c.DateTime(nullable: false),
					Place = c.String(),
					Comments = c.String(),
					Created = c.DateTime(nullable: false),
					Organizer_ID = c.Int(),
					SessionType_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.User", t => t.Organizer_ID)
				.ForeignKey("dbo.SessionType", t => t.SessionType_ID)
				.Index(t => t.Organizer_ID)
				.Index(t => t.SessionType_ID);

			CreateTable(
				"dbo.L_Opening",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					Project = c.String(),
					Start = c.DateTime(nullable: false),
					TG = c.String(),
					Prof = c.Int(nullable: false),
					Description = c.String(),
					Created = c.DateTime(nullable: false),
				})
				.PrimaryKey(t => t.ID);

			CreateTable(
				"dbo.L_ProfHoliday",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					Professor = c.Int(nullable: false),
					Occasion = c.String(),
					Start = c.DateTime(),
					End = c.DateTime(),
					Created = c.DateTime(nullable: false),
				})
				.PrimaryKey(t => t.ID);

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

			CreateTable(
				"dbo.TopicHistory",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					TopicID = c.Int(nullable: false),
					Title = c.String(),
					Description = c.String(),
					Proposal = c.String(),
					OwnerID = c.Int(nullable: false),
					SessionTypeID = c.Int(nullable: false),
					ValidFrom = c.DateTime(nullable: false),
					ValidUntil = c.DateTime(nullable: false),
				})
				.PrimaryKey(t => t.ID)
				.Index(t => t.TopicID);

			CreateTable(
				"dbo.SessionTypeUser",
				c => new
				{
					SessionType_ID = c.Int(nullable: false),
					User_ID = c.Int(nullable: false),
				})
				.PrimaryKey(t => new {t.SessionType_ID, t.User_ID})
				.ForeignKey("dbo.SessionType", t => t.SessionType_ID, cascadeDelete: true)
				.ForeignKey("dbo.User", t => t.User_ID, cascadeDelete: true)
				.Index(t => t.SessionType_ID)
				.Index(t => t.User_ID);
		}

		public override void Down()
		{
			DropForeignKey("dbo.L_IlkMeeting", "SessionType_ID", "dbo.SessionType");
			DropForeignKey("dbo.L_IlkMeeting", "Organizer_ID", "dbo.User");
			DropForeignKey("dbo.L_IlkDay", "SessionType_ID", "dbo.SessionType");
			DropForeignKey("dbo.L_IlkDay", "Organizer_ID", "dbo.User");
			DropForeignKey("dbo.L_EmployeePresentation", "Ilk_ID", "dbo.User");
			DropForeignKey("dbo.Attachment", "EmployeePresentation_ID", "dbo.L_EmployeePresentation");
			DropForeignKey("dbo.L_Conference", "Ilk_ID", "dbo.User");
			DropForeignKey("dbo.ActiveSession", "SessionType_ID", "dbo.SessionType");
			DropForeignKey("dbo.User", "ActiveSession_ID", "dbo.ActiveSession");
			DropForeignKey("dbo.ActiveSession", "Manager_ID", "dbo.User");
			DropForeignKey("dbo.Topic", "ActiveSession_ID", "dbo.ActiveSession");
			DropForeignKey("dbo.Vote", "Voter_ID", "dbo.User");
			DropForeignKey("dbo.Vote", "Topic_ID", "dbo.Topic");
			DropForeignKey("dbo.Topic", "TargetSessionTypeID", "dbo.SessionType");
			DropForeignKey("dbo.Topic", "SessionTypeID", "dbo.SessionType");
			DropForeignKey("dbo.Topic", "OwnerID", "dbo.User");
			DropForeignKey("dbo.Decision", "ID", "dbo.Topic");
			DropForeignKey("dbo.Comment", "TopicID", "dbo.Topic");
			DropForeignKey("dbo.Comment", "Author_ID", "dbo.User");
			DropForeignKey("dbo.Attachment", "Uploader_ID", "dbo.User");
			DropForeignKey("dbo.Attachment", "TopicID", "dbo.Topic");
			DropForeignKey("dbo.Assignment", "Topic_ID", "dbo.Topic");
			DropForeignKey("dbo.Assignment", "Owner_ID", "dbo.User");
			DropForeignKey("dbo.SessionTypeUser", "User_ID", "dbo.User");
			DropForeignKey("dbo.SessionTypeUser", "SessionType_ID", "dbo.SessionType");
			DropIndex("dbo.SessionTypeUser", new[] {"User_ID"});
			DropIndex("dbo.SessionTypeUser", new[] {"SessionType_ID"});
			DropIndex("dbo.TopicHistory", new[] {"TopicID"});
			DropIndex("dbo.L_IlkMeeting", new[] {"SessionType_ID"});
			DropIndex("dbo.L_IlkMeeting", new[] {"Organizer_ID"});
			DropIndex("dbo.L_IlkDay", new[] {"SessionType_ID"});
			DropIndex("dbo.L_IlkDay", new[] {"Organizer_ID"});
			DropIndex("dbo.L_EmployeePresentation", new[] {"Ilk_ID"});
			DropIndex("dbo.L_Conference", new[] {"Ilk_ID"});
			DropIndex("dbo.Vote", new[] {"Voter_ID"});
			DropIndex("dbo.Vote", new[] {"Topic_ID"});
			DropIndex("dbo.Decision", new[] {"ID"});
			DropIndex("dbo.Comment", new[] {"Author_ID"});
			DropIndex("dbo.Comment", new[] {"TopicID"});
			DropIndex("dbo.Attachment", new[] {"EmployeePresentation_ID"});
			DropIndex("dbo.Attachment", new[] {"Uploader_ID"});
			DropIndex("dbo.Attachment", new[] {"TopicID"});
			DropIndex("dbo.User", new[] {"ActiveSession_ID"});
			DropIndex("dbo.User", "guid_index");
			DropIndex("dbo.Assignment", new[] {"Topic_ID"});
			DropIndex("dbo.Assignment", new[] {"Owner_ID"});
			DropIndex("dbo.Topic", new[] {"ActiveSession_ID"});
			DropIndex("dbo.Topic", new[] {"TargetSessionTypeID"});
			DropIndex("dbo.Topic", new[] {"SessionTypeID"});
			DropIndex("dbo.Topic", new[] {"OwnerID"});
			DropIndex("dbo.ActiveSession", new[] {"SessionType_ID"});
			DropIndex("dbo.ActiveSession", new[] {"Manager_ID"});
			DropTable("dbo.SessionTypeUser");
			DropTable("dbo.TopicHistory");
			DropTable("dbo.Record");
			DropTable("dbo.L_ProfHoliday");
			DropTable("dbo.L_Opening");
			DropTable("dbo.L_IlkMeeting");
			DropTable("dbo.L_IlkDay");
			DropTable("dbo.L_Extension");
			DropTable("dbo.L_Event");
			DropTable("dbo.L_EmployeePresentation");
			DropTable("dbo.L_Conference");
			DropTable("dbo.Vote");
			DropTable("dbo.Decision");
			DropTable("dbo.Comment");
			DropTable("dbo.Attachment");
			DropTable("dbo.SessionType");
			DropTable("dbo.User");
			DropTable("dbo.Assignment");
			DropTable("dbo.Topic");
			DropTable("dbo.ActiveSession");
		}
	}
}