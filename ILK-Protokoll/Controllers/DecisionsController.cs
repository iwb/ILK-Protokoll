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
	public class DecisionsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.DecisionStyle = "active";
		}

		// GET: Decisions
		public ActionResult Index()
		{
			var decisions = db.Decisions.Include(d => d.OriginTopic);
			return View(decisions.ToList());
		}

		// GET: Decisions/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Decision decision = db.Decisions.Find(id);
			if (decision == null)
			{
				return HttpNotFound();
			}
			return null;
		}

		// GET: Decisions/Create
		public ActionResult Create()
		{
			ViewBag.ID = new SelectList(db.Topics, "ID", "Title");
			return null;
		}

		// POST: Decisions/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,Name")] Decision decision)
		{
			if (ModelState.IsValid)
			{
				db.Decisions.Add(decision);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.ID = new SelectList(db.Topics, "ID", "Title", decision.ID);
			return null;
		}

		// GET: Decisions/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Decision decision = db.Decisions.Find(id);
			if (decision == null)
			{
				return HttpNotFound();
			}
			ViewBag.ID = new SelectList(db.Topics, "ID", "Title", decision.ID);
			return null;
		}

		// POST: Decisions/Edit/5
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,Name")] Decision decision)
		{
			if (ModelState.IsValid)
			{
				db.Entry(decision).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.ID = new SelectList(db.Topics, "ID", "Title", decision.ID);
			return null;
		}

		// GET: Decisions/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Decision decision = db.Decisions.Find(id);
			if (decision == null)
			{
				return HttpNotFound();
			}
			return null;
		}

		// POST: Decisions/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Decision decision = db.Decisions.Find(id);
			db.Decisions.Remove(decision);
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
