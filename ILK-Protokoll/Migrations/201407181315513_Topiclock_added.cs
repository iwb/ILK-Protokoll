namespace ILK_Protokoll.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Topiclock_added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TopicLock",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Action = c.Int(nullable: false),
                        Session_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ActiveSession", t => t.Session_ID)
                .ForeignKey("dbo.Topic", t => t.ID)
                .Index(t => t.ID)
                .Index(t => t.Session_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TopicLock", "ID", "dbo.Topic");
            DropForeignKey("dbo.TopicLock", "Session_ID", "dbo.ActiveSession");
            DropIndex("dbo.TopicLock", new[] { "Session_ID" });
            DropIndex("dbo.TopicLock", new[] { "ID" });
            DropTable("dbo.TopicLock");
        }
    }
}
