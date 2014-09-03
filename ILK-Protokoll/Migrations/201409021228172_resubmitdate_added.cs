using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class resubmitdate_added : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Topic", "ResubmissionDate", c => c.DateTime());
		}

		public override void Down()
		{
			DropColumn("dbo.Topic", "ResubmissionDate");
		}
	}
}