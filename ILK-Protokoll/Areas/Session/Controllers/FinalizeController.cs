using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
				return RedirectToAction("Index", "Master");

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GenerateReport()
		{
			ActiveSession session = GetSession();
			session.End = DateTime.Now;
			session.LockedTopics = db.TopicLocks
				.Include(tl => tl.Session)
				.Include(tl => tl.Topic)
				.ToList();

			foreach (TopicLock tl in session.LockedTopics)
				tl.Topic.IsReadOnly = true;

			db.SaveChanges();

			SessionReport report = SessionReport.FromActiveSession(session);

			string html = HelperMethods.RenderViewAsString(ControllerContext, "SessionReport", session);
			byte[] pdfcontent = HelperMethods.ConvertHTMLToPDF(html);

			System.IO.File.WriteAllBytes(@"C:\ILK-Protokoll_Reports\" + report.FileName, pdfcontent);

			List<Topic> topics = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.Lock)
				.Where(t => t.Lock.Session.ID == session.ID)
				.ToList();

			foreach (Topic t in topics)
			{
				switch (t.Lock.Action)
				{
					case TopicAction.None:
						t.IsReadOnly = false;
						break;
					case TopicAction.Decide:
						t.Decision = new Decision
						{
							OriginTopic = t,
							Report = report,
							Text = t.Proposal,
							Type = DecisionType.Resolution
						};
						break;
					case TopicAction.Close:
						t.Decision = new Decision
						{
							OriginTopic = t,
							Report = report,
							Text = t.Proposal,
							Type = DecisionType.Closed
						};
						break;
					case TopicAction.Delete:
						db.Votes.RemoveRange(db.Votes.Where(v => v.Topic.ID == t.ID));
						db.Topics.Remove(t);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				db.TopicLocks.Remove(t.Lock);
			}
			db.ActiveSessions.Remove(session);
			db.SessionReports.Add(report);

			try
			{
				db.SaveChanges();
			}
			catch (DataException ex)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, ex.Message);
			}

			return PartialView("_ReportSuccess", 3);
		}

		public ActionResult ShowReport()
		{
			ActiveSession session = GetSession();
			session.End = DateTime.Now;
			session.LockedTopics = db.TopicLocks
				.Include(tl => tl.Session)
				.Include(tl => tl.Topic)
				.ToList();

			return View("SessionReport", session);
		}
	}
}