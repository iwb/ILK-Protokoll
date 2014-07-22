using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Areas.Session.ViewModels;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.util;

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
			ActiveSession session = GetSession();
			if (session == null)
				return RedirectToAction("Index", "Master");

			return View();
		}

		[HttpPost]
		[ActionName("Index")]
		[ValidateAntiForgeryToken]
		public ActionResult Confirmed()
		{
			var vm = new ReportViewModel { Title = "Testprotokoll" + GetSession().ID };

			string html = HelperMethods.RenderViewAsString(ControllerContext, "SessionReport", vm);
			byte[] pdfcontent = HelperMethods.ConvertHTMLToPDF(html);
			System.IO.File.WriteAllBytes(@"C:\temp\mails\repor.pdf", pdfcontent);

			return View();
		}
	}
}