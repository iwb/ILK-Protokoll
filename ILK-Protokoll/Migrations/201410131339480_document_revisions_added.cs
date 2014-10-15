using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class document_revisions_added : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("dbo.Attachment", "EmployeePresentationID", "dbo.L_EmployeePresentation");
			DropForeignKey("dbo.Attachment", "TopicID", "dbo.Topic");
			DropForeignKey("dbo.Attachment", "UploaderID", "dbo.User");
			DropIndex("dbo.Attachment", new[] {"TopicID"});
			DropIndex("dbo.Attachment", new[] {"EmployeePresentationID"});
			DropIndex("dbo.Attachment", new[] {"UploaderID"});

			CreateTable(
				"dbo.Document",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					GUID = c.Guid(nullable: false),
					TopicID = c.Int(),
					EmployeePresentationID = c.Int(),
					LockTime = c.DateTime(),
					Created = c.DateTime(nullable: false),
					Deleted = c.DateTime(),
					DisplayName = c.String(nullable: false),
					LatestRevisionID = c.Int(nullable: false),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.L_EmployeePresentation", t => t.EmployeePresentationID)
				.ForeignKey("dbo.Revision", t => t.LatestRevisionID, cascadeDelete: false)
				.ForeignKey("dbo.Topic", t => t.TopicID)
				.Index(t => t.TopicID)
				.Index(t => t.EmployeePresentationID)
				.Index(t => t.LatestRevisionID);

			CreateTable(
				"dbo.Revision",
				c => new
				{
					ID = c.Int(nullable: false, identity: true),
					GUID = c.Guid(nullable: false),
					ParentDocumentID = c.Int(nullable: false),
					Created = c.DateTime(nullable: false),
					UploaderID = c.Int(nullable: false),
					FileSize = c.Int(nullable: false),
					SafeName = c.String(nullable: false),
					Extension = c.String(nullable: false),
					Document_ID = c.Int(),
				})
				.PrimaryKey(t => t.ID)
				.ForeignKey("dbo.Document", t => t.ParentDocumentID, cascadeDelete: true)
				.ForeignKey("dbo.User", t => t.UploaderID, cascadeDelete: true)
				.ForeignKey("dbo.Document", t => t.Document_ID)
				.Index(t => t.ParentDocumentID)
				.Index(t => t.UploaderID)
				.Index(t => t.Document_ID);

			// Migrate Data
			Sql(@"INSERT INTO [dbo].[Document]
								  ([GUID]
								  ,[TopicID]
								  ,[EmployeePresentationID]
								  ,[Created]
								  ,[Deleted]
								  ,[DisplayName]
								  ,[LatestRevisionID])
						  SELECT NEWID()
						        ,TopicID
						        ,EmployeePresentationID
						        ,Created
						        ,Deleted
						        ,DisplayName
						        ,ID
					FROM [dbo].Attachment

					SET IDENTITY_INSERT [dbo].[Revision] ON

					INSERT INTO [dbo].[Revision]
								  ([ID]
								  ,[GUID]
								  ,[ParentDocumentID]
								  ,[Created]
								  ,[UploaderID]
								  ,[FileSize]
								  ,[SafeName]
								  ,[Extension]
								  ,[Document_ID])
						  SELECT ID
								  ,NEWID()
								  ,(SELECT ID FROM [dbo].[Document] AS doc WHERE doc.LatestRevisionID = attachment.ID)
								  ,Created
								  ,UploaderID
								  ,FileSize
								  ,SafeName
								  ,Extension
								  ,(SELECT ID FROM [dbo].[Document] AS doc WHERE doc.LatestRevisionID = attachment.ID)
					FROM [dbo].Attachment as attachment");

			Sql(@"SET IDENTITY_INSERT [dbo].[Revision] OFF");
		}

		public override void Down()
		{
			DropForeignKey("dbo.Document", "TopicID", "dbo.Topic");
			DropForeignKey("dbo.Document", "LatestRevisionID", "dbo.Revision");
			DropForeignKey("dbo.Document", "EmployeePresentationID", "dbo.L_EmployeePresentation");
			DropForeignKey("dbo.Revision", "Document_ID", "dbo.Document");
			DropForeignKey("dbo.Revision", "UploaderID", "dbo.User");
			DropForeignKey("dbo.Revision", "ParentDocumentID", "dbo.Document");

			Sql(@"TRUNCATE TABLE [dbo].[Attachment]
					SET IDENTITY_INSERT [dbo].[Attachment] ON

					INSERT INTO [dbo].[Attachment]
								  ([ID]
								  ,[TopicID]
								  ,[Deleted]
								  ,[DisplayName]
								  ,[SafeName]
								  ,[Extension]
								  ,[Created]
								  ,[FileSize]
								  ,[UploaderID]
								  ,[EmployeePresentationID])
						  SELECT        
								rev.[ID]
								  ,doc.[TopicID]
								  ,doc.[Deleted]
								  ,doc.[DisplayName]
								  ,rev.[SafeName]
								  ,rev.[Extension]
								  ,rev.[Created]
								  ,rev.[FileSize]
								  ,rev.[UploaderID]
								  ,doc.[EmployeePresentationID] FROM [dbo].Document as doc
						  JOIN [dbo].Revision as rev ON doc.LatestRevisionID = rev.ID

					SET IDENTITY_INSERT [dbo].[Attachment] OFF");

			DropIndex("dbo.Document", new[] {"LatestRevisionID"});
			DropIndex("dbo.Document", new[] {"EmployeePresentationID"});
			DropIndex("dbo.Document", new[] {"TopicID"});
			DropIndex("dbo.Revision", new[] {"Document_ID"});
			DropIndex("dbo.Revision", new[] {"UploaderID"});
			DropIndex("dbo.Revision", new[] {"ParentDocumentID"});

			DropTable("dbo.Revision");
			DropTable("dbo.Document");

			CreateIndex("dbo.Attachment", "UploaderID");
			CreateIndex("dbo.Attachment", "EmployeePresentationID");
			CreateIndex("dbo.Attachment", "TopicID");
			AddForeignKey("dbo.Attachment", "UploaderID", "dbo.User", "ID", cascadeDelete: false);
			AddForeignKey("dbo.Attachment", "TopicID", "dbo.Topic", "ID");
			AddForeignKey("dbo.Attachment", "EmployeePresentationID", "dbo.L_EmployeePresentation", "ID");
		}
	}
}