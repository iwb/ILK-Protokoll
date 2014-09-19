using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models;

namespace ILK_Protokoll.Areas.Session.Controllers
{
	public class MasterController : SessionBaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.SMasterStyle = "active";
		}

		// GET: Session/Master
		public ActionResult Index()
		{
			ViewBag.SessionTypes = new SelectList(db.GetActiveSessionTypes(), "ID", "Name");
			return View(db.ActiveSessions.Include(session => session.Manager).ToList());
		}

		public ActionResult Create(int SessionTypeID)
		{
			var uid = GetCurrentUserID();
			var activeSession = (from s in db.ActiveSessions
				where s.Manager.ID == uid && s.SessionType.ID == SessionTypeID
				select s.ID).SingleOrDefault();

			if (activeSession > 0)
				return View(ResumeSession(activeSession)); 

			var st = db.SessionTypes.Find(SessionTypeID);
			if (st != null)
				return View(CreateNewSession(st));
			else
				return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Sitzungstyp nicht gefunden.");
		}

		public ActionResult Resume(int SessionID)
		{
			try
			{
				ResumeSession(SessionID);
			}
			catch (ArgumentException)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Sitzungstyp nicht gefunden.");
			}
			return RedirectToAction("Edit");
		}

		[HttpGet]
		public ActionResult Edit()
		{
			var session = GetSession();
			if (session == null)
				return RedirectToAction("Index");

			ViewBag.UserDict = session.SessionType.Attendees.ToDictionary(u => u, u => session.PresentUsers.Contains(u));

			return View(session);
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AbortSession(int id)
		{
			var session = GetSession();
			if (session == null)
				return RedirectToAction("Index");

			if (session.ID != id)
				return HTTPStatus(422, "Die Sitzungs-ID stimmt nicht überein!");

			db.ActiveSessions.Remove(session);
			db.SaveChanges();
			Session.Remove("SessionID");

			return RedirectToAction("Index", "Master", new { Area = "Session" });
		}
	}
}