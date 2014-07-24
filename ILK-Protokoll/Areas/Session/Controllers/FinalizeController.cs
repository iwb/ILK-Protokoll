﻿using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Areas.Session.ViewModels;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.util;

namespace ILK_Protokoll.Areas.Session.Controllers
{
	public class FinalizeController : SessionBaseController
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
				session = ResumeSession(4);

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GenerateReport()
		{
			string html = HelperMethods.RenderViewAsString(ControllerContext, "SessionReport", GetSession());
			byte[] pdfcontent = HelperMethods.ConvertHTMLToPDF(html);
			System.IO.File.WriteAllBytes(@"C:\temp\mails\report.pdf", pdfcontent);

			return PartialView("_ReportSuccess", 3);
		}
	}
}