using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ILK_Protokoll.Areas.Administration.Controllers;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;
using JetBrains.Annotations;

namespace ILK_Protokoll.Controllers
{
	/// <summary>
	///    Der Basiscontroller, von dem alle anderen Controller abgeleitet sind. Er stellt die Datenbankverbindung her und
	///    bietet einige kleine Methoden, die in mehrer Controllern benötigt werden.
	/// </summary>
	public class BaseController : Controller
	{
		private User _currentUser;

		/// <summary>
		///    Die Datenbankreferenz, die jeder Controller erbt.
		/// </summary>
		protected readonly DataContext db = new DataContext();

		/// <summary>
		///    Die Datenbank wird ggf. initialisiert.
		/// </summary>
		public BaseController()
		{
			db.Database.Initialize(false);
			db.Database.Log = Console.Write;
		}

		/// <summary>
		///    Diese Methode läuft vor jeder Aktion jedes Controllers. Hier werden häufig benutzte Variablen in ViewBag gespeichert
		///    und die Referent auf die Sitzung gesetzt. zudem wird ggf. der Profiler aktiviert, falls die URL mit ?profiler=true
		///    aufgerufen wurde.
		/// </summary>
		/// <param name="filterContext"></param>
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			var user = GetCurrentUser();
			ViewBag.CurrentUser = user;
			ViewBag.CurrentColorScheme = user.Settings.ColorScheme;

			var session = GetSession();
			ViewBag.CurrentSession = session;
			if (session != null)
				ViewBag.LastSession = session.SessionType.LastDate;

			if (Request.QueryString["profiler"] != null)
				Session["profiler"] = Request.QueryString["profiler"] == "true";
		}

		/// <summary>
		///    Gibt den aktuell eingeloggten Benutzer zurück, oder ein Nullobjekt, falls keine Autorisierung erfolgt ist.
		/// </summary>
		/// <returns></returns>
		[NotNull]
		protected User GetCurrentUser()
		{
			if (_currentUser != null)
				return _currentUser;

			var user = Session["CurrentUser"] as User;
			var userid = Session["UserID"] as int?;
			if (user != null)
				_currentUser = user;
			else if (userid != null)
				_currentUser = db.Users.AsNoTracking().Single(u => u.ID == userid);

			if (_currentUser == null) // User was not found in our database
				_currentUser = UserController.GetUser(db, User);

			Session["UserID"] = _currentUser.ID;
			Session["CurrentUser"] = _currentUser;

			return _currentUser;
		}

		/// <summary>
		///    Gibt die ID des aktuellen Benutzers zurück. Falls keine Autorisierung erfolgt (anonymer Zugriff) wird 0
		///    zurückgegeben.
		/// </summary>
		/// <returns>Die ID des Benutzers oder 0, falls kein Benutzer eingeloggt ist.</returns>
		protected int GetCurrentUserID()
		{
			return (int?)Session["UserID"] ?? GetCurrentUser().ID;
		}

		/// <summary>
		///    Gibt die aktuell laufende Sitzung zurück, oder null falls keine Sitzung läuft. Es wird keinesfalls eine Sitzung
		///    erzeugt oder wiederaufgenommen.
		/// </summary>
		/// <returns>Die aktuell laufende Sitzung zurück, oder null falls keine Sitzung läuft.</returns>
		[CanBeNull]
		protected ActiveSession GetSession()
		{
			var sessionID = Session["SessionID"];
			if (sessionID != null)
			{
				var s = db.ActiveSessions.Find((int)sessionID);

				if (s != null)
					Session["SessionID"] = s.ID;
				else
					Session.Remove("SessionID"); // Stale session

				return s;
			}
			else
				return null;
		}

		/// <summary>
		///    Gibt eine SelectList zurück, die die aktiven Benutzer enthält.
		/// </summary>
		/// <returns>Die Liste mit den aktiven Benutzern. Der aktuelle Benutzer ist immer ganz oben.</returns>
		protected SelectList CreateUserSelectList()
		{
			return new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
		}

