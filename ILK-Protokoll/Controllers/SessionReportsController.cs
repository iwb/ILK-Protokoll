using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class SessionReportsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.SessionReportStyle = "active";
		}

		// GET: SessionReports
		public ActionResult Index()
		{
			return View(db.SessionReports.ToList());
		}

		// GET: SessionReports/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SessionReport SessionReport = db.SessionReports.Find(id);
			if (SessionReport == null)
			{
				return HttpNotFound();
			}
			return null;
		}

		// GET: SessionReports/Create
		public ActionResult Create()
		{
			return null;
		}

		// POST: SessionReports/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,TopicID,Name,Created")] SessionReport SessionReport)
		{
			if (ModelState.IsValid)
			{
				db.SessionReports.Add(SessionReport);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return null;
		}

		// GET: SessionReports/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SessionReport SessionReport = db.SessionReports.Find(id);
			if (SessionReport == null)
			{
				return HttpNotFound();
			}
			return null;
		}

		// POST: SessionReports/Edit/5
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,TopicID,Name,Created")] SessionReport SessionReport)
		{
			if (ModelState.IsValid)
			{
				db.Entry(SessionReport).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return null;
		}

		// GET: SessionReports/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SessionReport SessionReport = db.SessionReports.Find(id);
			if (SessionReport == null)
			{
				return HttpNotFound();
			}
			return null;
		}

		// POST: SessionReports/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			SessionReport SessionReport = db.SessionReports.Find(id);
			db.SessionReports.Remove(SessionReport);
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