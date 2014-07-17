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

		public virtual MvcMailMessage NewAssignment(Assignment a)
		{
			ViewBag.User = a.Owner;
			ViewBag.Assignment = a;
			return Populate(x =>
			{
				x.Subject = string.Format("Neue Aufgabe »{0}« im ILK-Protokoll", a.Title);
				x.ViewName = "NewAssignment";
				x.To.Add(a.Owner.EmailAddress);
			});
		}
	}
}