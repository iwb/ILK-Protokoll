using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Mailers;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;
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
			[Bind(Include = "ShowToDos,ShowDuties,ShowPast,ShowFuture,ShowDone,UserID")] FilteredAssignments filter)
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
			filter.UserList = CreateUserSelectList();

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
			var assignment = db.Assignments.Find(id);
			assignment.IsDone = false;

			var topic = db.Topics
				.Include(t => t.Lock)
				.Include(t => t.Lock.Session.Manager)
				.Single(t => t.ID == assignment.TopicID);

			if (assignment.Type == AssignmentType.ToDo && topic.Lock != null)
				topic.Lock.Action = TopicAction.None; // Falls eine ToDo hinzugefügt wird, Wiedervorlage auswählen.

			db.SaveChanges();

			return RedirectToAction("Details", new {id});
		}

		/// <summary>
		///    Verschickt Erinnerungen für Aufgaben, die bald fällig werden, oder überfällig sind.
		/// </summary>
		/// <param name="db">Ein Datenbankkontext</param>
		/// <returns>Anzahl der Aufgaben, die betrachtet wurden.</returns>
		public static string SendReminders(DataContext db)
		{
			/* Gespeichert wird in der Datenbank ja nur der Datumsanteil. Eine Aufgabe, die am Donnerstag fällig wird, enthält hier also Do, 0:00 als Zeitangabe.
			 * Tatsächlich fällig wird sie allerdings erst am Donnerstag um 24:00. Damit erklärt sich der eine Tag Unterschied in den cutoff-Daten. */

			var mailsSent = 0;
			var statusMsg = new StringBuilder();
			var mailer = new UserMailer();
			var cutoff = DateTime.Now.AddDays(6);
			var due =
				db.Assignments.Where(a => !a.IsDone && !a.ReminderSent && a.IsActive && a.DueDate < cutoff && a.Owner.IsActive)
					.ToList();
			foreach (var a in due)
			{
				// Erinnerung für Umsetzungsaufgaben nur, wenn ein Beschluss gefallen ist 
				if (a.Type == AssignmentType.ToDo || a.Topic.HasDecision(DecisionType.Resolution))
				{
					mailer.SendAssignmentReminder(a);
					a.ReminderSent = true;
					mailsSent++;
				}
			}
			db.SaveChanges();

			statusMsg.AppendFormat("Zur Erinnerung: {0} Aufgaben, {1} Erinnerungsmails versendet.\n", due.Count, mailsSent);
			mailsSent = 0;

			cutoff = DateTime.Now.AddDays(-1);
			var overdue = db.Assignments.Where(a => !a.IsDone && a.IsActive && a.DueDate < cutoff && a.Owner.IsActive).ToList();
			foreach (var a in overdue.Where(a => a.Type == AssignmentType.ToDo || a.Topic.HasDecision(DecisionType.Resolution)))
			{
				mailer.SendAssignmentOverdue(a);
				mailsSent++;
			}

			statusMsg.AppendFormat("Möglicherweise überfällig: {0}, {1} E-Mails versendet.\n", due.Count, mailsSent);

			return statusMsg.ToString();
		}

		// GET: Assignments/Create
		[HttpGet]
		public ActionResult Create(int topicID)
		{
			if (IsTopicLocked(topicID))
				throw new TopicLockedException();

			var a = new AssignmentEdit
			{
				TopicID = topicID,
				OwnerSelectList = CreateOwnerSelectListitems()
			};

			return View(a);
		}

		// POST: Administration/SessionTypes/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([Bind] AssignmentEdit input)
		{
			if (IsTopicLocked(input.TopicID))
				throw new TopicLockedException();
			else if (!ModelState.IsValid)
			{
				input.OwnerSelectList = CreateOwnerSelectListitems();
				return View(input);
			}
			else
			{
				var topic = db.Topics.Find(input.TopicID);

				// Ungelesen-Markierung und Aktion aktualisieren
				MarkAsUnread(topic);
				if (input.Type == AssignmentType.ToDo && topic.Lock != null)
					topic.Lock.Action = TopicAction.None; // Falls ein ToDo hinzugefügt wird, Wiedervorlage auswählen.

				var userlist = input.OwnerID >= 0
					? input.OwnerID.ToEnumerable()
					: db.SessionTypes.Find(-input.OwnerID).Attendees.Select(a => a.ID).ToList();

				var mailer = new UserMailer();
				var list = new List<Assignment>();
				foreach (var userid in userlist)
				{
					var a = Assignment.FromViewModel(input);
					a.Owner = db.Users.Find(userid);
					list.Add(a);
				}

				db.Assignments.AddRange(list);
				db.SaveChanges();

				await Task.WhenAll(list
					.Where(assignment => assignment.Type == AssignmentType.ToDo && input.IsActive)
					.Select(a => mailer.SendNewAssignment(a)));

				return RedirectToAction("Details", "Topics", new {id = input.TopicID});
			}
		}

		/// <summary>
		///    Erzeugt die Auswahlliste für die Besitzer. Da in #83 auch Gruppen auswählbar sein sollen, werden diese kombiniert.
		///    Einzelnbenutzer sind durch einen positiven Wert gekennzeichnet. Dies ist die UserID. Gruppen werden durch einen
		///    negativen Wert beschrieben, der die negierte Sitzungstypus-ID ist. Werte von 0 sind für beide IDs ungültig und
		///    sollten daher nie vorkommen. Die Unterscheidung innerhelb der GUI erfolgt über die Gruppen in dem Dropdown Menü.
		/// </summary>
		/// <returns>Eine Liste, in der Einzelbenutzer und Benutzergruppen enthalten sind.</returns>
		private List<SelectListItem> CreateOwnerSelectListitems()
		{
			var usergroup = new SelectListGroup {Name = "Benutzer"};
			var groupsgroup = new SelectListGroup {Name = "Gruppen"};

			var listitems = db.GetUserOrdered(GetCurrentUser()).Select(u => new SelectListItem
			{
				Text = u.ShortName,
				Value = u.ID.ToString(),
				Group = usergroup
			}).Concat(db.GetActiveSessionTypes().Select(x => new SelectListItem
			{
				Text = x.Name,
				Value = (-x.ID).ToString(),
				Group = groupsgroup
			})).ToList();
			return listitems;
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

			ViewBag.UserList = CreateUserSelectList();

			var vm = new AssignmentEdit
			{
				Description = assignment.Description,
				DueDate = assignment.DueDate,
				ID = assignment.ID,
				OwnerID = assignment.OwnerID,
				Title = assignment.Title,
				TopicID = assignment.TopicID,
				Type = assignment.Type,
				IsActive = assignment.IsActive
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
				ViewBag.UserList = CreateUserSelectList();
				return View(input);
			}

			var assignment = db.Assignments.Find(input.ID);
			if (IsTopicLocked(assignment.TopicID))
				throw new TopicLockedException();
			if (assignment.IsActive && assignment.Type == AssignmentType.ToDo)
				input.IsActive = true; // Verhindert, dass das Aktiv-Flag bei ToDos zurück auf false geändert wird.

			if (assignment.Type == AssignmentType.ToDo && !assignment.IsActive && input.IsActive)
				// Das Aktiv-Flag hat sich auf true geändert
			{
				var mailer = new UserMailer();
				mailer.SendNewAssignment(assignment);
			}

			assignment.IncorporateUpdates(input);
			db.Entry(assignment).State = EntityState.Modified;
			db.SaveChanges();

			return RedirectToAction("Details", "Topics", new {id = input.TopicID});
		}

		// GET: Assignments/Delete/5
		public ActionResult Delete(int? id)
		{
			if (GetSession() == null)
				return HTTPStatus(HttpStatusCode.Forbidden, "Nur im Sitzungsmodus dürfen Aufgaben gelöscht werden!");

			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			var assignment = db.Assignments.Find(id);
			if (assignment == null)
				return HttpNotFound();
			return View(assignment);
		}

		// POST: Assignments/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			var assignment = db.Assignments.Find(id);
			var tid = assignment.TopicID;
			db.Assignments.Remove(assignment);
			db.SaveChanges();
			return RedirectToAction("Details", "Topics", new {id = tid});
		}
	}
}