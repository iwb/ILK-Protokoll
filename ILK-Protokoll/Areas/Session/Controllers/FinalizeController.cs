using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ILK_Protokoll.Controllers;

namespace ILK_Protokoll.Areas.Session.Controllers
{
	public class FinalizeController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.SFinalizeStyle = "active";
		}

		// GET: Session/Finalize
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ActionName("Index")]
		[ValidateAntiForgeryToken]
		public ActionResult Confirmed()
		{
			return View();
		}
	}
}