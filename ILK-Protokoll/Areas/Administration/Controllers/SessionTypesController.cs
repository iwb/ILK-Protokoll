using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	public class SessionTypesController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.AdminStyle = "active";
			ViewBag.ASTStyle = "active";
		}

		// GET: Administration/SessionTypes
		public ActionResult Index()
		{
			return View(db.SessionTypes.ToList());
		}

		// GET: Administration/SessionTypes/Details/5
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

		// POST: Administration/SessionTypes/Create
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

		// GET: Administration/SessionTypes/Edit/5
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

		// POST: Administration/SessionTypes/Edit/5
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

		// GET: Administration/SessionTypes/Delete/5
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

		// POST: Administration/SessionTypes/Delete/5
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