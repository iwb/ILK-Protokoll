using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Principal;
using System.Web.Mvc;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	public class UserController : BaseController
	{
		// GET: Administration/User

		private const string DomainName = "iwbmuc";

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
				user.IsActive = user == GetCurrentUser();

			const string addgroupName = "ILK"; // Benutzer dieser Gruppe werden hinzugefügt

			using (var context = new PrincipalContext(ContextType.Domain, DomainName))
			using (var userp = new UserPrincipal(context))
			using (var searcher = new PrincipalSearcher(userp))
			//using (GroupPrincipal adEmployees = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, allGroupName))
			{
				PrincipalSearchResult<Principal> adEmployees = searcher.FindAll();

				Dictionary<Guid, Principal> employees = adEmployees.Where(p => p.Guid != null).ToDictionary(p => p.Guid.Value);


				// Benutzer, zu denen eine GUID gespeichert ist, werden zuerst synchronisiert, da die Übereinstimmung garantiert richtig ist
				foreach (User user in myusers.Where(u => u.Guid != Guid.Empty))
				{
					Principal p;
					if (employees.TryGetValue(user.Guid, out p))
					{
						user.ShortName = p.SamAccountName;
						user.LongName = p.DisplayName;
						employees.Remove(user.Guid);
					}
				}

				// Als zweites wird über das Namenskürzel synchronisiert. Die User ohen GUID bekommen hier eine GUID.
				foreach (User user in myusers.Where(u => u.Guid == Guid.Empty))
				{
					Principal iwbuser = employees.Values.SingleOrDefault(u => u.SamAccountName == user.ShortName);

					if (iwbuser != null && iwbuser.Guid != null)
					{
						user.Guid = iwbuser.Guid.Value;
						user.LongName = iwbuser.DisplayName;
						employees.Remove(user.Guid);
					}
				}

				// Schließlich werden neue User in die Datenbank importiert.
				using (GroupPrincipal ilks = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, addgroupName))
				{
					if (ilks == null)
					{
						return new HttpStatusCodeResult(
							HttpStatusCode.InternalServerError,
							string.Format("Die Gruppe \"{0}\" wurde nicht gefunden.", addgroupName)
							);
					}
					// Der Benutzer "TerminILK" ist hier nicht angebracht und wird über diese GUID entfernt.
					var terminilkGuid = new Guid("{b5f9a1c7-ba25-4902-88a0-ffe59fae893a}");

					foreach (Principal adIlk in ilks.GetMembers(true).Where(p => p.Guid != terminilkGuid))
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
			db.SaveChanges();

			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		public static User GetUser(DataContext db, IPrincipal userPrincipal)
		{
			string fullName = userPrincipal.Identity.Name;
			string shortName = fullName.Split('\\').Last();

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
						user.IsActive = true;
					}
					db.SaveChanges();
					return user;
				}
			}
		}

		private static User CreateUserFromADUser(Principal aduser)
		{
			return new User
			{
				Guid = aduser.Guid ?? Guid.Empty,
				ShortName = aduser.SamAccountName,
				LongName = aduser.DisplayName,
				IsActive = true
			};
		}
	}
}