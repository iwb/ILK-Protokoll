using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Mailers;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;
using StackExchange.Profiling;

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
		public async Task<ActionResult> GenerateReport()
		{
			ActiveSession session = GetSession();

			if (session == null)
				return HTTPStatus(HttpStatusCode.InternalServerError, "Active Session not found.");

			session.ManagerID = GetCurrentUserID();
			session.End = DateTime.Now;
			session.LockedTopics = session.LockedTopics.ToList(); // Wird mehrfach durchlaufen

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

			string html;
			byte[] pdfcontent;
			using (MiniProfiler.Current.Step("Rendering des Reports"))
			{
				html = HelperMethods.RenderViewAsString(ControllerContext, "SessionReport", session);
			}
			using (MiniProfiler.Current.Step("PDF Generierung"))
			{
				pdfcontent = HelperMethods.ConvertHTMLToPDF(html);
			}

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

			var result = await ProcessTopics(session, report);
			if (result != null)
				return result;

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

		private async Task<ActionResult> ProcessTopics(ActiveSession session, SessionReport report)
		{
			List<Topic> topics = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.Lock)
				.Include(t => t.Assignments)
				.Where(t => t.Lock.Session.ID == session.ID)
				.ToList();

			var mailer = new UserMailer();

			try
			{
				mailer.SendSessionReport(topics, report);
			}
			catch (Exception ex)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, "Fehler beim Versenden der E-Mails: " + ex.Message);
			}

			foreach (var t in topics)
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
						// Inaktive Aufgaben löschen
						db.Assignments.RemoveRange(db.Assignments.Where(a => a.TopicID == t.ID && !a.IsActive));

						// Für aktive Umsetzungaufgaben E-Mails verschicken
						var tasks = new List<Task>();
						foreach (var duty in t.Assignments.Where(a => a.Type == AssignmentType.Duty && a.IsActive))
							tasks.Add(mailer.SendNewAssignment(duty));
						await Task.WhenAll(tasks);

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
						db.DeleteTopic(t.ID);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				if (t.Lock != null) // Wenn gelöscht wird, ist t.Lock hier bereits null
					db.TopicLocks.Remove(t.Lock);
			}

			return null;
		}

		public ActionResult ShowReport()
		{
			ActiveSession session = GetSession();
			Debug.Assert(session != null, "session != null");
			session.End = DateTime.Now;

			return View("SessionReport", session);
		}
	}
}