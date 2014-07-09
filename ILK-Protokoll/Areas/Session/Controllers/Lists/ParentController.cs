﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
// ReSharper disable Mvc.PartialViewNotResolved
	public class ParentController<TModel> : SessionBaseController
		where TModel : BaseItem, new()
	{
		protected DbSet<TModel> _dbSet;

		public virtual PartialViewResult _List()
		{
			ViewBag.LastSession = GetSession().SessionType.LastDate ?? DateTime.MinValue;
			return PartialView(_dbSet.ToList());
		}

		public virtual PartialViewResult _CreateForm()
		{
			return PartialView("_CreateForm", new TModel());
		}

		public virtual PartialViewResult _FetchRow(int id)
		{
			return PartialView("_Row", _dbSet.Find(id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult _Create([Bind(Exclude = "ID, Created")] TModel ev)
		{
			if (!ModelState.IsValid)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var row = _dbSet.Create();
			TryUpdateModel(row, "", null, new[] {"Created"});
			_dbSet.Add(row);
			db.SaveChanges();
			return PartialView("_Row", row);
		}

		public virtual ActionResult _BeginEdit(int id)
		{
			TModel ev = _dbSet.Find(id);
			if (ev == null)
				return HttpNotFound();

			return PartialView("_Edit", ev);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult _Edit([Bind(Exclude = "Created")] TModel input)
		{
			if (!ModelState.IsValid)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			// Get the object from the database to enble lasy loading.
			var row = _dbSet.Find(input.ID);
			TryUpdateModel(row, "", null, new[] {"Created"});
			db.SaveChanges();
			return PartialView("_Row", row);
		}

		[HttpPost]
		public virtual ActionResult _Delete(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			TModel ev = _dbSet.Find(id.Value);
			if (ev == null)
				return HttpNotFound();

			_dbSet.Remove(_dbSet.Find(id));
			db.SaveChanges();

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}
	}

// ReSharper restore Mvc.PartialViewNotResolved
}