using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
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
			var session = Session["S"] as ActiveSession;
			if (session == null)
				throw new InvalidOperationException("Es konnte keine laufende Sitzung gefudnen werden.");
			return session;
		}

		// GET: Session/Master
		public ActionResult Index()
		{
			ViewBag.SessionTypes = new SelectList(db.SessionTypes, "ID", "Name");
			return View(db.Sessions.ToList());
		}

		public ActionResult Create(int SessionTypeId)
		{
			var st = db.SessionTypes.Find(SessionTypeId);
			if (st != null)
			{
				var session = db.Sessions.Add(new ActiveSession(st));
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