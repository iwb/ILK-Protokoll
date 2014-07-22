using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EntityFramework.Extensions;
using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	// ReSharper disable Mvc.PartialViewNotResolved
	public class ParentController<TModel> : SessionBaseController
		where TModel : BaseItem, new()
	{
		protected DbSet<TModel> _dbSet;
		private IQueryable<TModel> _entities;

		private readonly TimeSpan EditDuration = TimeSpan.FromMinutes(5);

		protected IQueryable<TModel> Entities
		{
			get { return _entities ?? _dbSet; }
			set { _entities = value; }
		}

		public virtual PartialViewResult _List()
		{
			var cutoff = DateTime.Now - EditDuration;
			_dbSet.Where(e => e.LockTime  < cutoff).Update(e => new TModel(){ LockSessionID = null });

			return PartialView(Entities.ToList());
		}

		public virtual PartialViewResult _CreateForm()
		{
			return PartialView("_CreateForm", new TModel());
		}

		public virtual PartialViewResult _FetchRow(int id)
		{
			var row = Entities.Single(m => m.ID == id);
			if (row.LockSessionID == GetSession().ID)
			{
				row.LockSessionID = null;
				db.SaveChanges();
			}
				
			return PartialView("_Row", row);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult _Create([Bind(Exclude = "ID, Created")] TModel ev)
		{
			if (!ModelState.IsValid)
			{
				var message = ErrorMessageFromModelState();
				return HTTPStatus(422, message);
			}

			var row = _dbSet.Create();
			TryUpdateModel(row, "", null, new[] { "Created" });
			_dbSet.Add(row);

			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var message = ErrorMessageFromException(e);
				return HTTPStatus(500, message);
			}
			return _FetchRow(row.ID);
		}

		public virtual ActionResult _BeginEdit(int id)
		{
			TModel ev = Entities.Single(m => m.ID == id);
			if (ev == null)
				return HttpNotFound();
			else if (ev.LockSessionID.HasValue && ev.LockSessionID != GetSession().ID)
				return HTTPStatus(HttpStatusCode.Conflict, "Der Listeneintrag ist gesperrt.");

			ev.LockSessionID = GetSession().ID;
			ev.LockTime = DateTime.Now;
			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var message = ErrorMessageFromException(e);
				return HTTPStatus(500, message);
			}

			return PartialView("_Edit", ev);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult _Edit([Bind(Exclude = "Created")] TModel input)
		{
			if (!ModelState.IsValid)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			// Get the object from the database to enable lazy loading.
			var row = Entities.Single(m => m.ID == input.ID);

			if (row.LockSessionID == GetSession().ID)
			{
				TryUpdateModel(row, "", null, new[] {"Created"});
				row.LockSessionID = null;
			}

			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var message = ErrorMessageFromException(e);
				return HTTPStatus(500, message);
			}
			return _FetchRow(input.ID);
		}

		[HttpPost]
		public virtual ActionResult _Delete(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			TModel ev = _dbSet.Find(id.Value);
			if (ev == null)
				return HttpNotFound();
			else if (ev.LockSessionID != null)
				return HTTPStatus(HttpStatusCode.Conflict, "Der Eintrag wird gerade bearbeitet!");

			_dbSet.Remove(_dbSet.Find(id));
			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var message = ErrorMessageFromException(e);
				return HTTPStatus(500, message);
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}
	}

	// ReSharper restore Mvc.PartialViewNotResolved
}