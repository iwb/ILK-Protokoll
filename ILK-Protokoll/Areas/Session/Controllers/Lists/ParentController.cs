using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using EntityFramework.Extensions;
using ILK_Protokoll.Areas.Session.Models.Lists;
using ILK_Protokoll.Controllers;
using Event = DDay.iCal.Event;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	// ReSharper disable Mvc.PartialViewNotResolved
	/// <summary>
	/// Dieser Controller ist die Basis für alle Controller der Listenelemente und übernimmt die Grundfunktionen. Besonders einfache listen können so allein durch eine Ableitung und Konkretisierung dieser Klasse erstellt werden. Im Konstruktor der Kindklasse MUSS das Feld <see cref="_dbSet" /> mit der passenden Referenz aus <see cref="BaseController.db" /> befüllt werden.
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public class ParentController<TModel> : SessionBaseController
		where TModel : BaseItem, new()
	{
		private readonly TimeSpan _editDuration = TimeSpan.FromMinutes(5);
		protected DbSet<TModel> _dbSet;
		private IQueryable<TModel> _entities;

		/// <summary>
		/// Die Liste der Datensätze, die angezeigt werden sollen. In der Kindklasse können hier noch Einschränkungen oder Sortierungen vorgenommen werden.
		/// </summary>
		protected IQueryable<TModel> Entities
		{
			get { return _entities ?? _dbSet; }
			set { _entities = value; }
		}

		public virtual PartialViewResult _List(bool reporting = false)
		{
			CleanupLocks();

			ViewBag.Reporting = reporting;
			return PartialView(Entities.ToList());
		}

		protected void CleanupLocks()
		{
			var thisSession = GetSession();

			if (thisSession == null)
				return;

			var cutoff = DateTime.Now - _editDuration;
			// Locks entfernen, die zu alt sind
			_dbSet.Where(e => e.LockTime < cutoff).Update(e => new TModel {LockSessionID = null});
			// Und die eigenen Locks entfernen
			_dbSet.Where(e => e.LockSessionID == thisSession.ID).Update(e => new TModel {LockSessionID = null});
		}

		public virtual PartialViewResult _CreateForm()
		{
			return PartialView("_CreateForm", new TModel());
		}

		public virtual PartialViewResult _FetchRow(int id)
		{
			var row = Entities.Single(m => m.ID == id);
			var session = GetSession();
			if (session != null && row.LockSessionID == session.ID) // ggf. lock entfernen
			{
				row.LockSessionID = null;
				db.SaveChanges();
			}
			ViewBag.Reporting = false;

			return PartialView("_Row", row);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult _Create([Bind(Exclude = "ID, LastChanged")] TModel ev)
		{
			if (!ModelState.IsValid)
			{
				var message = ErrorMessageFromModelState();
				return HTTPStatus(422, message);
			}

			var row = _dbSet.Create();
			TryUpdateModel(row, "", null, new[] {"LastChanged"});
			_dbSet.Add(row);

			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var message = ErrorMessageFromException(e);
				return HTTPStatus(HttpStatusCode.InternalServerError, message);
			}
			return _FetchRow(row.ID);
		}

		public virtual ActionResult _BeginEdit(int id)
		{
			TModel ev = Entities.Single(m => m.ID == id);
			var session = GetSession();
			if (ev == null)
				return HttpNotFound();
			else if (ev.LockSessionID.HasValue && (session == null || ev.LockSessionID != session.ID))
				return HTTPStatus(HttpStatusCode.Conflict, "Der Listeneintrag ist gesperrt.");

			if (session != null)
			{
				ev.LockSessionID = session.ID; // Lock erzeugen
				ev.LockTime = DateTime.Now;

				try
				{
					db.SaveChanges();
				}
				catch (DbEntityValidationException e)
				{
					var message = ErrorMessageFromException(e);
					return HTTPStatus(HttpStatusCode.InternalServerError, message);
				}
			}
			return PartialView("_Edit", ev);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult _Edit([Bind(Exclude = "LastChanged")] TModel input)
		{
			var session = GetSession();
			if (!ModelState.IsValid)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			// Get the object from the database to enable lazy loading.
			var row = Entities.Single(m => m.ID == input.ID);

			if (row.LockSessionID != null && (session == null || row.LockSessionID != session.ID))
				return HTTPStatus(HttpStatusCode.Conflict, "Der Datensatz ist momentan gesperrt."); // HTTP 409 Conflict

			TryUpdateModel(row, "", null, new[] {"LastChanged"});
			row.LockSessionID = null;
			row.LastChanged = DateTime.Now;

			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var message = ErrorMessageFromException(e);
				return HTTPStatus(HttpStatusCode.InternalServerError, message);
			}
			return _FetchRow(input.ID);
		}

		[HttpPost]
		public virtual ActionResult _Delete(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			TModel ev = _dbSet.Find(id.Value);
			if (ev == null)
				return HttpNotFound();
			else if (ev.LockSessionID != null)
				return HTTPStatus(HttpStatusCode.Conflict, "Der Eintrag wird gerade bearbeitet!");

			_dbSet.Remove(_dbSet.Find(id));
			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var message = ErrorMessageFromException(e);
				return HTTPStatus(HttpStatusCode.InternalServerError, message);
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}

		public virtual ActionResult Download(int id)
		{
			throw new NotImplementedException();
		}

		protected static string CreateCalendarEvent(
			string title, string description, DateTime startDate, DateTime endDate,
			string location, string eventId, bool allDayEvent)
		{
			if (string.IsNullOrWhiteSpace(eventId))
				eventId = Guid.NewGuid().ToString();

			var iCal = new iCalendar
			{
				Method = "REQUEST",
				Version = "2.0"
			};

			iCal.AddLocalTimeZone();

			var evt = iCal.Create<Event>();
			evt.Summary = title;
			evt.Start = new iCalDateTime(startDate);
			evt.End = new iCalDateTime(endDate);
			evt.Description = description;
			evt.Location = location;
			evt.IsAllDay = allDayEvent;
			evt.UID = eventId;
			evt.Organizer = new Organizer {CommonName = "ILK-Protokoll", Value = new Uri("mailto:no-reply@iwb.tum.de")};
			evt.Alarms.Add(new Alarm
			{
				Duration = new TimeSpan(0, 15, 0),
				Trigger = new Trigger(new TimeSpan(0, 15, 0)),
				Action = AlarmAction.Display,
				Description = "Erinnerung"
			});

			return new iCalendarSerializer().SerializeToString(iCal);
		}
	}

	// ReSharper restore Mvc.PartialViewNotResolved
}