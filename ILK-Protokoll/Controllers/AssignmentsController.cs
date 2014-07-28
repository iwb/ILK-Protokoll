using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Mailers;
using ILK_Protokoll.Models;
using ILK_Protokoll.ViewModels;
using Mvc.Mailer;

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

		/// <summary>
		/// Verschickt Erinnerungen für Aufgaben, die bald fällig werden, oder überfällig sind.
		/// </summary>
		/// <param name="db">Ein Datenbankkontext</param>
		/// <returns>Anzahl der E-maisl, die verschickt wurden.</returns>
		public static int SendReminders(DataContext db)
		{
			/* Gespeichert wird in der Datenbank ja nur der Datumsanteil. Eine Aufgabe, die am Donnerstag fällig wird, enthält hier also Do, 0:00 als Zeitangabe.
			 * Tatsächlich fällig wird sie allerdings erst am Donnerstag um 24:00. Damit erklärt sich der eine Tag Unterschied in den cutoff-Daten. */
			var mailer = new UserMailer();
			var cutoff = DateTime.Now.AddDays(6);
			var due = db.Assignments.Where(a => !a.IsDone && !a.ReminderSent && a.DueDate < cutoff).ToList();
			foreach (var a in due)
			{
				mailer.SendAssignmentReminder(a);
				a.ReminderSent = true;
			}
			db.SaveChanges();

			cutoff = DateTime.Now.AddDays(-1);
			var overdue = db.Assignments.Where(a => !a.IsDone && a.DueDate < cutoff).ToList();
			foreach (var a in overdue)
				mailer.SendAssignmentOverdue(a);
			
			return due.Count + overdue.Count;
		}
		
		// GET: Assignments/Create
		[HttpGet]
		public ActionResult Create(int topicID)
		{
			if (IsTopicLocked(topicID))
				throw new TopicLockedException();

			var a = new AssignmentEdit {TopicID = topicID};

			ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
			return View(a);
		}

		// POST: Administration/SessionTypes/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind] AssignmentEdit input)
		{
			if (IsTopicLocked(input.TopicID))
				throw new TopicLockedException();
			else if (!ModelState.IsValid)
			{
				ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
				return View(input);
			}
			else
			{
				var assignment = db.Assignments.Create();
				TryUpdateModel(assignment, new[] {"Type", "Title", "Description", "TopicID", "OwnerID", "DueDate"});

				db.Assignments.Add(assignment);
				db.SaveChanges();

				if (assignment.Type == AssignmentType.ToDo)
				{
					var mailer = new UserMailer();
					mailer.SendNewAssignment(assignment);
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

			if (IsTopicLocked(assignment.TopicID))
				throw new TopicLockedException();

			ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");

			var vm = new AssignmentEdit()
			{
				Description = assignment.Description,
				DueDate = assignment.DueDate,
				ID = assignment.ID,
				OwnerID = assignment.OwnerID,
				Title = assignment.Title,
				TopicID = assignment.TopicID,
				Type = assignment.Type
			};

			return View(vm);
		}

		// POST: Assignments/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind] AssignmentEdit input)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
				return View(input);
			}

			var assignment = db.Assignments.Find(input.ID);

			if (IsTopicLocked(assignment.TopicID))
				throw new TopicLockedException();

			TryUpdateModel(assignment, new[] {"Type", "Title", "Description", "TopicID", "OwnerID", "DueDate"});
			db.Entry(assignment).State = EntityState.Modified;
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}