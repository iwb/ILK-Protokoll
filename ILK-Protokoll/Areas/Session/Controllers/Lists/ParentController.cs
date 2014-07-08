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

		public PartialViewResult _List()
		{
			return PartialView(_dbSet.ToList());
		}

		public PartialViewResult _CreateForm()
		{
			return PartialView("_CreateForm", new TModel());
		}

		public PartialViewResult _FetchRow(int id)
		{
			return PartialView("_Row", _dbSet.Find(id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _Create([Bind(Include = "StartDate,EndDate,Time,Place,Description,Organizer")] TModel ev)
		{
			if (!ModelState.IsValid)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			_dbSet.Add(ev);
			db.SaveChanges();
			return PartialView("_Row", ev);
		}

		public ActionResult _BeginEdit(int id)
		{
			var ev = _dbSet.Find(id);
			if (ev == null)
				return HttpNotFound();

			return PartialView("_Edit", ev);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _Edit([Bind(Include = "ID,StartDate,EndDate,Time,Place,Description,Organizer")] TModel input)
		{
			if (!ModelState.IsValid)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			db.Entry(input).State = EntityState.Modified;
			db.SaveChanges();
			return PartialView("_Row", input);
		}

		[HttpPost]
		public ActionResult _Delete(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			var ev = _dbSet.Find(id.Value);
			if (ev == null)
				return HttpNotFound();

			_dbSet.Remove(_dbSet.Find(id));
			db.SaveChanges();

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}
	}

// ReSharper restore Mvc.PartialViewNotResolved
}