﻿using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ILK_Protokoll.Controllers
{
	public class BaseController : Controller
	{
		protected readonly DataContext db = new DataContext();

		protected User GetCurrentUser()
		{
			string username = User.Identity.Name.Split('\\').Last();
			return db.Users.Single(x => x.Name.Equals(username, StringComparison.CurrentCultureIgnoreCase));
		}
	}
}