		/// <summary>
		///    Gibt eine SelectList zurück, die die aktiven Benutzer enthält. Ermöglicht eine Auswahl des selektierten Benutzers
		///    per ID.
		/// </summary>
		/// <param name="selectedID">Die UserID, die vorselektiert sein soll.</param>
		/// <returns>Die Liste mit den aktiven Benutzern. Der aktuelle Benutzer ist immer ganz oben.</returns>
		protected SelectList CreateUserSelectList(int selectedID)
		{
			return new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName", selectedID);
		}

		/// <summary>
		///    Gibt ein Dictionary zurück, dass Benutzern einen definierten Wert zuweist. Hilfreich insbesondere als Dictionary&lt;
		///    User, bool&gt; für eine Reihe von Checkboxen.
		/// </summary>
		/// <param name="valueSelector"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected Dictionary<User, T> CreateUserDictionary<T>(Func<User, T> valueSelector)
		{
			return db.GetUserOrdered(GetCurrentUser()).ToDictionary(u => u, valueSelector);
		}

		/// <summary>
		///    Gibt ein Dictionary zurück, dass alle vorhandenen Tags als Schlüssel enthält.
		/// </summary>
		/// <param name="preselectedTags">Tags, für die das Ergebnisdictionary true enthalten soll.</param>
		/// <returns>Ein Dictionary mit allen Tags.</returns>
		protected IDictionary<Tag, bool> CreateTagDictionary(IEnumerable<TagTopic> preselectedTags)
		{
			var preselection = preselectedTags.Select(tt => tt.TagID).ToArray();
			return db.Tags.ToDictionary(t => t, t => preselection.Contains(t.ID));
		}

		/// <summary>
		///    Ermittelt, ob das Thema mit der TopicID gerade gesperrt ist. Hierzu wird jedesmal eine SQL-Abfrage gestellt.
		/// </summary>
		/// <param name="topicID">Die TopicID der Diskussion</param>
		/// <returns>Status der Diskussion</returns>
		public bool IsTopicLocked(int topicID)
		{
			return IsTopicLocked(db.Topics
				.Include(t => t.Lock)
				.Include(t => t.Lock.Session.Manager)
				.Single(t => t.ID == topicID));
		}

		/// <summary>
		///    Ermittelt, ob das Thema gerade gesperrt ist. Hierzu wird jedesmal eine SQL-Abfrage gestellt.
		/// </summary>
		/// <param name="t">Die Diskussion, deren Status ermittelt werden soll.</param>
		/// <returns>Status der Diskussion</returns>
		protected bool IsTopicLocked(Topic t)
		{
			var tlock =
				db.TopicLocks.Where(tl => tl.TopicID == t.ID).Select(tl => new {tl.TopicID, tl.Session.ManagerID}).SingleOrDefault();
			return tlock != null && tlock.ManagerID != GetCurrentUserID();
		}

		/// <summary>
		///    Markiert ein Thema als ungelesen für alle Benutzer (steuerbar über den Parameter <paramref name="skipCurrentUser"/>)
		/// </summary>
		/// <param name="topic">Das Thema, dass als ungelesen markliert werden soll.</param>
		/// <param name="skipCurrentUser">
		///    Wenn true, wird der gelesen-Status des Themas für den aktuellen Benutzer nicht verändert.
		///    (Standard)
		/// </param>
		protected void MarkAsUnread(Topic topic, bool skipCurrentUser = true)
		{
			var lazyusers = topic.UnreadBy.ToDictionary(u => u.UserID);
			var users = db.GetActiveUsers();
			if (skipCurrentUser)
			{
				var cuid = GetCurrentUserID();
				users = users.Where(u => u.ID != cuid);
			}
			foreach (var user in users)
			{
				if (lazyusers.ContainsKey(user.ID))
					lazyusers[user.ID].LatestChange = DateTime.Now;
				else
					topic.UnreadBy.Add(new UnreadState {TopicID = topic.ID, UserID = user.ID});
			}
		}

