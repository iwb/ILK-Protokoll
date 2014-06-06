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
	public class SessionTypesController : BaseController
	{

		// GET: SessionTypes
		public ActionResult Index()
		{
			return View(db.SessionTypes.ToList());
		}

		// GET: SessionTypes/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SessionType sessionType = db.SessionTypes.Find(id);
			if (sessionType == null)
			{
				return HttpNotFound();
			}
			return View(sessionType);
		}

		// GET: SessionTypes/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: SessionTypes/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,Name")] SessionType sessionType)
		{
			if (ModelState.IsValid)
			{
				db.SessionTypes.Add(sessionType);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(sessionType);
		}

		// GET: SessionTypes/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SessionType sessionType = db.SessionTypes.Find(id);
			if (sessionType == null)
			{
				return HttpNotFound();
			}
			return View(sessionType);
		}

		// POST: SessionTypes/Edit/5
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,Name")] SessionType sessionType)
		{
			if (ModelState.IsValid)
			{
				db.Entry(sessionType).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(sessionType);
		}

		// GET: SessionTypes/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SessionType sessionType = db.SessionTypes.Find(id);
			if (sessionType == null)
			{
				return HttpNotFound();
			}
			return View(sessionType);
		}

		// POST: SessionTypes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			SessionType sessionType = db.SessionTypes.Find(id);
			db.SessionTypes.Remove(sessionType);
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
