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
				.Where(t => t.Lock.Session.ID == session.ID || t.SessionTypeID == session.SessionType.ID)
				.OrderByDescending(t => t.Priority)
				.ThenBy(t => t.Created).ToList();

			foreach (Topic topic in topics)
				topic.IsLocked = topic.Lock != null;

			return View(topics);
		}

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
	}
}