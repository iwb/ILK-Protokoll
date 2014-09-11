using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models.Lists;
using ILK_Protokoll.util;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LConferencesController : ParentController<Conference>
	{
		public LConferencesController()
		{
			_dbSet = db.LConferences;
			Entities = _dbSet.OrderBy(conf => conf.StartDate).ThenBy(conf => conf.EndDate);
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

		public override ActionResult Download(int id)
		{
			Debug.Assert(Request.Url != null, "Request.Url != null");

			var conf = _dbSet.Find(id);
			if (conf.GUID == null)
			{
				conf.GUID = Guid.NewGuid();
				db.SaveChanges();
			}

			var ical = CreateCalendarEvent("Konferenz: " + conf.Description.Shorten(50),
				conf.Description + "\r\n\r\nhttp://" + Request.Url.Authority + Url.Content("~/ViewLists#conference_table"),
				conf.StartDate, conf.EndDate.AddDays(1),
				"", conf.GUID.ToString(), true);

			return Content(ical, "text/calendar", Encoding.UTF8);
		}
	}
}