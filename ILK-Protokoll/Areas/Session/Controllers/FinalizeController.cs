﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Models;
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
			var session = GetSession();
			session.End = DateTime.Now;
			session.LockedTopics = db.TopicLocks
				.Include(tl => tl.Session)
				.Include(tl => tl.Topic)
				.ToList();

			foreach ( var tl in session.LockedTopics)
				tl.Topic.IsReadOnly = true;
			

			string html = HelperMethods.RenderViewAsString(ControllerContext, "SessionReport", session);
			byte[] pdfcontent = HelperMethods.ConvertHTMLToPDF(html);
			System.IO.File.WriteAllBytes(@"C:\temp\mails\report.pdf", pdfcontent);

			var report = SessionReport.FromActiveSession(session);

			List<Topic> topics = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.TargetSessionType)
				.Include(t => t.Owner)
				.Include(t => t.Comments)
				.Include(t => t.Lock)
				.Where(t => t.Lock.Session.ID == session.ID)
				.ToList();

			//foreach (var t in topics)
			//{
			//	switch (t.Lock.Action)
			//	{
			//		case TopicAction.None:
			//			break;
			//		case TopicAction.Decide:
			//			t.IsReadOnly = true;
			//			t.Decision = new Decision()
			//			{
			//				OriginTopic = t,
			//				Report = report,
			//				Text = t.Proposal,
			//				Type = DecisionType.Resolution
			//			};
			//			break;
			//		case TopicAction.Close:
			//			t.IsReadOnly = true;
			//			t.Decision = new Decision()
			//			{
			//				OriginTopic = t,
			//				Report = report,
			//				Text = t.Proposal,
			//				Type = DecisionType.Closed
			//			};
			//			break;
			//		case TopicAction.Delete:
			//			db.Votes.RemoveRange(db.Votes.Where(v => v.Topic.ID == t.ID));
			//			db.Topics.Remove(t);
			//			break;
			//		default:
			//			throw new ArgumentOutOfRangeException();
			//	}
			//}
			try
			{
				db.SaveChanges();
			}
			catch (System.Data.DataException ex)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, ex.Message);
			}

			return PartialView("_ReportSuccess", 3);
		}

		public ActionResult ShowReport()
		{
			var session = GetSession();
			session.End = DateTime.Now;
			session.LockedTopics = db.TopicLocks
				.Include(tl => tl.Session)
				.Include(tl => tl.Topic)
				.ToList();

			return View("SessionReport", session);
		}
	}
}