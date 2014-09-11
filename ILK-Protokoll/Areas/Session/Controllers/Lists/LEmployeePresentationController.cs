using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EntityFramework.Extensions;
using ILK_Protokoll.Areas.Session.Models.Lists;
using ILK_Protokoll.Controllers;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LEmployeePresentationsController : ParentController<EmployeePresentation>
	{
		public LEmployeePresentationsController()
		{
			_dbSet = db.LEmployeePresentations;
			Entities = _dbSet.Include(ep => ep.Attachments).OrderByDescending(x => x.Selected).ThenBy(x => x.LastPresentation);
		}

		public override PartialViewResult _List(bool reporting = false)
		{
			CleanupLocks();

			ViewBag.Reporting = reporting;
			var items = Entities.ToList();
			foreach (var emp in items)
			{
				if (emp.Attachments.Count(a => a.Deleted == null) > 0)
					emp.FileURL = AttachmentsController.GetVirtualPath(emp.Attachments.Where(a => a.Deleted == null).OrderByDescending(a => a.Created).First().ID, Request, db, Url);
			}
			return PartialView(items);
		}

		public override PartialViewResult _CreateForm()
		{
			ViewBag.UserList = CreateUserSelectList();
			return base._CreateForm();
		}

		public override ActionResult _BeginEdit(int id)
		{
			ViewBag.UserList = CreateUserSelectList();
			return base._BeginEdit(id);
		}

		public ActionResult Edit(int? id, string returnURL = null)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			EmployeePresentation presentation = _dbSet.Include(ep => ep.Attachments).Single(ep => ep.ID == id);
			if (presentation == null)
				return HttpNotFound();

			ViewBag.UserList = CreateUserSelectList();
			ViewBag.ReturnURL = returnURL ?? Url.Action("Index", "Lists", new {Area = "Session"});
			return View(presentation);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult Edit([Bind(Exclude = "Created")] EmployeePresentation input, string returnURL = null)
		{
			if (ModelState.IsValid)
			{
				db.Entry(input).State = EntityState.Modified;
				db.SaveChanges();
				if (returnURL == null)
					return RedirectToAction("Index", "Lists", new {Area = "Session"});
				else
					return Redirect(returnURL);
			}
			ViewBag.UserList = CreateUserSelectList();
			ViewBag.ReturnURL = returnURL ?? Url.Action("Index", "Lists", new { Area = "Session" });
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