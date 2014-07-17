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

		public virtual MvcMailMessage NewAssignment(Assignment assignment)
		{
			ViewBag.User = assignment.Owner;
			ViewBag.Assignment = assignment;
			return Populate(x =>
			{
				x.Subject = string.Format("Neue Aufgabe »{0}« im ILK-Protokoll", assignment.Title);
				x.ViewName = "NewAssignment";
				x.To.Add(assignment.Owner.EmailAddress);
			});
		}

		public MvcMailMessage AssignmentReminder(Assignment assignment)
		{
			ViewBag.User = assignment.Owner;
			ViewBag.Assignment = assignment;
			return Populate(x =>
			{
				x.Subject = string.Format("Die Aufgabe »{0}« wird bald fällig", assignment.Title);
				x.ViewName = "AssignmentReminder";
				x.To.Add(assignment.Owner.EmailAddress);
			});
		}
	}
}