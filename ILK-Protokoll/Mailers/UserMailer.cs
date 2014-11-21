using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Hosting;
using ILK_Protokoll.Models;
using Mvc.Mailer;

namespace ILK_Protokoll.Mailers
{
	public class UserMailer : MailerBase
	{
		private readonly string FQDN = "http://" + Dns.GetHostName() + ".iwb.mw.tu-muenchen.de";

		public UserMailer()
		{
// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			MasterName = "~/Views/UserMailer/_Layout.cshtml";
		}

		public virtual void SendWelcome(User u)
		{
			ViewData.Model = u.EmailName;
			ViewBag.Host = FQDN;
			var mail = Populate(x =>
			{
				x.Subject = "Wilkommen beim ILK-Protokoll";
				x.ViewName = "Welcome";
				x.To.Add(u.EmailAddress);
			});
			HostingEnvironment.QueueBackgroundWorkItem(ct => mail.SendAsync());
		}

		public virtual Task SendNewAssignment(Assignment assignment)
		{
			ViewData.Model = assignment;
			ViewBag.Host = FQDN;
			var mail = Populate(x =>
			{
				x.Subject = string.Format("Neue Aufgabe »{0}« im ILK-Protokoll", assignment.Title);
				x.ViewName = "NewAssignment";
				x.To.Add(assignment.Owner.EmailAddress);
			});
			return mail.SendAsync();
		}

		public void SendAssignmentReminder(Assignment assignment)
		{
			ViewData.Model = assignment;
			ViewBag.Host = FQDN;
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
			ViewData.Model = assignment;
			ViewBag.Host = FQDN;
			var mail = Populate(x =>
			{
				x.Subject = string.Format("Die Aufgabe »{0}« ist überfällig!", assignment.Title);
				x.ViewName = "AssignmentOverdue";
				x.To.Add(assignment.Owner.EmailAddress);
			});
			mail.Send();
		}

		public void SendSessionReport(IEnumerable<Topic> topics, SessionReport report)
		{
			ViewData.Model = topics;
			ViewBag.Report = report;
			ViewBag.Host = FQDN;
			var mail = Populate(x =>
			{
				x.Subject = string.Format("Eine Sitzung des Typs »{0}« wurde durchgeführt", report.SessionType.Name);
				x.ViewName = "NewSessionReport";
				foreach (var user in report.SessionType.Attendees.Where(u => u.IsActive))
				{
					if (user.Settings.ReportOccasions == SessionReportOccasions.Always
					    || (user.Settings.ReportOccasions == SessionReportOccasions.WhenAbsent && !report.PresentUsers.Contains(user)))
						x.To.Add(user.EmailAddress);
				}
			});
			if (mail.To.Count > 0)
				mail.Send();
		}
	}
}