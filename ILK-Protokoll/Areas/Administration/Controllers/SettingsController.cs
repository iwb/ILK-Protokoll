using System.Web.Mvc;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	public class SettingsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.AdminStyle = "active";
			ViewBag.ASettingsStyle = "active";
		}

		// GET: Administration/Settings
		[HttpGet]
		public ActionResult Index()
		{
			return View(GetCurrentUser());
		}

		// POST: Administration/Settings
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Save(ColorScheme colorScheme)
		{
			GetCurrentUser().ColorScheme = colorScheme;
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}