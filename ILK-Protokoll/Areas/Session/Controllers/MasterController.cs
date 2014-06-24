using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Controllers;
using Microsoft.Ajax.Utilities;

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
			var session = Session["S"] as ActiveSession;
			if (session == null)
			{
				session = new ActiveSession(db.SessionTypes.First());
				Session["S"] = session;
				return session;
				throw new InvalidOperationException("Es konnte keine laufende Sitzung gefudnen werden.");
			}
			return session;
		}

		// GET: Session/Master
		public ActionResult Index()
		{
			ViewBag.SessionTypes = new SelectList(db.SessionTypes, "ID", "Name");
			return View(db.Sessions.ToList());
		}

		public ActionResult Create(int SessionTypeID)
		{
			var st = db.SessionTypes.Find(SessionTypeID);
			if (st != null)
			{
				var session = db.Sessions.Add(new ActiveSession(st));
				db.SaveChanges();
				Session["S"] = session;
				return View(session);
			}
			else
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Sitzungstyp nicht gefunden.");
			}
		}

		public ActionResult Resume(int SessionID)
		{
			var session = db.Sessions.Find(SessionID);
			if (session != null)
			{
				Session["S"] = session;
				return View(session);
			}
			else
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Sitzungstyp nicht gefunden.");
			}
		}

		public ActionResult Edit()
		{
			return View(GetSession());
		}
	}
}