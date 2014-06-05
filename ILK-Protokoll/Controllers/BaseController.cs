using System;
using System.Linq;
using System.Web.Mvc;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class BaseController : Controller
	{
		protected readonly DataContext db = new DataContext();

		protected User GetCurrentUser()
		{
			string username = User.Identity.Name.Split('\\').Last();
			User user = db.Users.SingleOrDefault(x => x.Name.Equals(username, StringComparison.CurrentCultureIgnoreCase));
			if (user == null)
			{
				user = new User(username);
				db.Users.Add(user);
				db.SaveChanges();
			}
			return user;
		}
	}
}