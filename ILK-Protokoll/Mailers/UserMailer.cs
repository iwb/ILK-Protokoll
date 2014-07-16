using ILK_Protokoll.Models;
using Mvc.Mailer;

namespace ILK_Protokoll.Mailers
{
	public class UserMailer : MailerBase
	{
		public UserMailer()
		{
			MasterName = "_Layout";
		}

		public virtual MvcMailMessage Welcome(User u)
		{
			ViewBag.User = u;
			return Populate(x =>
			{
				x.Subject = "Wilkommen beim ILK-Protokoll";
				x.ViewName = "Welcome";
				x.To.Add(u.EmailAddress);
			});
		}

		public virtual MvcMailMessage NewAssignment(User u, Assignment a)
		{
			ViewBag.User = u;
			ViewBag.Assignment = a;
			return Populate(x =>
			{
				x.Subject = "Neue Aufgabe im ILK-Protokoll";
				x.ViewName = "NewAssignment";
				x.To.Add(u.EmailAddress);
			});
		}
	}
}