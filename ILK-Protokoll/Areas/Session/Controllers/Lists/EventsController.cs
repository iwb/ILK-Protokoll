using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class EventsController : ParentController<Event>
	{
		// GET: Session/Events
		public PartialViewResult Index()
		{
			return PartialView(db.L_Events.ToList());
		}

		// GET: Session/Events/Create
		public PartialViewResult _CreateForm()
		{
			return PartialView("_CreateForm", new Event());
		}

		// POST: Session/Events/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,StartDate,EndDate,Time,Place,Description,Created")] Event @event)
		{
			if (ModelState.IsValid)
			{
				db.L_Events.Add(@event);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(@event);
		}

		// GET: Session/Events/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Event @event = db.L_Events.Find(id);
			if (@event == null)
			{
				return HttpNotFound();
			}
			return View(@event);
		}

		// POST: Session/Events/Edit/5
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,StartDate,EndDate,Time,Place,Description,Created")] Event @event)
		{
			if (ModelState.IsValid)
			{
				db.Entry(@event).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(@event);
		}

		// GET: Session/Events/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Event @event = db.L_Events.Find(id);
			if (@event == null)
			{
				return HttpNotFound();
			}
			return View(@event);
		}

		// POST: Session/Events/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Event @event = db.L_Events.Find(id);
			db.L_Events.Remove(@event);
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