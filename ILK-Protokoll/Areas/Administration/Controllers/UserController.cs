using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Principal;
using System.Web.Mvc;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Mailers;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	public class UserController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.AdminStyle = "active";
			ViewBag.AUserStyle = "active";
		}

		private const string DomainName = "iwbmuc";
		private readonly string[] _authorizeGroups = {"ILK", "ILK-Proto"}; // Benutzer dieser Gruppen werden automatisch hinzugefügt

		// GET: Administration/User
		public ActionResult Index()
		{
			List<User> users = db.Users.OrderByDescending(u => u.IsActive).ThenBy(u => u.ShortName).ToList();
			return View("Index", users);
		}

		// GET: Administration/User/Sync
		public ActionResult _Sync()
		{
			List<User> myusers = db.Users.ToList();
			// Zunächst alle Benutzer (außer dem aktuellen Benutzer) auf inakitv setzen.
			foreach (User user in myusers)
				user.IsActive = user.Equals(GetCurrentUser());

			using (var context = new PrincipalContext(ContextType.Domain, DomainName))
			using (var userp = new UserPrincipal(context))
			using (var searcher = new PrincipalSearcher(userp))
			{
				PrincipalSearchResult<Principal> adEmployees = searcher.FindAll();

				Dictionary<Guid, UserPrincipal> employees = adEmployees.Where(p => p.Guid != null).Cast<UserPrincipal>().ToDictionary(p => p.Guid.Value);


				// Benutzer, zu denen eine GUID gespeichert ist, werden zuerst synchronisiert, da die Übereinstimmung garantiert richtig ist
				foreach (User user in myusers.Where(u => u.Guid != Guid.Empty))
				{
					UserPrincipal iwbuser;
					if (employees.TryGetValue(user.Guid, out iwbuser))
					{
						user.ShortName = iwbuser.SamAccountName;
						user.LongName = iwbuser.DisplayName;
						user.EmailAddress = iwbuser.EmailAddress;
						employees.Remove(user.Guid);
					}
				}

				// Als zweites wird über das Namenskürzel synchronisiert. Die User ohne GUID bekommen hier eine GUID.
				foreach (User user in myusers.Where(u => u.Guid == Guid.Empty))
				{
					UserPrincipal iwbuser = employees.Values.SingleOrDefault(u => u.SamAccountName == user.ShortName);
					if (iwbuser != null && iwbuser.Guid != null)
					{
						user.Guid = iwbuser.Guid.Value;
						user.LongName = iwbuser.DisplayName;
						user.EmailAddress = iwbuser.EmailAddress;
						employees.Remove(user.Guid);
					}
				}

				// Schließlich werden neue User in die Datenbank importiert.
				foreach (var group in _authorizeGroups)
				{
					using (GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, group))
					{
						if (groupPrincipal == null)
						{
							return HTTPStatus(HttpStatusCode.InternalServerError,
								string.Format("Die Gruppe \"{0}\" wurde nicht gefunden.", group));
						}
						// Der Benutzer "TerminILK" ist hier nicht angebracht und wird über diese GUID entfernt.
						var terminilkGuid = new Guid("{b5f9a1c7-ba25-4902-88a0-ffe59fae893a}");

						foreach (var adIlk in groupPrincipal.GetMembers(true).Cast<UserPrincipal>().Where(p => p.Guid != terminilkGuid))
						{
							User ilk = myusers.SingleOrDefault(u => u.Guid == adIlk.Guid);
							if (ilk == null)
							{
								ilk = CreateUserFromADUser(adIlk);
								db.Users.Add(ilk);
							}
							ilk.IsActive = true;
						}
					}
				}
			}
			db.SaveChanges();

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		public static User GetUser(DataContext db, IPrincipal userPrincipal)
		{
			string fullName = userPrincipal.Identity.Name;
			string shortName = fullName.Split('\\').Last();

			if (string.IsNullOrEmpty(fullName))
				return new User() {ID = 0, ShortName = "xx", LongName = "Anonymous User"};

			using (var context = new PrincipalContext(ContextType.Domain, DomainName))
			using (UserPrincipal aduser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, fullName))
			{
				if (aduser == null || aduser.Guid == null)
					throw new AuthenticationException("Keine GUID im AD gefunden.");

				User user = db.Users.FirstOrDefault(u => u.Guid == aduser.Guid.Value);
				if (user != null)
				{
					if (!user.IsActive)
					{
						user.IsActive = true;
						db.SaveChanges();
					}
					return user;
				}
				else
				{
					user = db.Users.SingleOrDefault(u => u.ShortName.Equals(shortName, StringComparison.CurrentCultureIgnoreCase));
					if (user == null)
					{
						user = CreateUserFromADUser(aduser);
						db.Users.Add(user);
					}
					else
					{
						user.Guid = aduser.Guid.Value;
						user.LongName = aduser.DisplayName;
						user.EmailAddress = aduser.EmailAddress;
						user.IsActive = true;
					}
					db.SaveChanges();
					return user;
				}
			}
		}

		public static User CreateUserFromShortName(string samname)
		{
			using (var context = new PrincipalContext(ContextType.Domain, DomainName))
			using (UserPrincipal aduser = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, samname))
			{
				if (aduser == null || aduser.Guid == null)
					throw new AuthenticationException("Keine GUID im AD gefunden.");

				return new User
				{
					Guid = aduser.Guid.Value,
					ShortName = aduser.SamAccountName,
					LongName = aduser.DisplayName,
					EmailAddress = aduser.EmailAddress,
					IsActive = true
				};
			}
		}

		private static User CreateUserFromADUser(UserPrincipal aduser)
		{
			var u = new User
			{
				Guid = aduser.Guid ?? Guid.Empty,
				ShortName = aduser.SamAccountName,
				LongName = aduser.DisplayName,
				EmailAddress = aduser.EmailAddress,
				IsActive = true
			};
			new UserMailer().SendWelcome(u);
			return u;
		}
	}
}