using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
			User user = db.Users.SingleOrDefault(x => x.ShortName.Equals(username, StringComparison.CurrentCultureIgnoreCase));
			if (user == null)
			{
				user = new User(username);
				db.Users.Add(user);
				db.SaveChanges();
			}
			return user;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();

			base.Dispose(disposing);
		}

		protected override void Initialize(RequestContext requestContext)
		{
			HttpContextBase Context = requestContext.HttpContext;

			Context.Response.SuppressDefaultCacheControlHeader = true;

			Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
			Context.Response.Headers["Pragma"] = "no-cache";
			Context.Response.Headers["Expires"] = "0";

			base.Initialize(requestContext);
		}
	}
}