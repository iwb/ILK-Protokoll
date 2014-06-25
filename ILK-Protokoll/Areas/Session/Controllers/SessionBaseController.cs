using System;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Controllers;

namespace ILK_Protokoll.Areas.Session.Controllers
{
	public class SessionBaseController : BaseController
	{
		protected ActiveSession GetSession()
		{
			var sessionID = (int?)Session["SessionID"];
			if (sessionID.HasValue && sessionID > 0)
				return db.ActiveSessions.Find(sessionID);
			else
				return null;
		}

		protected ActiveSession CreateNewSession(SessionType type)
		{
			var session = db.ActiveSessions.Add(new ActiveSession(type));
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
			return session;
		}
	}
}