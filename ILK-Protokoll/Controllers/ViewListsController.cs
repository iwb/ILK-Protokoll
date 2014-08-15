using System.Web.Mvc;

namespace ILK_Protokoll.Controllers
{
	public class ViewListsController : Controller
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.ViewListsStyle = "active";
		}

		// GET: Lists
		public ActionResult Index()
		{
			return View();
		}
	}
}