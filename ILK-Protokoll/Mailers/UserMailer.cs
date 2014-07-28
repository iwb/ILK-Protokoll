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

		public virtual void SendNewAssignment(Assignment assignment)
		{
			ViewBag.User = assignment.Owner;
			ViewBag.Assignment = assignment;
			var mail = Populate(x =>
			{
				x.Subject = string.Format("Neue Aufgabe »{0}« im ILK-Protokoll", assignment.Title);
				x.ViewName = "NewAssignment";
				x.To.Add(assignment.Owner.EmailAddress);
			});
			mail.Send();
		}

		public void SendAssignmentReminder(Assignment assignment)
		{
			ViewBag.User = assignment.Owner;
			ViewBag.Assignment = assignment;
			var mail = Populate(x =>
			{
				x.Subject = string.Format("Die Aufgabe »{0}« wird bald fällig", assignment.Title);
				x.ViewName = "AssignmentReminder";
				x.To.Add(assignment.Owner.EmailAddress);
			});
			mail.Send();
		}

		public void SendAssignmentOverdue(Assignment assignment)
		{
			ViewBag.User = assignment.Owner;
			ViewBag.Assignment = assignment;
			var mail = Populate(x =>
			{
				x.Subject = string.Format("Die Aufgabe »{0}« ist überfällig!", assignment.Title);
				x.ViewName = "AssignmentOverdue";
				x.To.Add(assignment.Owner.EmailAddress);
			});
			mail.Send();
		}
	}
}