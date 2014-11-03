using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.Models;
using ILK_Protokoll.ViewModels;
using StackExchange.Profiling;

namespace ILK_Protokoll.Areas.Session.Controllers
{
	/// <summary>
	/// Ermöglicht eine Übersicht über alle laufenden und abgeschlossenen Diskussionen.
	/// </summary>
	public class DiscussionController : SessionBaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.SDiscussionStyle = "active";
		}

		// GET: Session/Topic
		public ActionResult Index(FilteredTopics filter, string viewPref)
		{
			ActiveSession session = GetSession();
			if (session == null)
				return RedirectToAction("Index", "Master");

			if (viewPref == "Table" || viewPref == "Panels")
				Session["DiscussionView"] = viewPref;

			PrepareTopics(filter, session);

			if ((Session["DiscussionView"] as string) == "Table")
				return View("IndexTable", filter);
			else
				return View("IndexPanels", filter); // Default
		}

		private void PrepareTopics(FilteredTopics filter, ActiveSession session)
		{
			// Zwecks Performance wäre hier eigentlich eine Query-Projektion angebracht
			IQueryable<Topic> query = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.TargetSessionType)
				.Include(t => t.Owner)
				.Include(t => t.Comments)
				.Include(t => t.Lock)
				.Include(t => t.Tags)
				.Include(t => t.Votes)
				.Include(t => t.UnreadBy)
				.Include(t => t.Assignments)
				.Where(t => t.Decision == null && !t.IsReadOnly)
				.Where(t =>
					t.Lock.Session.ID == session.ID ||
					(t.SessionTypeID == session.SessionType.ID && t.Created < session.Start && !(t.ResubmissionDate >= session.Start)));

			if (filter.ShowPriority >= 0)
				query = query.Where(t => t.Priority == (Priority)filter.ShowPriority);

			if (filter.Timespan != 0)
			{
				if (filter.Timespan > 0) // Nur die letzten x Tage anzeigen
				{
					var cutoff = DateTime.Today.AddDays(-filter.Timespan);
					query = query.Where(t => t.Created >= cutoff);
				}
				else // Alles VOR den letzten x Tagen anzeigen
				{
					var cutoff = DateTime.Today.AddDays(filter.Timespan);
					query = query.Where(t => t.Created < cutoff);
				}
			}

			if (filter.OwnerID != 0)
				query = query.Where(a => a.OwnerID == filter.OwnerID);

			filter.UserList = CreateUserSelectList();
			filter.PriorityList = TopicsController.PriorityChoices(filter.ShowPriority);
			filter.SessionTypeList = new SelectList(db.GetActiveSessionTypes(), "ID", "Name");

			filter.TimespanList = TopicsController.TimespanChoices(filter.Timespan);

			using (MiniProfiler.Current.Step("Sortierung der Themen"))
			{
				filter.Topics = query.ToList().OrderByDescending(t => t.IsUnreadBy(GetCurrentUserID())).ThenBy(t =>
				{
					TimeSpan time;
					return TimeSpan.TryParse(t.Time, out time) ? time : new TimeSpan(24, 0, 0);
				}).ThenByDescending(t => t.Priority).ThenBy(t => t.Created).ToList();
			}

			foreach (var topic in filter.Topics)
				topic.IsLocked = topic.Lock != null;
		}

		[HttpPost]
		public ActionResult _ChangeState(int id, TopicAction state, string view)
		{
			var appropriateView = view == "Table" ? "_StateDropdown" : "_StateButtons";

			return ChangeState(id, state, appropriateView);
		}

		private ActionResult ChangeState(int id, TopicAction state, string view)
		{
			ActiveSession session = GetSession();
			if (session == null)
				return HTTPStatus(HttpStatusCode.Forbidden, "Keine Sitzung gefunden.");

			TopicLock tlock = db.TopicLocks
				.Include(tl => tl.Session)
				.Include(tl => tl.Topic)
				.Single(tl => tl.TopicID == id);

			if (tlock.Session.ID != session.ID)
				return HTTPStatus(HttpStatusCode.Forbidden, "Falsche Sitzung.");

			// Den Beschluss verhindern, falls noch offene Aufgaben vorliegen
			if (state == TopicAction.Decide &&
			    tlock.Topic.Assignments.Any(a => a.Type == AssignmentType.ToDo && !a.IsDone && a.IsActive))
			{
				tlock.Message = "Es liegen noch offene ToDo-Aufgaben vor. Dieses Thema kann daher nicht beschlossen werden.";
				return PartialView(view, tlock);
			}

			// Den Beschluss verhindern, falls der Punkt verschoben wird
			if (state != TopicAction.None && tlock.Topic.TargetSessionTypeID != null)
			{
				tlock.Message = "Der Punkt ist zum Verschieben vorgemerkt und darf daher nicht behandelt werden.";
				return PartialView(view, tlock);
			}

			tlock.Action = state;

			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				string message = ErrorMessageFromException(e);
				return HTTPStatus(HttpStatusCode.InternalServerError, message);
			}

			return PartialView(view, tlock);
		}

		[HttpPost]
		public ActionResult _ChangeResubmissionDate(int topicID, DateTime? resubmissionDate)
		{
			var topic = db.Topics.Find(topicID);
			topic.ResubmissionDate = resubmissionDate;

			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				string message = ErrorMessageFromException(e);
				return HTTPStatus(HttpStatusCode.InternalServerError, message);
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}
	}
}