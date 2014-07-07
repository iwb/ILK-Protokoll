using System;
using System.Linq;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Controllers
{
	public class AssignmentsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.AssignmentStyle = "active";
		}

		// GET: Assignments
		public ActionResult Index(
			[Bind(Include = "ShowToDos,ShowDuties,ShowPast, ShowFuture, ShowDone, UserID")] FilteredAssignments filter)
		{
			IQueryable<Assignment> query = db.Assignments;

			if (!filter.ShowToDos)
				query = query.Where(a => a.Type != AssignmentType.ToDo);

			if (!filter.ShowDuties)
				query = query.Where(a => a.Type != AssignmentType.Duty);

			if (!filter.ShowPast)
				query = query.Where(a => a.DueDate > DateTime.Today);

			if (!filter.ShowFuture)
				query = query.Where(a => a.DueDate <= DateTime.Today);

			if (!filter.ShowDone)
				query = query.Where(a => !a.IsDone);

			if (filter.UserID != 0)
				query = query.Where(a => a.Owner.ID == filter.UserID);

			filter.Assignments = query.OrderBy(a => a.DueDate).ToList();
			filter.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "Shortname");

			return View(filter);
		}

		// GET: Assignments/Details/5
		public ActionResult Details(int id)
		{
			return null;
		}

		// GET: Assignments/Create
		[HttpGet]
		public ActionResult Create(int? topicID)
		{
			var a = new Assignment();

			if (topicID.HasValue)
				a.Topic = db.Topics.Find(topicID.Value);

			ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");

			return View(a);
		}

		// POST: Administration/SessionTypes/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Type,Title,Description,TopicID,OwnerID,DueDate,")] Assignment input)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
				return View(input);
			}
			else
			{
				db.Assignments.Add(input);
				db.SaveChanges();
				return RedirectToAction("Index", "Assignments");
			}
		}
	}
}