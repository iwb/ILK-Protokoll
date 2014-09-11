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
			var user = db.Users.Find(GetCurrentUserID());
			return View(user);
		}

		// POST: Administration/Settings
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Save(ColorScheme colorScheme)
		{
			// GetCurrentUser() trackt keine Änderungen, daher den User neu aus der DB holen
			var user = db.Users.Find(GetCurrentUserID());
			user.ColorScheme = colorScheme;
			db.SaveChanges();
			Session["CurrentUser"] = null;
			return RedirectToAction("Index");
		}
	}
}