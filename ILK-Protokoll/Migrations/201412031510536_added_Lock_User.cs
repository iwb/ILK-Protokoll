namespace ILK_Protokoll.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_Lock_User : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Document", "LockUserID", c => c.Int());
            CreateIndex("dbo.Document", "LockUserID");
            AddForeignKey("dbo.Document", "LockUserID", "dbo.User", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Document", "LockUserID", "dbo.User");
            DropIndex("dbo.Document", new[] { "LockUserID" });
            DropColumn("dbo.Document", "LockUserID");
        }
    }
}
