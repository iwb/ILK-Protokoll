using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
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
			Entities = _dbSet.Include(ep => ep.Attachments).OrderByDescending(x => x.Selected).ThenBy(x => x.LastPresentation);
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

			EmployeePresentation presentation = _dbSet.Include(ep => ep.Attachments).First(ep => ep.ID == id);
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

		public override ActionResult _Delete(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			EmployeePresentation ep = _dbSet.Find(id.Value);
			if (ep == null)
				return HttpNotFound();

			var linkedFiles = db.Attachments.Include(a => a.Uploader).Where(a => a.EmployeePresentationID == id);

			foreach (var file in linkedFiles)
			{
				file.EmployeePresentationID = null;
				file.Deleted = file.Deleted ?? DateTime.Now;
			}
			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var msg = ErrorMessageFromException(e);
				return HTTPStatus(500, msg);
			}

			ep.Attachments.Clear();
			_dbSet.Remove(_dbSet.Find(id));
			db.SaveChanges();

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}
	}
}