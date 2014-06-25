using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class RecordsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.RecordStyle = "active";
		}

		// GET: Records
		public ActionResult Index()
		{
			return View(db.Records.ToList());
		}

		// GET: Records/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Record Record = db.Records.Find(id);
			if (Record == null)
			{
				return HttpNotFound();
			}
			return View(Record);
		}

		// GET: Records/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Records/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,TopicID,Name,Created")] Record Record)
		{
			if (ModelState.IsValid)
			{
				db.Records.Add(Record);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(Record);
		}

		// GET: Records/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Record Record = db.Records.Find(id);
			if (Record == null)
			{
				return HttpNotFound();
			}
			return View(Record);
		}

		// POST: Records/Edit/5
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,TopicID,Name,Created")] Record Record)
		{
			if (ModelState.IsValid)
			{
				db.Entry(Record).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(Record);
		}

		// GET: Records/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Record Record = db.Records.Find(id);
			if (Record == null)
			{
				return HttpNotFound();
			}
			return View(Record);
		}

		// POST: Records/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Record Record = db.Records.Find(id);
			db.Records.Remove(Record);
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