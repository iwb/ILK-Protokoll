using System.Data.Entity.Migrations;

namespace ILK_Protokoll.Migrations
{
	public partial class Added_TopicReadonly : DbMigration
	{
		public override void Up()
		{
			AddColumn("dbo.Topic", "IsReadOnly", c => c.Boolean(nullable: false));
		}

		public override void Down()
		{
			DropColumn("dbo.Topic", "IsReadOnly");
		}
	}
}