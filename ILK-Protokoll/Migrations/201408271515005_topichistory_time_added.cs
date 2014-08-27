namespace ILK_Protokoll.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class topichistory_time_added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TopicHistory", "Time", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TopicHistory", "Time");
        }
    }
}
