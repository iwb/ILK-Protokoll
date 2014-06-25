using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Controllers;

namespace ILK_Protokoll.Areas.Session.Controllers
{
	public class MasterController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.SMasterStyle = "active";
		}

		private ActiveSession GetSession()
		{
			var sessionID = (int?)Session["SessionID"];
			if (sessionID.HasValue && sessionID > 0)
				return db.ActiveSessions.Find(sessionID);
			else
				return null;
		}

		// GET: Session/Master
		public ActionResult Index()
		{
			ViewBag.SessionTypes = new SelectList(db.SessionTypes, "ID", "Name");
			return View(db.ActiveSessions.ToList());
		}

		public ActionResult Create(int SessionTypeID)
		{
			var st = db.SessionTypes.Find(SessionTypeID);
			if (st != null)
			{
				var session = db.ActiveSessions.Add(new ActiveSession(st));
				db.SaveChanges();
				Session["SessionID"] = session.ID;
				return View(session);
			}
			else
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Sitzungstyp nicht gefunden.");
			}
		}

		public ActionResult Resume(int SessionID)
		{
			var session = db.ActiveSessions.Find(SessionID);
			if (session != null)
			{
				Session["SessionID"] = session.ID;
				return RedirectToAction("Edit");
			}
			else
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Sitzungstyp nicht gefunden.");
			}
		}

		[HttpGet]
		public ActionResult Edit()
		{
			var session = GetSession();
			if (session == null)
				return RedirectToAction("Index");

			ViewBag.UserDict = session.SessionType.Attendees.ToDictionary(u => u, u => session.PresentUsers.Contains(u));

			return View(GetSession());
		}

		[HttpPost]
		public ActionResult Edit(Dictionary<int, bool> Users,
			[Bind(Include = "AdditionalAttendees,Notes")] ActiveSession input)
		{
			var session = GetSession();
			if (session == null)
				return RedirectToAction("Index");

			session.AdditionalAttendees = input.AdditionalAttendees;
			session.Notes = input.Notes;
			session.PresentUsers = Users.Where(kvp => kvp.Value).Select(kvp => db.Users.Find(kvp.Key)).ToList();
			db.SaveChanges();

			return RedirectToAction("Index", "Lists", new {Area = "Session"});
		}
	}
}