namespace ILK_Protokoll.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lists_rp_ip_added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.L_IndustryProject",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Partner = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IlkID = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        LastChanged = c.DateTime(nullable: false),
                        LockTime = c.DateTime(nullable: false),
                        LockSessionID = c.Int(),
                        GUID = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.IlkID, cascadeDelete: true)
                .Index(t => t.IlkID);
            
            CreateTable(
                "dbo.L_ResearchProposal",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Sponsor = c.String(nullable: false),
                        Akronym = c.String(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        IlkID = c.Int(nullable: false),
                        Employee = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        LastChanged = c.DateTime(nullable: false),
                        LockTime = c.DateTime(nullable: false),
                        LockSessionID = c.Int(),
                        GUID = c.Guid(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.IlkID, cascadeDelete: true)
                .Index(t => t.IlkID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.L_ResearchProposal", "IlkID", "dbo.User");
            DropForeignKey("dbo.L_IndustryProject", "IlkID", "dbo.User");
            DropIndex("dbo.L_ResearchProposal", new[] { "IlkID" });
            DropIndex("dbo.L_IndustryProject", new[] { "IlkID" });
            DropTable("dbo.L_ResearchProposal");
            DropTable("dbo.L_IndustryProject");
        }
    }
}
