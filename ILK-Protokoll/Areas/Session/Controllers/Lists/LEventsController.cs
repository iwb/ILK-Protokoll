using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LEventsController : ParentController<Event>
	{
		// GET: Session/Events
		public PartialViewResult _List()
		{
			return PartialView(db.LEvents.ToList());
		}

		// GET: Session/Events/Create
		public PartialViewResult _CreateForm()
		{
			return PartialView("_CreateForm", new Event());
		}

		// GET: Session/Events/FetchRow
		public PartialViewResult _FetchRow(int eventID)
		{
			return PartialView("_Row", db.LEvents.Find(eventID));
		}

		// POST: Session/Events/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _Create([Bind(Include = "StartDate,EndDate,Time,Place,Description,Organizer")] Event ev)
		{
			if (!ModelState.IsValid)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			db.LEvents.Add(ev);
			db.SaveChanges();
			return PartialView("_Row", ev);
		}

		// POST: Session/Events/Edit
		public ActionResult _BeginEdit(int eventID)
		{
			var ev = db.LEvents.Find(eventID);
			if (ev == null)
				return HttpNotFound();

			return PartialView("_Edit", ev);
		}

		// POST: Session/Events/Edit
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _Edit([Bind(Include = "ID,StartDate,EndDate,Time,Place,Description,Organizer")] Event input)
		{
			if (!ModelState.IsValid)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			db.Entry(input).State = EntityState.Modified;
			db.SaveChanges();
			return PartialView("_Row", input);
		}

		// POST: Session/Events/Delete
		[HttpPost]
		public ActionResult _Delete(int? eventID)
		{
			if (eventID == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			Event ev = db.LEvents.Find(eventID.Value);
			if (ev == null)
				return HttpNotFound();

			db.LEvents.Remove(db.LEvents.Find(eventID));
			db.SaveChanges();

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}
	}
}