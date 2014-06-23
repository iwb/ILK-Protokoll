using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using System.Web.Mvc;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	public class RecycleBinController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.AdminStyle = "active";
			ViewBag.ARBStyle = "active";
		}

		// GET: Administration/RecycleBin
		public ActionResult Index()
		{
			var items = db.Attachments.Where(a => a.Deleted != null).Include(a => a.Topic).OrderByDescending(a => a.Created).ToList();
			return View(items);
		}


		public ActionResult _Restore(int attachmentID)
		{
			var attachment = db.Attachments.Include(a => a.Uploader).First(a => a.ID == attachmentID);
			attachment.Deleted = null;

			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				Debug.WriteLine(e.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage);
				return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}

		[HttpGet]
		public ActionResult Purge()
		{
			var itemcount = db.Attachments.Count(a => a.Deleted != null);
			return View(itemcount);
		}

		[HttpPost, ActionName("Purge")]
		[ValidateAntiForgeryToken]
		public ActionResult PurgeConfirmed()
		{
			db.Attachments.RemoveRange(db.Attachments.Where(a => a.Deleted != null));
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}