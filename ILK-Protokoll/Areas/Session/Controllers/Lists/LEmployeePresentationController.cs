using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LEmployeePresentationsController : ParentController<EmployeePresentation>
	{
		public LEmployeePresentationsController()
		{
			_dbSet = db.LEmployeePresentations;
		}

		public override PartialViewResult _CreateForm()
		{
			ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
			return base._CreateForm();
		}

		public override ActionResult _BeginEdit(int id)
		{
			ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
			return base._BeginEdit(id);
		}

		public ActionResult Edit(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			EmployeePresentation presentation = _dbSet.Find(id);
			if (presentation == null)
				return HttpNotFound();

			ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
			return View(presentation);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult Edit([Bind(Exclude = "Created")] EmployeePresentation input)
		{
			if (ModelState.IsValid)
			{
				db.Entry(input).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index", "Lists", new {Area = "Session"});
			}
			ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
			return View(input);
		}
	}
}