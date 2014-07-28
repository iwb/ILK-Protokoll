using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class DecisionsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.DecisionStyle = "active";
		}

		// GET: Decisions
		public ActionResult Index()
		{
			var decisions = db.Decisions.Include(d => d.OriginTopic);
			return View(decisions.ToList());
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
