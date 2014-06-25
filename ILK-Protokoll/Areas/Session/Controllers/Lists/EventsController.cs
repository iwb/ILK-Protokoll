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
		public ActionResult Create([Bind(Include = "StartDate,EndDate,Time,Place,Description,Organizer")] Event ev)
		{
			if (ModelState.IsValid)
			{
				db.L_Events.Add(ev);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(ev);
		}

		// POST: Session/Events/Edit
		public ActionResult _BeginEdit(int eventID)
		{
			var ev = db.L_Events.Find(eventID);
			if (ev == null)
				return HttpNotFound();

			return View("_Edit", ev);
		}

		//// POST: Session/Events/Edit/5
		//// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		//// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Edit([Bind(Include = "ID,StartDate,EndDate,Time,Place,Description,Created")] Event @event)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		db.Entry(@event).State = EntityState.Modified;
		//		db.SaveChanges();
		//		return RedirectToAction("Index");
		//	}
		//	return View(@event);
		//}

		// POST: Session/Events/Delete
		[HttpPost]
		public ActionResult _Delete(int? eventID)
		{
			if (eventID == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Event ev = db.L_Events.Find(eventID.Value);
			if (ev == null)
			{
				return HttpNotFound();
			}
			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}
	}
}