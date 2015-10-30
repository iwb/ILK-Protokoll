namespace ILK_Protokoll.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rsprop_status_added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.L_ResearchProposal", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.L_ResearchProposal", "Status");
        }
    }
}
