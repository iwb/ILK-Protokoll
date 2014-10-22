using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Controllers
{
	public class SessionBaseController : BaseController
	{
		protected ActiveSession CreateNewSession(SessionType type)
		{
			var session = db.ActiveSessions.Add(new ActiveSession(type)
			{
				ManagerID = GetCurrentUserID()
			});

			// GGf. Themen übernehmen, die in die aktuelle Sitzung hinein verschoben werden.
			foreach (var t in db.Topics.Include(t => t.Creator).Where(t => t.TargetSessionTypeID == type.ID))
			{
				t.SessionTypeID = type.ID;
				t.TargetSessionTypeID = null;
			}
			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				throw new InvalidOperationException(ErrorMessageFromException(e), e);
			}

			var topics = db.Topics
				.Where(t => t.Decision == null && !t.IsReadOnly)
				.Where(t => t.SessionTypeID == session.SessionType.ID && t.TargetSessionTypeID == null)
				.Where(t => t.ResubmissionDate == null || t.ResubmissionDate < DateTime.Now)
				.ToList();

			foreach (var topic in topics)
			{
				MarkAsUnread(topic, skipCurrentUser: false);
				session.LockedTopics.Add(new TopicLock()
				{
					Topic = topic,
					Session = session,
					Action = TopicAction.None
				});
			}

			db.SaveChanges();
			Session["SessionID"] = session.ID;
			return session;
		}

		protected ActiveSession ResumeSession(int sessionID)
		{
			var session = db.ActiveSessions.Find(sessionID);

			if (session == null)
				throw new ArgumentException("Session-ID was not found.");

			Session["SessionID"] = session.ID;
			session.ManagerID = GetCurrentUserID();

			foreach (var tlock in session.LockedTopics)
			{
				MarkAsUnread(tlock.Topic, skipCurrentUser: false);
			}
			db.SaveChanges();
			return session;
		}
	}
}