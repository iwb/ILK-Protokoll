using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.DataLayer
{
	public class DataContext : DbContext
	{
		public DataContext() : base("DataContext")
		{
		}

		public DbSet<SessionType> SessionTypes { get; set; }
		public DbSet<Session> Sessions { get; set; }
		public DbSet<Topic> Topics { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Comment> Comments { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}

		public Topic GetCurrentTopic(int id)
		{
			return Topics.Where(t => t.ID == id).OrderByDescending(t => t.ValidFrom).First();
		}

		public Topic GetTopicAt(int id, DateTime time)
		{
			return Topics.Where(t => t.ID == id).Where(t => t.ValidFrom < time).OrderByDescending(t => t.ValidFrom).First();
		}
	}
}