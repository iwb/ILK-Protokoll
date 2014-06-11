using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ILK_Protokoll.Controllers;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	public class ListController : BaseController
	{
		// GET: Administration/List
		public ActionResult Index()
		{
			return View();
		}
	}
}
