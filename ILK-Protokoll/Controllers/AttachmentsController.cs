using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class AttachmentsController : BaseController
	{
		public const string VirtualPath = "~/Attachments/Download/";
		private static readonly Regex InvalidChars = new Regex(@"[^a-zA-Z0-9_-]");

		private static readonly HashSet<string> OfficeExtensions = new HashSet<string>
		{
			"pdf",
			"doc",
			"docm",
			"docx",
			"dotx",
			"one",
			"pdf",
			"potx",
			"ppt",
			"pptx",
			"vsd",
			"xls",
			"xlsm",
			"xlsx",
			"xltx"
		};

		private string Serverpath
		{
			get { return @"C:\ILK-Protokoll_Uploads\"; }
		}

		public string GetVirtualPath(int attachmentID)
		{
			string userAgent = Request.UserAgent;
			bool isInternetExplorer = !string.IsNullOrEmpty(userAgent) && userAgent.Contains("Trident");
			if (Environment.MachineName == "02MUCILK" && isInternetExplorer)
			{
				Attachment a = db.Attachments.Find(attachmentID);
				bool isOfficeDocument = OfficeExtensions.Contains(a.Extension);
				if (isOfficeDocument)
					return "file://02mucilk/Uploads/" + a.FileName;
			}

			return Url.Action("Download", new {id = attachmentID});
		}

		// GET: Attachments
		public PartialViewResult _List(AttachmentContainer entity, int id, bool makeList = false)
		{
			var files = db.Attachments
				.Where(a => a.Deleted == null)
				.OrderBy(a => a.DisplayName)
				.Include(a => a.Uploader);

			if (entity == AttachmentContainer.Topic)
				files = files.Where(a => a.TopicID == id);
			else if (entity == AttachmentContainer.EmployeePresentation)
				files = files.Where(a => a.EmployeePresentationID == id);

			ViewBag.EntityID = id;
			ViewBag.CurrentUser = GetCurrentUser();
			ViewBag.KnownExtensions = new HashSet<string>(
				from path in Directory.GetFiles(Server.MapPath("~/img/fileicons"), "*.png")
				select Path.GetFileNameWithoutExtension(path));

			if (makeList)
				return PartialView("_AttachmentList", files.ToList());
			else
				return PartialView("_AttachmentTable", files.ToList());
		}

		public PartialViewResult _UploadForm(AttachmentContainer entity, int id)
		{
			return PartialView("_UploadForm", Tuple.Create(entity, id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _Upload(AttachmentContainer entity, int id)
		{
			if (Request.Files.Count == 0)
				return new HttpStatusCodeResult(HttpStatusCode.NoContent, "Es wurden keine Dateien empfangen.");

			if (id <= 0)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Die Dateien können keiner Diskussion zugeordnet werden.");

			var statusMessage = new StringBuilder();
			int successful = 0;

			for (int i = 0; i < Request.Files.Count; i++)
			{
				HttpPostedFileBase file = Request.Files[i];

				if (file == null)
					continue;

				if (string.IsNullOrWhiteSpace(file.FileName))
				{
					statusMessage.AppendLine("Eine Datei hat einen ungültigen Dateinamen.");
					continue;
				}
				string fullName = Path.GetFileName(file.FileName);
				if (file.ContentLength == 0)
				{
					statusMessage.AppendFormat("Datei \"{0}\" hat keinen Inhalt.", fullName).AppendLine();
					continue;
				}

				string filename = Path.GetFileNameWithoutExtension(file.FileName);
				string fileext = Path.GetExtension(file.FileName);
				if (!string.IsNullOrEmpty(fileext))
					fileext = fileext.Substring(1);

				var attachment = new Attachment
				{
					Deleted = null,
					DisplayName = fullName,
					SafeName = InvalidChars.Replace(filename, ""),
					Extension = fileext,
					FileSize = file.ContentLength,
					Uploader = GetCurrentUser(),
					Created = DateTime.Now
				};

				switch (entity)
				{
					case AttachmentContainer.Topic:
						attachment.TopicID = id;
						break;
					case AttachmentContainer.EmployeePresentation:
						attachment.EmployeePresentationID = id;
						break;
				}

				try
				{
					db.Attachments.Add(attachment);
					db.SaveChanges(); // Damit das Attachment seine ID bekommt. Diese wird anschließend im Dateinamen hinterlegt
					string path = Path.Combine(Serverpath, attachment.FileName);
					file.SaveAs(path);
					successful++;
				}
				catch (DbEntityValidationException)
				{
					statusMessage.AppendFormat("Datei \"{0}\" konnte nicht in der Datenbank gespeichert werden.", fullName)
						.AppendLine();
				}
				catch (IOException)
				{
					statusMessage.AppendFormat("Datei \"{0}\" konnte nicht gespeichert werden.", fullName).AppendLine();
				}
			}
			statusMessage.AppendFormat(
				successful == 1 ? "Eine Datei wurde erfolgreich verarbeitet." : "{0} Dateien wurden erfolgreich verarbeitet.",
				successful);

			ViewBag.StatusMessage = statusMessage.ToString();

			return _List(entity, id);
		}

		[HttpPost]
		public ActionResult _Delete(int attachmentID)
		{
			Attachment attachment = db.Attachments.Include(a => a.Uploader).First(a => a.ID == attachmentID);

			if (attachment.Deleted != null)
				return HTTPStatus(422, "Das Objekt befindet sich bereits im Papierkorb.");

			attachment.Deleted = DateTime.Now; // In den Papierkorb
			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var message = ErrorMessageFromException(e);
				return HTTPStatus(500, message);
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}

		public ActionResult _PermanentDelete(int attachmentID)
		{
			Attachment attachment = db.Attachments.Include(a => a.Uploader).First(a => a.ID == attachmentID);

			if (attachment.Deleted == null) // In den Papierkorb
				return HTTPStatus(422, "Das Objekt befindet sich noch nicht im Papierkorb.");

			try
			{
				string path = Path.Combine(Serverpath, attachment.FileName);
				System.IO.File.Delete(path);

				attachment.TopicID = null;
				attachment.EmployeePresentationID = null;

				db.Attachments.Remove(attachment);
			}
			catch (IOException)
			{
				return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
			}

			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var message = ErrorMessageFromException(e);
				return HTTPStatus(500, message);
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}

		public FileResult Download(int id)
		{
			Attachment a = db.Attachments.Find(id);

			var cd = new ContentDisposition
			{
				FileName = a.DisplayName,
				Inline = true,
			};
			Response.AppendHeader("Content-Disposition", cd.ToString());

			return File(Path.Combine(Serverpath, a.FileName), MimeMapping.GetMimeMapping(a.DisplayName));
		}
	}
}