using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Mailers;
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
			var a = db.Assignments.Find(id);
			return View(a);
		}

		public ActionResult MarkAsDone(int id)
		{
			var a = db.Assignments.Find(id);
			a.IsDone = true;
			db.SaveChanges();

			return RedirectToAction("Details", new {id});
		}

		public ActionResult MarkAsOpen(int id)
		{
			var a = db.Assignments.Find(id);
			a.IsDone = false;
			db.SaveChanges();

			return RedirectToAction("Details", new {id});
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

				var assignment = db.Assignments
					.Include(a => a.Owner)
					.Include(a => a.Topic)
					.Single(a => a.ID == input.ID);

				if (assignment.Type == AssignmentType.ToDo)
				{
					var mailer = new UserMailer();
					var msg = mailer.NewAssignment(assignment);
					msg.Send();
				}

				return RedirectToAction("Index", "Assignments");
			}
		}

		// GET: Assignments/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			Assignment assignment = db.Assignments.Find(id);

			if (assignment == null)
				return HttpNotFound();

			ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
			return View(assignment);
		}

		// POST: Assignments/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(
			[Bind(Include = "ID,Type,Title,Description,TopicID,OwnerID,DueDate,ReminderSent,IsDone")] Assignment assignment)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
				return View(assignment);
			}

			db.Entry(assignment).State = EntityState.Modified;
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}