using System;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Controllers;

namespace ILK_Protokoll.Areas.Session.Controllers
{
	public class SessionBaseController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			if (GetSession() != null)
			{
				ViewBag.LastSession = GetSession().SessionType.LastDate;
				ViewBag.CurrentSession = GetSession();
			}
		}

		protected ActiveSession CreateNewSession(SessionType type)
		{
			var session = db.ActiveSessions.Add(new ActiveSession(type) {Manager = GetCurrentUser()});
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