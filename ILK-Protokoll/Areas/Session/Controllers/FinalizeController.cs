using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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
				.Include(tl => tl.Topic.Creator)
				.ToList();

			session.SessionType.LastDate = session.End;

			foreach (TopicLock tl in session.LockedTopics)
				tl.Topic.IsReadOnly = true;

			SessionReport report = SessionReport.FromActiveSession(session);
			report = db.SessionReports.Add(report);
			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var message = "Fehler beim Speichern des SessionReport<br />" + ErrorMessageFromException(e);
				return HTTPStatus(HttpStatusCode.InternalServerError, message);
			}

			string html = HelperMethods.RenderViewAsString(ControllerContext, "SessionReport", session);
			byte[] pdfcontent = HelperMethods.ConvertHTMLToPDF(html);

			try
			{
				System.IO.File.WriteAllBytes(SessionReport.Directory + report.FileName, pdfcontent);
			}
			catch (IOException e)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, e.Message);
			}
			catch (UnauthorizedAccessException e)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, e.Message);
			}

			List<Topic> topics = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.Lock)
				.Include(t => t.Assignments)
				.Where(t => t.Lock.Session.ID == session.ID)
				.ToList();

			var mailer = new UserMailer();

			try
			{
				mailer.SendSessionReport(session, report);
			}
			catch (Exception ex)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, "Fehler beim mailen: " + ex.Message);
			}

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
						db.Assignments.RemoveRange(db.Assignments.Where(a => a.TopicID == t.ID && !a.IsActive)); // Inaktive Aufgaben löschen
						foreach (var duty in t.Assignments.Where(a => a.Type == AssignmentType.Duty && a.IsActive))
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
			catch (DbEntityValidationException e)
			{
				var message = "Fehler beim Schreiben der Topics<br />" + ErrorMessageFromException(e);
				return HTTPStatus(HttpStatusCode.InternalServerError, message);
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
			Debug.Assert(session != null, "session != null");
			session.End = DateTime.Now;
			session.LockedTopics = db.TopicLocks
				.Include(tl => tl.Session)
				.Include(tl => tl.Topic)
				.ToList();

			return View("SessionReport", session);
		}
	}
}