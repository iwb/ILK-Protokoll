using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models.Lists;
using ILK_Protokoll.util;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LIndustryProjectsController : ParentController<IndustryProject>
	{
		public LIndustryProjectsController()
		{
			_dbSet = db.LIndustryProject;
			Entities = _dbSet.OrderBy(e => e.StartDate).ThenBy(e => e.Name);
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