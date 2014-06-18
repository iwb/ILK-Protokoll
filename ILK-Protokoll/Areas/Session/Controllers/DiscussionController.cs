using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Controllers
{
	public class DiscussionController : BaseController
	{
		// GET: Session/Topic
		public ActionResult Index()
		{
			List<Topic> topics = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.TargetSessionType)
				.Include(t => t.Owner)
				.Include(t => t.ToDos)
				.Include(t => t.Duties)
				.Include(t => t.Comments).ToList();
			return View(topics);
		}
	}
}