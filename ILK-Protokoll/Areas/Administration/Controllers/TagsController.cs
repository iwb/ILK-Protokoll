using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	[DisplayName("Tags")]
	public class TagsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.AdminStyle = "active";
			ViewBag.ATagStyle = "active";
		}

		// GET: Administration/Tags
		public ActionResult Index()
		{
			return View(db.Tags.ToList());
		}

		// POST: Administration/Tags/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Tag tag)
		{
			if (ModelState.IsValid)
			{
				db.Tags.Add(tag);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return Index();
		}

		// GET: Administration/Tags/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			Tag tag = db.Tags.Find(id);
			if (tag == null)
				return HttpNotFound();
			return View(tag);
		}

		// POST: Administration/Tags/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Tag tag)
		{
			if (ModelState.IsValid)
			{
				db.Entry(tag).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(tag);
		}

		// GET: Administration/Tags/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			Tag tag = db.Tags.Find(id);
			if (tag == null)
				return HttpNotFound();
			return View(tag);
		}

		// POST: Administration/Tags/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Tag tag = db.Tags.Find(id);
			db.Tags.Remove(tag);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}