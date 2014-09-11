﻿using System.Data.Entity;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LHolidaysController : ParentController<Holiday>
	{
		public LHolidaysController()
		{
			_dbSet = db.LHolidays;
			Entities = _dbSet.Include(h => h.Person);
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
	}
}