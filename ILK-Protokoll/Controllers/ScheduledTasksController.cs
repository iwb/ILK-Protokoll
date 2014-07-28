using System;
using System.Linq;
using System.Web.Mvc;
using ILK_Protokoll.Mailers;

namespace ILK_Protokoll.Controllers
{
	public class ScheduledTasksController : BaseController
	{
		private readonly TimeSpan _reminderTimeSpan = new TimeSpan(6, 0, 0, 0);

		// GET: ScheduledTasks
		[AllowAnonymous]
		public string Index()
		{
			int mailsSent = AssignmentsController.SendReminders(db);

			return string.Format("Es wurden {0} E-Mails verschickt.", mailsSent);
		}
	}
}