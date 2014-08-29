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
			var session = db.ActiveSessions.Add(new ActiveSession(type) {Manager = GetCurrentUser()});

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
				.ToList();

			foreach (var topic in topics)
			{
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

		protected ActiveSession ResumeSession(int SessionID)
		{
			var session = db.ActiveSessions.Find(SessionID);

			if (session == null)
				throw new ArgumentException("Session-ID was not found.");

			Session["SessionID"] = session.ID;
			session.Manager = GetCurrentUser();
			db.SaveChanges();
			return session;
		}
	}
}