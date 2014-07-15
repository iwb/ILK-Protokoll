using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
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
			var session = GetSession();

			List<Topic> topics = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.TargetSessionType)
				.Include(t => t.Owner)
				.Include(t => t.Assignments)
				.Include(t => t.Comments)
				.Where(t => t.SessionTypeID == session.ID)
				.OrderByDescending(t => t.Priority)
				.ThenByDescending(t => t.Created).ToList();

			return View(topics);
		}
	}
}