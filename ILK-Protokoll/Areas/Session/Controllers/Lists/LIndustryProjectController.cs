using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models.Lists;
using ILK_Protokoll.util;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LEventsController : ParentController<Event>
	{
		private static readonly Regex SpecificRegex = new Regex(@"(\d\d:\d\d)\s?\W\s?(\d\d:\d\d)", RegexOptions.IgnoreCase);
		private static readonly Regex MorningRegex = new Regex(@"morgen", RegexOptions.IgnoreCase);
		private static readonly Regex MidnoonRegex = new Regex(@"vormittag", RegexOptions.IgnoreCase);
		private static readonly Regex AfternoonRegex = new Regex(@"nachmittag", RegexOptions.IgnoreCase);
		private static readonly Regex EveningRegex = new Regex(@"abend", RegexOptions.IgnoreCase);

		public LEventsController()
		{
			_dbSet = db.LEvents;
			Entities = _dbSet.OrderBy(e => e.StartDate).ThenBy(e => e.EndDate);
		}

		public override ActionResult Download(int id)
		{
			Debug.Assert(Request.Url != null, "Request.Url != null");

			var Event = _dbSet.Find(id);
			if (Event.GUID == null)
			{
				Event.GUID = Guid.NewGuid();
				db.SaveChanges();
			}

			bool allDayEvent = false;
			DateTime start, end;

			if (Event.StartDate != Event.EndDate)
			{
				allDayEvent = true;
				start = Event.StartDate;
				end = Event.EndDate.AddDays(1);
			}
			else
			{
				if (SpecificRegex.IsMatch(Event.Time))
				{
					var match = SpecificRegex.Match(Event.Time);
					start = Event.StartDate.Add(TimeSpan.Parse(match.Groups[1].Value));
					end = Event.StartDate.Add(TimeSpan.Parse(match.Groups[2].Value));
				}
				else if (MorningRegex.IsMatch(Event.Time))
				{
					start = Event.StartDate.AddHours(8);
					end = Event.StartDate.AddHours(10);
				}
				else if (MidnoonRegex.IsMatch(Event.Time))
				{
					start = Event.StartDate.AddHours(9);
					end = Event.StartDate.AddHours(12);
				}
				else if (AfternoonRegex.IsMatch(Event.Time))
				{
					start = Event.StartDate.AddHours(13);
					end = Event.StartDate.AddHours(17);
				}
				else if (EveningRegex.IsMatch(Event.Time))
				{
					start = Event.StartDate.AddHours(17);
					end = Event.StartDate.AddHours(20);
				}
				else
				{
					allDayEvent = true;
					start = Event.StartDate;
					end = Event.EndDate.AddDays(1);
				}
			}

			var ical = CreateCalendarEvent("Termin: " + Event.Description.Shorten(50),
				Event.Description + "\r\n\r\nhttp://" + Request.Url.Authority + Url.Content("~/ViewLists#event_table"),
				start, end,
				Event.Place, Event.GUID.ToString(), allDayEvent);

			return Content(ical, "text/calendar", Encoding.UTF8);
		}
	}
}