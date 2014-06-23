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
	public class ReportsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.ReportStyle = "active";
		}

		// GET: Reports
		public ActionResult Index()
		{
			return View(db.Reports.ToList());
		}

		// GET: Reports/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Report report = db.Reports.Find(id);
			if (report == null)
			{
				return HttpNotFound();
			}
			return View(report);
		}

		// GET: Reports/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Reports/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,TopicID,Name,Created")] Report report)
		{
			if (ModelState.IsValid)
			{
				db.Reports.Add(report);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(report);
		}

		// GET: Reports/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Report report = db.Reports.Find(id);
			if (report == null)
			{
				return HttpNotFound();
			}
			return View(report);
		}

		// POST: Reports/Edit/5
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,TopicID,Name,Created")] Report report)
		{
			if (ModelState.IsValid)
			{
				db.Entry(report).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(report);
		}

		// GET: Reports/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Report report = db.Reports.Find(id);
			if (report == null)
			{
				return HttpNotFound();
			}
			return View(report);
		}

		// POST: Reports/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Report report = db.Reports.Find(id);
			db.Reports.Remove(report);
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
