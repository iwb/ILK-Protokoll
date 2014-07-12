﻿using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LIlkDaysController : ParentController<IlkDay>
	{
		public LIlkDaysController()
		{
			_dbSet = db.LIlkDays;
		}

		public override PartialViewResult _CreateForm()
		{
			ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
			ViewBag.SessionTypeList = new SelectList(db.SessionTypes, "ID", "Name");

			return base._CreateForm();
		}

		public override ActionResult _BeginEdit(int id)
		{
			ViewBag.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
			ViewBag.SessionTypeList = new SelectList(db.SessionTypes, "ID", "Name");
			return base._BeginEdit(id);
		}
	}
}