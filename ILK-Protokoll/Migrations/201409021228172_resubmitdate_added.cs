using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class resubmitdate_added : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.TopicLock", "ResubmissionDate", c => c.DateTime());
			AddColumn("dbo.Topic", "ResubmissionDate", c => c.DateTime());
		}

		public override void Down()
		{
			DropColumn("dbo.Topic", "ResubmissionDate");
			DropColumn("dbo.TopicLock", "ResubmissionDate");
		}
	}
}