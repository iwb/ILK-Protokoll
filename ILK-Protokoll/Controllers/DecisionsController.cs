using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Controllers
{
	public class DecisionsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.DecisionStyle = "active";
		}

		// GET: Decisions
		public ActionResult Index(FilteredDecisions filter)
		{
			IQueryable<Decision> query = db.Decisions
				.Include(d => d.OriginTopic)
				.Include(d => d.Report);

			if (!filter.ShowClosed)
				query = query.Where(d => d.Type != DecisionType.Closed);

			if (!filter.ShowResolution)
				query = query.Where(d => d.Type != DecisionType.Resolution);


			if (filter.Timespan != 0)
			{
				if (filter.Timespan == -1) // Zeitfenster berücksichtigen
				{
					var cutoff = filter.WindowEndDate.AddDays(1); // Inklusivgrenzen
					query = query.Where(d => d.Report.End >= filter.WindowStartDate && d.Report.End <= cutoff);
				}
				else
				{
					var cutoff = DateTime.Today.AddDays(-filter.Timespan);
					query = query.Where(d => d.Report.End > cutoff);
				}
			}

			if (!string.IsNullOrWhiteSpace(filter.Searchterm))
				query = query.Where(d => d.Text.Contains(filter.Searchterm) || d.OriginTopic.Title.Contains(filter.Searchterm));

			filter.Decisions = query.OrderBy(d => d.Report.End).ToList();
			filter.TimespanChoices = TimespanChoices(filter.Timespan);
			return View(filter);
		}

		private static IEnumerable<SelectListItem> TimespanChoices(int preselect)
		{
			return new[]
			{
				new SelectListItem
				{
					Text = "(Zeitraum beliebig)",
					Value = "0",
					Selected = preselect == 0
				},
				new SelectListItem
				{
					Text = "1 Tag",
					Value = "1",
					Selected = preselect == 1
				},
				new SelectListItem
				{
					Text = "7 Tage",
					Value = "7",
					Selected = preselect == 7
				},
				new SelectListItem
				{
					Text = "30 Tage",
					Value = "30",
					Selected = preselect == 30
				},
				new SelectListItem
				{
					Text = "100 Tage",
					Value = "100",
					Selected = preselect == 100
				},
				new SelectListItem
				{
					Text = "Benutzerdefiniert",
					Value = "-1",
					Selected = preselect == -1
				}
			};
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}