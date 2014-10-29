using System.Web.Mvc;
using ILK_Protokoll.Controllers;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	public class AdminHomeController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.AdminStyle = "active";
		}

		// GET: Administration/Home
		public ActionResult Index()
		{
			return View();
		}
	}
}