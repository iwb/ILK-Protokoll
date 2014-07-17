using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
// ReSharper disable Mvc.PartialViewNotResolved
	public class ParentController<TModel> : SessionBaseController
		where TModel : BaseItem, new()
	{
		protected DbSet<TModel> _dbSet;
		private IQueryable<TModel> _entities;

		protected IQueryable<TModel> Entities
		{
			get { return _entities ?? _dbSet; }
			set { _entities = value; }
		}

		public virtual PartialViewResult _List()
		{
			return PartialView(Entities.ToList());
		}

		public virtual PartialViewResult _CreateForm()
		{
			return PartialView("_CreateForm", new TModel());
		}

		public virtual PartialViewResult _FetchRow(int id)
		{
			return PartialView("_Row", Entities.Single(m => m.ID == id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult _Create([Bind(Exclude = "ID, Created")] TModel ev)
		{
			if (!ModelState.IsValid)
			{
				StringBuilder msg = new StringBuilder();
				foreach (var kvp in ModelState)
					foreach (var error in kvp.Value.Errors)
						msg.AppendFormat("{1} <br />", kvp.Key, error.ErrorMessage);

				Response.Clear();
				Response.StatusCode = 422;
				Response.StatusDescription = "Unprocessable Entity";
				return Content(msg.ToString());
			}

			var row = _dbSet.Create();
			TryUpdateModel(row, "", null, new[] {"Created"});
			_dbSet.Add(row);
			db.SaveChanges();
			return _FetchRow(row.ID);
		}

		public virtual ActionResult _BeginEdit(int id)
		{
			TModel ev = Entities.Single(m => m.ID == id);
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
			var row = Entities.Single(m => m.ID == input.ID);
			TryUpdateModel(row, "", null, new[] {"Created"});
			db.SaveChanges();
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

			_dbSet.Remove(_dbSet.Find(id));
			db.SaveChanges();

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}
	}

// ReSharper restore Mvc.PartialViewNotResolved
}