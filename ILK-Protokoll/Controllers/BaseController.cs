using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ILK_Protokoll.Areas.Administration.Controllers;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class BaseController : Controller
	{
		protected readonly DataContext db = new DataContext();

		protected User _CurrentUser;

		protected User GetCurrentUser()
		{
			if (_CurrentUser == null)
			{
				if (Session["UserID"] == null)
				{
					_CurrentUser = UserController.GetUser(db, User);
					Session["UserID"] = _CurrentUser.ID;
				}
				else
					_CurrentUser = db.Users.Find(Session["UserID"]);
			}

			return _CurrentUser;
		}

		protected ActiveSession GetSession()
		{
			var sessionID = (int?)Session["SessionID"];
			if (sessionID.HasValue && sessionID > 0)
				return db.ActiveSessions.Find(sessionID);
			else
				return null;
		}

		protected bool SendMail(string subject, string message)
		{
			return SendMail(subject, message, GetCurrentUser());
		}

		protected bool SendMail(string subject, string message, User recipient)
		{
			MailMessage mail = new MailMessage
			{
				From = new MailAddress("no-reply@iwb.tum.de", "ILK-Protokoll"),
				Subject = subject,
				Body = message,
			};
			mail.To.Add(new MailAddress(recipient.EmailAddress));

			SmtpClient smtpClient = new SmtpClient("mail", 25)
			{
				UseDefaultCredentials = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				EnableSsl = false
			};
			try
			{
				smtpClient.Send(mail);
				return true;
			}
			catch (SmtpException)
			{
				return false;
			}
		}

		protected ContentResult HTTPStatus(int statuscode, string message)
		{
			Response.Clear();
			Response.StatusCode = statuscode;
			Response.StatusDescription = "Unprocessable Entity";
			return Content(message);
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

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.ColorScheme = GetCurrentUser().ColorScheme;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();

			base.Dispose(disposing);
		}
	}
}