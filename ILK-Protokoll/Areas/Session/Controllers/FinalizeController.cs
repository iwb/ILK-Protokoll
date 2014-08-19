using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Mailers;
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
			session.Manager = GetCurrentUser();
			session.End = DateTime.Now;
			session.LockedTopics = db.TopicLocks
				.Include(tl => tl.Session)
				.Include(tl => tl.Topic)
				.ToList();

			session.SessionType.LastDate = session.End;

			foreach (TopicLock tl in session.LockedTopics)
				tl.Topic.IsReadOnly = true;

			SessionReport report = SessionReport.FromActiveSession(session);
			report = db.SessionReports.Add(report);
			db.SaveChanges();


			string html = HelperMethods.RenderViewAsString(ControllerContext, "SessionReport", session);
			byte[] pdfcontent = HelperMethods.ConvertHTMLToPDF(html);

			System.IO.File.WriteAllBytes(SessionReport.Directory + report.FileName, pdfcontent);

			List<Topic> topics = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.Lock)
				.Include(t => t.Assignments)
				.Where(t => t.Lock.Session.ID == session.ID)
				.ToList();

			var mailer = new UserMailer();
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
						foreach ( var duty in t.Assignments.Where(a => a.Type == AssignmentType.Duty))
							mailer.SendNewAssignment(duty);
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
			Session.Remove("SessionID");

			try
			{
				db.SaveChanges();
			}
			catch (DataException ex)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, ex.Message);
			}

			return PartialView("_ReportSuccess", report.ID);
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