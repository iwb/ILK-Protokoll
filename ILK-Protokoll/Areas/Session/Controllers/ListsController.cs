using System.Web.Mvc;

namespace ILK_Protokoll.Areas.Session.Controllers
{
	public class ListsController : SessionBaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.SListsStyle = "active";
		}

		// GET: Session/List
		public ActionResult Index()
		{
			var session = GetSession();
			if (session == null)
				return RedirectToAction("Index", "Master");

			return View();
		}
	}
}