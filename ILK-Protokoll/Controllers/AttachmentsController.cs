using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class AttachmentsController : BaseController
	{
		public const string VirtualPath = "~/Uploads/";
		private static readonly Regex InvalidChars = new Regex(@"[^a-zA-Z0-9_-]");

		private string Serverpath
		{
			get { return Server.MapPath(VirtualPath); }
		}

		// GET: Attachments
		public PartialViewResult _List(int? topicID, bool makeList = false)
		{
			List<Attachment> files = db.Attachments
				.Where(a => a.TopicID == topicID)
				.Where(a => a.Deleted == null)
				.OrderBy(a => a.DisplayName)
				.Include(a => a.Uploader)
				.ToList();
			ViewBag.TopicID = topicID;
			ViewBag.CurrentUser = GetCurrentUser();
			ViewBag.KnownExtensions = new HashSet<string>(
				from path in Directory.GetFiles(Server.MapPath("~/img/fileicons"), "*.png")
				select Path.GetFileNameWithoutExtension(path));

			if (makeList)
				return PartialView("_AttachmentList", files);
			else
				return PartialView("_AttachmentTable", files);
		}

		public PartialViewResult _UploadForm(int topicID)
		{
			return PartialView("_UploadForm", topicID);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _Upload(int topicID)
		{
			if (Request.Files.Count == 0)
				return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Es wurden keine Dateien empfangen.");

			if (topicID <= 0)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Die Dateien können keiner Diskussion zugeordnet werden.");

			bool success = false;

			for (int i = 0; i < Request.Files.Count; i++)
			{
				var file = Request.Files[i];
				if (file != null && file.ContentLength > 0)
				{
					string filename = Path.GetFileNameWithoutExtension(file.FileName);
					string fileext = Path.GetExtension(file.FileName);
					if (!string.IsNullOrEmpty(fileext))
						fileext = fileext.Substring(1);

					var attachment = new Attachment
					{
						TopicID = topicID,
						Deleted = null,
						DisplayName = Path.GetFileName(file.FileName),
						SafeName = InvalidChars.Replace(filename, ""),
						Extension = fileext,
						FileSize = file.ContentLength,
						Uploader = GetCurrentUser(),
						Created = DateTime.Now
					};
					db.Attachments.Add(attachment);
					db.SaveChanges(); // Damit das Attachment seine ID bekommt. Diese wird anschließend im Dateinamen hinterlegt

					string path = Path.Combine(Serverpath, attachment.FileName);
					file.SaveAs(path);
					success = true;
				}
			}

			if (success)
				return _List(topicID);
			else
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
					"Der Server hat zwar Dateien empfangen, konnte sie jedoch nicht verarbeiten.");
		}

		[HttpPost]
		public ActionResult _Delete(int attachmentID)
		{
			var attachment = db.Attachments.Include(a => a.Uploader).First(a => a.ID == attachmentID);


			if (attachment.Deleted == null) // In den Papierkorb
			{
				attachment.Deleted = DateTime.Now;
			}
			else // Endgültig löschen
			{
				try
				{
					string path = Path.Combine(Serverpath, attachment.FileName);
					System.IO.File.Delete(path);
					attachment.Topic.Attachments.Remove(attachment);
					db.Attachments.Remove(attachment);
				}
				catch (IOException)
				{
					return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
				}
			}

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
	}
}