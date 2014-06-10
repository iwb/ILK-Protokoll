using System.Web.Mvc;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	public class RecycleBinController : Controller
	{
		// GET: Administration/RecycleBin
		public ActionResult Index()
		{
			return View();
		}
	}
}