		/// <summary>
		///    Markiert eine Diskussion für den aktuell eingeloggten Benutzer als gelesen.
		/// </summary>
		/// <param name="topic">Die Diskussion</param>
		protected void MarkAsRead(Topic topic)
		{
			var item = topic.UnreadBy.FirstOrDefault(u => u.UserID == GetCurrentUserID());
			if (item != null)
			{
				db.UnreadState.Remove(item);
				db.SaveChanges();
			}
		}

		/// <summary>
		///    Gibt eine HTTP Statusmeldung inklusive Inhalt zurück. Der Statuscode und der Inhalt können separat angegeben werden.
		/// </summary>
		/// <param name="statuscode">Der Statuscode (z.B. 500 für "Internal Server Error")</param>
		/// <param name="message">Die Statusnachricht, die an den Client übermittelt werden soll.</param>
		/// <returns></returns>
		protected ContentResult HTTPStatus(int statuscode, string message)
		{
			Response.Clear();
			Response.StatusCode = statuscode;

			if (Enum.IsDefined(typeof(HttpStatusCode), statuscode))
				Response.StatusDescription = ((HttpStatusCode)statuscode).ToString();
			else if (statuscode == 422)
				Response.StatusDescription = "Unprocessable Entity";
			else
				Response.StatusDescription = "Internal Server Error";

			return Content(message);
		}

		/// <summary>
		///    Gibt eine HTTP Statusmeldung inklusive Inhalt zurück. Der Statuscode und der Inhalt können separat angegeben werden.
		/// </summary>
		/// <param name="statuscode">Der Statuscode (z.B. HttpStatusCode.InternalServerError)</param>
		/// <param name="message">Die Statusnachricht, die an den Client übermittelt werden soll.</param>
		/// <returns></returns>
		protected ContentResult HTTPStatus(HttpStatusCode statuscode, string message)
		{
			return HTTPStatus((int)statuscode, message);
		}

		/// <summary>
		///    Generiert eine lesbare Fehlermeldung aus der Exception <paramref name="ex"/>.
		/// </summary>
		/// <param name="ex">Die aufgetretene Exception.</param>
		/// <returns>Eine Fehlermeldung im HTML-Format.</returns>
		protected static string ErrorMessageFromException(DbEntityValidationException ex)
		{
			StringBuilder msg = new StringBuilder();
			foreach (var entity in ex.EntityValidationErrors)
				foreach (var error in entity.ValidationErrors)
					msg.AppendFormat("{0}: {1} <br />", error.PropertyName, error.ErrorMessage);

			return msg.ToString();
		}

		/// <summary>
		///    Generiert eine lesbare Fehlermeldung aus dem Modellstatus des Controllers.
		/// </summary>
		/// <returns>Eine Fehlermeldung im HTML-Format.</returns>
		protected string ErrorMessageFromModelState()
		{
			StringBuilder msg = new StringBuilder();
			foreach (var kvp in ModelState)
				foreach (var error in kvp.Value.Errors)
					msg.AppendFormat("{0} <br />", error.ErrorMessage);

			return msg.ToString();
		}

		/// <summary>
		///    Die Header werden gesetzt, sodass der Browsercache möglichst unterbunden wird. Da die Seite meistens im LAN
		///    aufgerufen wird, sollte dies keine starken Auswirkungen auf die Geschwindigkeit haben. Umgekehrt sind die gecachten
		///    Daten oft veraltet.
		/// </summary>
		protected override void Initialize(RequestContext requestContext)
		{
			HttpContextBase Context = requestContext.HttpContext;

			Context.Response.SuppressDefaultCacheControlHeader = true;

			Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
			Context.Response.Headers["Pragma"] = "no-cache";
			Context.Response.Headers["Expires"] = "0";

			base.Initialize(requestContext);
		}

		/// <summary>
		/// Gibt alle von der aktuellen Instanz der <see cref="T:System.Web.Mvc.Controller"/>-Klasse verwendeten Ressourcen frei.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();

			base.Dispose(disposing);
		}
	}
}