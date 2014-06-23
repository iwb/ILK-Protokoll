using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;

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
		public ActionResult Index()
		{
			return View(db.Assignments.ToList());
		}

		// GET: Assignments/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Assignment assignment = db.Assignments.Find(id);
			if (assignment == null)
			{
				return HttpNotFound();
			}
			return View(assignment);
		}

		// GET: Assignments/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Assignments/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,Title,Description,DueDate,ReminderSent")] Assignment assignment)
		{
			if (ModelState.IsValid)
			{
				db.Assignments.Add(assignment);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(assignment);
		}

		// GET: Assignments/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Assignment assignment = db.Assignments.Find(id);
			if (assignment == null)
			{
				return HttpNotFound();
			}
			return View(assignment);
		}

		// POST: Assignments/Edit/5
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,Title,Description,DueDate,ReminderSent")] Assignment assignment)
		{
			if (ModelState.IsValid)
			{
				db.Entry(assignment).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(assignment);
		}

		// GET: Assignments/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Assignment assignment = db.Assignments.Find(id);
			if (assignment == null)
			{
				return HttpNotFound();
			}
			return View(assignment);
		}

		// POST: Assignments/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Assignment assignment = db.Assignments.Find(id);
			db.Assignments.Remove(assignment);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
