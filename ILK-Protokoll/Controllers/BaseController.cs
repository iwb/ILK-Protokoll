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
	public class BaseController : Controller
	{
		protected readonly DataContext db = new DataContext();

		protected User _CurrentUser;

		public BaseController()
		{
			db.Database.Initialize(false);
		}

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			var user = GetCurrentUser();
			ViewBag.CurrentUser = user;
			ViewBag.CurrentColorScheme = user.ColorScheme;

			var session = GetSession();
			ViewBag.CurrentSession = session;
			if (session != null)
				ViewBag.LastSession = session.SessionType.LastDate;

			if (Request.QueryString["profiler"] != null)
				Session["profiler"] = Request.QueryString["profiler"] == "true";
		}

		[NotNull]
		protected User GetCurrentUser()
		{
			if (_CurrentUser == null)
			{
				var user = Session["CurrentUser"] as User;
				var userid = Session["UserID"] as int?;
				if (user != null)
					_CurrentUser = user;
				else if (userid != null)
					_CurrentUser = db.Users.AsNoTracking().Single(u => u.ID == userid);

				if (_CurrentUser == null) // User was not found in our database
					_CurrentUser = UserController.GetUser(db, User) ?? new User(); // new User() ==> Anonymous User

				Session["UserID"] = _CurrentUser.ID;
				Session["CurrentUser"] = _CurrentUser;
			}

			return _CurrentUser;
		}

		protected int GetCurrentUserID()
		{
			return (int?)Session["UserID"] ?? GetCurrentUserID();
		}

		[CanBeNull]
		protected ActiveSession GetSession()
		{
			var sessionID = Session["SessionID"];
			if (sessionID != null)
			{
				var s = db.ActiveSessions.Find((int)sessionID);
				Session["SessionID"] = s.ID;
				return s;
			}
			else
				return null;
		}

		protected SelectList CreateUserSelectList()
		{
			return new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
		}

		protected SelectList CreateUserSelectList(int selectedID)
		{
			return new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName", selectedID);
		}

		protected Dictionary<User, T> CreateUserDictionary<T>(Func<User, T> valueSelector)
		{
			return db.GetUserOrdered(GetCurrentUser()).ToDictionary(u => u, valueSelector);
		}

		public bool IsTopicLocked(int topicID)
		{
			return IsTopicLocked(db.Topics
				.Include(t => t.Lock)
				.Include(t => t.Lock.Session.Manager)
				.Single(t => t.ID == topicID));
		}

		protected bool IsTopicLocked(Topic t)
		{
			return t.Lock != null && !t.Lock.Session.Manager.Equals(GetCurrentUser());
		}

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

		protected ContentResult HTTPStatus(HttpStatusCode statuscode, string message)
		{
			return HTTPStatus((int)statuscode, message);
		}

		protected static string ErrorMessageFromException(DbEntityValidationException ex)
		{
			StringBuilder msg = new StringBuilder();
			foreach (var entity in ex.EntityValidationErrors)
				foreach (var error in entity.ValidationErrors)
					msg.AppendFormat("{0}: {1} <br />", error.PropertyName, error.ErrorMessage);

			return msg.ToString();
		}

		protected string ErrorMessageFromModelState()
		{
			StringBuilder msg = new StringBuilder();
			foreach (var kvp in ModelState)
				foreach (var error in kvp.Value.Errors)
					msg.AppendFormat("{0} <br />", error.ErrorMessage);

			return msg.ToString();
		}

		protected override void Initialize(RequestContext requestContext)
		{
			HttpContextBase Context = requestContext.HttpContext;

			Context.Response.SuppressDefaultCacheControlHeader = true;

			Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
			Context.Response.Headers["Pragma"] = "no-cache";
			Context.Response.Headers["Expires"] = "0";

			base.Initialize(requestContext);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();

			base.Dispose(disposing);
		}
	}
}