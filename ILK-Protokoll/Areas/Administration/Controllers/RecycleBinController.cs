using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Controllers;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	[DisplayName("Papierkorb")]
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
			var items =
				db.Documents.Where(a => a.Deleted != null).OrderByDescending(a => a.Created).ToList();
			return View(items);
		}


		public ActionResult _Restore(int documentID)
		{
			var attachment = db.Documents.Include(a => a.LatestRevision).Single(d => d.ID == documentID);

			if (attachment.TopicID == null && attachment.EmployeePresentationID == null) // Verwaist
				return HTTPStatus(422, "Wiederherstellungsziel ist nicht mehr vorhanden");

			if (attachment.TopicID != null && attachment.Topic.IsReadOnly)
				return HTTPStatus(422, "Wiederherstellungsziel ist schreibgeschützt");

			attachment.Deleted = null;

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

		[HttpGet]
		public ActionResult Purge()
		{
			var itemcount = db.Documents.Count(a => a.Deleted != null);
			return View(itemcount);
		}

		[HttpPost, ActionName("Purge")]
		[ValidateAntiForgeryToken]
		public ActionResult PurgeConfirmed()
		{
			var ac = new AttachmentsController();
			foreach (var doc in db.Documents.Where(a => a.Deleted != null))
			{
				ac._PermanentDelete(doc.ID);
			}
			return RedirectToAction("Index");
		}
	}
}