using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	public class UserController : BaseController
	{
		// GET: Administration/User
		public ActionResult Index()
		{
			List<User> users = db.Users.ToList();
			return View("Index", users);
		}

		// GET: Administration/User/Sync
		public ActionResult Sync()
		{
			List<User> myusers = db.Users.ToList();
			Dictionary<Guid?, Principal> adusers = new Dictionary<Guid?, Principal>();

			const string groupName = "ILK";

			using (var ctx = new PrincipalContext(ContextType.Domain, "iwbmuc"))
			using (GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, groupName))
			{
				if (grp == null)
				{
					return new HttpStatusCodeResult(
						HttpStatusCode.InternalServerError,
						string.Format("Die Gruppe \"{0}\" wurde nicht gefunden.", groupName)
						);
				}

				adusers = grp.GetMembers(true).ToDictionary(p => p.Guid);
			}

			//foreach (var user in myusers)
			//	  user.IsActive = false;

			foreach (var user in myusers.Where(u => u.Guid != Guid.Empty))
			{
				Principal p;
				if (adusers.TryGetValue(user.Guid, out p))
				{
					user.ShortName = p.SamAccountName;
					user.LongName = p.DisplayName;
					user.IsActive = true;
					adusers.Remove(p.Guid);
				}
			}

			foreach (var user in myusers.Where(u => u.Guid == Guid.Empty))
			{
				var p = adusers.Values.SingleOrDefault(u => u.SamAccountName == user.ShortName);
				if (p != null)
				{
					user.Guid = p.Guid.Value;
					user.LongName = p.DisplayName;
					user.IsActive = true;
					adusers.Remove(p.Guid);
				}
			}

			foreach (var p in adusers.Values)
			{
				db.Users.Add(new User
				{
					Guid = p.Guid.Value,
					ShortName = p.SamAccountName,
					LongName = p.DisplayName,
					IsActive = true
				});
			}
			db.SaveChanges();

			return Index();
		}
	}
}