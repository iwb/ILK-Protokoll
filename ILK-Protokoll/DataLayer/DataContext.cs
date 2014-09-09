using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Areas.Session.Models.Lists;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;
using JetBrains.Annotations;

namespace ILK_Protokoll.DataLayer
{
	public class DataContext : DbContext
	{
		public DataContext()
			: base("DataContext")
		{
			//Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
		}

		public DbSet<ActiveSession> ActiveSessions { get; set; }
		public DbSet<Assignment> Assignments { get; set; }
		public DbSet<Attachment> Attachments { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Decision> Decisions { get; set; }
		public DbSet<Conference> LConferences { get; set; }
		public DbSet<EmployeePresentation> LEmployeePresentations { get; set; }
		public DbSet<Event> LEvents { get; set; }
		public DbSet<Extension> LExtensions { get; set; }
		public DbSet<IlkDay> LIlkDays { get; set; }
		public DbSet<IlkMeeting> LIlkMeetings { get; set; }
		public DbSet<Holiday> LHolidays { get; set; }
		public DbSet<Opening> LOpenings { get; set; }
		public DbSet<PushNotification> PushNotifications { get; set; }
		public DbSet<SessionReport> SessionReports { get; set; }
		public DbSet<SessionType> SessionTypes { get; set; }
		public DbSet<Topic> Topics { get; set; }
		public DbSet<TopicHistory> TopicHistory { get; set; }
		public DbSet<TopicLock> TopicLocks { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Vote> Votes { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}

		/// <summary>
		///    Gibt alle Benutzer zurück, mit dem aktuellen Benutzer als erstes Element und den restlichen Benutzern in
		///    alphabetischer Reihenfolge.
		/// </summary>
		/// <param name="currentUser">Der aktuelle Benutzer, dieser wird an den Anfang der Liste sortiert.</param>
		/// <returns></returns>
		public IEnumerable<User> GetUserOrdered([CanBeNull] User currentUser)
		{
			if (currentUser != null)
			{
				return
					currentUser.ToEnumerable()
						.Concat(Users
							.Where(u => u.ID != currentUser.ID && u.IsActive)
							.ToList()
							.OrderBy(u => u.ShortName, StringComparer.CurrentCultureIgnoreCase));
			}
			else
			{
				return Users
					.Where(u => u.IsActive)
					.ToList()
					.OrderBy(u => u.ShortName, StringComparer.CurrentCultureIgnoreCase);
			}
		}

		public IEnumerable<SessionType> GetActiveSessionTypes()
		{
			return SessionTypes.Where(st => st.Active).OrderBy(st => st.Name);
		}
	}
}