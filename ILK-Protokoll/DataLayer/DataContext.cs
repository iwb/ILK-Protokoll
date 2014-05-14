using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
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

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}
	}
}