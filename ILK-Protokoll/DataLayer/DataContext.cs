using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;

namespace ILK_Protokoll.DataLayer
{
	public class DataContext : DbContext
	{
		public DataContext()
			: base("DataContext")
		{
		}

		public DbSet<Topic> Topics { get; set; }
		public DbSet<TopicHistory> TopicHistory { get; set; }
		public DbSet<SessionType> SessionTypes { get; set; }
		public DbSet<ActiveSession> Sessions { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Attachment> Attachments { get; set; }
		public DbSet<Vote> Votes { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

			modelBuilder
				.Entity<TopicHistory>()
				.Property(t => t.TopicID)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
		}

		/// <summary>
		///    Gibt alle Benutzer zurück, mit dem aktuellen Benutzer als erstes Element und den restlichen benutzern in
		///    alphabetischer Reihenfolge.
		/// </summary>
		/// <param name="currentUser"></param>
		/// <returns></returns>
		public IEnumerable<User> GetUserOrdered(User currentUser)
		{
			return
				currentUser.ToEnumerable()
					.Concat(Users
						.Where(u => u.ID != currentUser.ID && u.IsActive)
						.ToList()
						.OrderBy(u => u.ShortName, StringComparer.CurrentCultureIgnoreCase));
		}

		public Topic GetTopicAt(int id, DateTime time)
		{
			Topic t = Topics.Find(id);
			if (time > t.ValidFrom)
				return t;
			else if (time < t.Created)
				return null;
			else
			{
				TopicHistory histdata = TopicHistory.Single(x => x.ValidFrom < time && x.ValidUntil > time);
				t.IncorporateHistory(histdata);
				return t;
			}
		}
	}
}