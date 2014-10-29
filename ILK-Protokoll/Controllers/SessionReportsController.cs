using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class SessionReportsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.SessionReportStyle = "active";
		}

		// GET: SessionReports
		public ActionResult Index()
		{
			return View(db.SessionReports.Include(sr => sr.Manager).ToList());
		}

		// GET: SessionReports/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			SessionReport SessionReport = db.SessionReports.Find(id);
			if (SessionReport == null)
				return HttpNotFound();
			return File(SessionReport.Directory + SessionReport.FileName, "application/pdf");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}