using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class topichistory_targetsession_added : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.TopicHistory", "TargetSessionTypeID", c => c.Int());
		}

		public override void Down()
		{
			DropColumn("dbo.TopicHistory", "TargetSessionTypeID");
		}
	}
}