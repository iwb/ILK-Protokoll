using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class HomeController : Controller
	{
		private readonly DataContext _db = new DataContext();

		private User GetCurrentUser()
		{
			string username = User.Identity.Name.Split('\\').Last();
			return _db.Users.Single(x => x.Name.Equals(username, StringComparison.CurrentCultureIgnoreCase));
		}

		public ActionResult Index()
		{
			IEnumerable<Topic> to = _db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.TargetSessionType)
				.Include(t => t.Owner)
				.Include(t => t.Votes)
				.Include(t => t.Votes.Select(v => v.Voter))
				.Include(t => t.Comments);
			ViewBag.Topics = to;
			ViewBag.CurrentUser = GetCurrentUser();
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}