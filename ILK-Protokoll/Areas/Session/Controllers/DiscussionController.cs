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
	public class DiscussionController : SessionBaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.SDiscussionStyle = "active";
		}

		// GET: Session/Topic
		public ActionResult Index(FilteredTopics filter)
		{
			ActiveSession session = GetSession();
			if (session == null)
				return RedirectToAction("Index", "Master");


			IQueryable<Topic> query = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.TargetSessionType)
				.Include(t => t.Owner)
				.Include(t => t.Comments)
				.Include(t => t.Lock)
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
				filter.Topics = query.ToList().OrderBy(t =>
				{
					TimeSpan time;
					return TimeSpan.TryParse(t.Time, out time) ? time : new TimeSpan();
				}).ThenByDescending(t => t.Priority).ThenBy(t => t.Created).ToList();
			}

			foreach (var topic in filter.Topics)
				topic.IsLocked = topic.Lock != null;

			return View(filter);
		}

		[HttpPost]
		public ActionResult _ChangeState(int id, TopicAction state)
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
				return PartialView("_StateButtons", tlock);
			}

			// Den Beschluss verhindern, falls der Punkt verschoben wird
			if (state != TopicAction.None && tlock.Topic.TargetSessionTypeID != null)
			{
				tlock.Message = "Der Punkt ist zum Verschieben vorgemerkt und darf daher nicht behandelt werden.";
				return PartialView("_StateButtons", tlock);
			}

			tlock.Action = state;

			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				string message = ErrorMessageFromException(e);
				return HTTPStatus(500, message);
			}

			return PartialView("_StateButtons", tlock);
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
				return HTTPStatus(500, message);
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}
	}
}