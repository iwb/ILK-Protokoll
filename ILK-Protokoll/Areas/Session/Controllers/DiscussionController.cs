using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.Models;

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
		public ActionResult Index()
		{
			ActiveSession session = GetSession();
			if (session == null)
				return RedirectToAction("Index", "Master");

			List<Topic> topics = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.TargetSessionType)
				.Include(t => t.Owner)
				.Include(t => t.Comments)
				.Include(t => t.Lock)
				.Where(t => t.Decision == null && !t.IsReadOnly)
				.Where(
					t => t.Lock.Session.ID == session.ID || (t.SessionTypeID == session.SessionType.ID && t.Created < session.Start && !(t.ResubmissionDate >= session.Start )))
				.OrderByDescending(t => t.Priority)
				.ThenBy(t => t.Created).ToList();

			foreach (Topic topic in topics)
				topic.IsLocked = topic.Lock != null;

			return View(topics);
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