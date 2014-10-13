namespace ILK_Protokoll.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class document_revisions_added : DbMigration
    {
        public override void Up()
        {
	        return;
            DropForeignKey("dbo.Attachment", "EmployeePresentationID", "dbo.L_EmployeePresentation");
            DropForeignKey("dbo.Attachment", "TopicID", "dbo.Topic");
            DropForeignKey("dbo.Attachment", "UploaderID", "dbo.User");
            DropIndex("dbo.Attachment", new[] { "TopicID" });
            DropIndex("dbo.Attachment", new[] { "EmployeePresentationID" });
            DropIndex("dbo.Attachment", new[] { "UploaderID" });
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
                .ForeignKey("dbo.Revision", t => t.LatestRevisionID, cascadeDelete: true)
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
            
            DropTable("dbo.Attachment");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Attachment",
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
            
            DropForeignKey("dbo.Document", "TopicID", "dbo.Topic");
            DropForeignKey("dbo.Revision", "Document_ID", "dbo.Document");
            DropForeignKey("dbo.Document", "LatestRevisionID", "dbo.Revision");
            DropForeignKey("dbo.Revision", "UploaderID", "dbo.User");
            DropForeignKey("dbo.Revision", "ParentDocumentID", "dbo.Document");
            DropForeignKey("dbo.Document", "EmployeePresentationID", "dbo.L_EmployeePresentation");
            DropIndex("dbo.Revision", new[] { "Document_ID" });
            DropIndex("dbo.Revision", new[] { "UploaderID" });
            DropIndex("dbo.Revision", new[] { "ParentDocumentID" });
            DropIndex("dbo.Document", new[] { "LatestRevisionID" });
            DropIndex("dbo.Document", new[] { "EmployeePresentationID" });
            DropIndex("dbo.Document", new[] { "TopicID" });
            DropTable("dbo.Revision");
            DropTable("dbo.Document");
            CreateIndex("dbo.Attachment", "UploaderID");
            CreateIndex("dbo.Attachment", "EmployeePresentationID");
            CreateIndex("dbo.Attachment", "TopicID");
            AddForeignKey("dbo.Attachment", "UploaderID", "dbo.User", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Attachment", "TopicID", "dbo.Topic", "ID");
            AddForeignKey("dbo.Attachment", "EmployeePresentationID", "dbo.L_EmployeePresentation", "ID");
        }
    }
}
