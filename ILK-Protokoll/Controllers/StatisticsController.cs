using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class StatisticsController : BaseController
	{
		// GET: Statistics
		[AllowAnonymous]
		public ActionResult Index()
		{
			return View(GetStats());
		}

		public PartialViewResult _Table()
		{
			return PartialView("_Table", GetStats());
		}

		private Dictionary<string, double> GetStats()
		{
			return new Dictionary<string, double>
			{
				{"Diskutierte Themen", db.Topics.Count()},
				{"Gefällte Beschlüsse", db.Decisions.Count(d => d.Type == DecisionType.Resolution)},
				{"Archivierte Themen", db.Decisions.Count(d => d.Type == DecisionType.Closed)},
				{"Durchgefürte Sitzungen", db.SessionReports.Count()},
				{"Vergebene Aufgaben", db.Assignments.Count()},
				{"Erledigte Aufgaben", db.Assignments.Count(a => a.IsDone)},
				{
					"Listenelemente",
					db.LConferences.Count() + db.LEmployeePresentations.Count() + db.LEvents.Count() + db.LExtensions.Count() +
					db.LHolidays.Count() + db.LIlkDays.Count() + db.LIlkMeetings.Count() + db.LOpenings.Count()
				},
				{"Benutzer", db.Users.Count(u => u.IsActive)},
				{"Anzahl Dokumente", db.Documents.Count()},
				{"Anzahl Dateien", db.Revisions.Count()}
			};
		}
	}
}