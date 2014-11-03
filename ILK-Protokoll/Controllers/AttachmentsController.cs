using System;
using System.Collections.Generic;
using System.ComponentModel;
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

		public static HashSet<string> KnownExtensions = new HashSet<string>();

		private string Serverpath
		{
			get { return @"C:\ILK-Protokoll_Uploads\"; }
		}

		// GET: Attachments
		public PartialViewResult _List(int id, DocumentContainer entityKind, bool makeList = false, bool showActions = true)
		{
			var documents = db.Documents
				.Where(a => a.Deleted == null)
				.OrderBy(a => a.DisplayName)
				.Include(a => a.Revisions)
				.Include(a => a.LatestRevision)
				.Include(a => a.LatestRevision.Uploader);

			if (entityKind == DocumentContainer.Topic)
				documents = documents.Where(a => a.TopicID == id);
			else if (entityKind == DocumentContainer.EmployeePresentation)
				documents = documents.Where(a => a.EmployeePresentationID == id);


			KnownExtensions = new HashSet<string>(
				from path in Directory.GetFiles(Server.MapPath("~/img/fileicons"), "*.png")
				select Path.GetFileNameWithoutExtension(path));

			ViewBag.EntityID = id;
			ViewBag.KnownExtensions = KnownExtensions;

			if (makeList)
				return PartialView("_AttachmentList", documents.ToList());
			else
			{
				return showActions
					? PartialView("_AttachmentTable", documents.ToList())
					: PartialView("~/Areas/Session/Views/Finalize/_ReportAttachments.cshtml", documents.ToList());
			}
		}

		public ActionResult _UploadForm(int id, DocumentContainer entityKind)
		{
			if (entityKind == DocumentContainer.Topic && IsTopicLocked(id))
				return Content("<div class=\"panel-footer\">Da das Thema gesperrt ist, können Sie keine Dateien hochladen.</div>");
			else
				return PartialView("_DocumentCreateForm", Tuple.Create(entityKind, id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _CreateDocuments(DocumentContainer entityKind, int id)
		{
			if (Request.Files.Count == 0)
				return HTTPStatus(HttpStatusCode.BadRequest, "Es wurden keine Dateien empfangen.");

			if (id <= 0)
				return HTTPStatus(HttpStatusCode.BadRequest, "Die Dateien können keinem Ziel zugeordnet werden.");

			Topic topic = null;
			if (entityKind == DocumentContainer.Topic)
			{
				topic = db.Topics.Find(id);

				if (IsTopicLocked(id))
					return HTTPStatus(HttpStatusCode.Forbidden, "Da das Thema gesperrt ist, können Sie keine Dateien hochladen.");

				if (topic.IsReadOnly)
				{
					return HTTPStatus(HttpStatusCode.Forbidden,
						"Da das Thema schreibgeschützt ist, können Sie keine Dateien bearbeiten.");
				}
			}

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

				var revision = new Revision
				{
					SafeName = InvalidChars.Replace(filename, ""),
					FileSize = file.ContentLength,
					UploaderID = GetCurrentUserID(),
					Extension = fileext,
					GUID = Guid.NewGuid(),
					Created = DateTime.Now
				};

				Document document = new Document(revision)
				{
					Deleted = null,
					DisplayName = fullName
				};

				switch (entityKind)
				{
					case DocumentContainer.Topic:
						document.TopicID = id;
						break;
					case DocumentContainer.EmployeePresentation:
						document.EmployeePresentationID = id;
						break;
					default:
						throw new InvalidEnumArgumentException("entityKind", (int)entityKind, typeof(DocumentContainer));
				}

				try
				{
					db.Documents.Add(document);
					db.SaveChanges(); // Damit die Revision die ID bekommt. Diese wird anschließend im Dateinamen hinterlegt
					document.LatestRevision = revision;
					db.SaveChanges();
					string path = Path.Combine(Serverpath, revision.FileName);
					file.SaveAs(path);
					successful++;
				}
				catch (DbEntityValidationException ex)
				{
					var message = ErrorMessageFromException(ex);
					statusMessage.AppendFormat("Datei \"{0}\" konnte nicht in der Datenbank gespeichert werden.\n{1}", fullName,
						message)
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

			// Ungelesen-Markierung aktualisieren
			if (topic != null && successful > 0)
				MarkAsUnread(topic);

			ViewBag.StatusMessage = statusMessage.ToString();

			return _List(id, entityKind);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateNewRevision(int id)
		{
			if (Request.Files.Count != 1)
				return HTTPStatus(HttpStatusCode.BadRequest, "Keine Datei empfangen");

			var file = Request.Files[0];

			if (id <= 0)
				return HTTPStatus(HttpStatusCode.BadRequest, "Die Dateien können keinem Ziel zugeordnet werden.");

			var document = db.Documents.Find(id);

			Topic topic = null;
			if (document.TopicID != null)
			{
				topic = db.Topics.Find(document.TopicID.Value);

				if (IsTopicLocked(document.TopicID.Value))
					return HTTPStatus(HttpStatusCode.Forbidden, "Da das Thema gesperrt ist, können Sie keine Dateien hochladen.");

				if (topic.IsReadOnly)
				{
					return HTTPStatus(HttpStatusCode.Forbidden,
						"Da das Thema schreibgeschützt ist, können Sie keine Dateien bearbeiten.");
				}
			}

			if (string.IsNullOrWhiteSpace(file.FileName))
				return HTTPStatus(HttpStatusCode.BadRequest, "Die Datei hat einen ungültigen Dateinamen.");

			var fullName = Path.GetFileName(file.FileName);
			if (file.ContentLength == 0)
				return HTTPStatus(HttpStatusCode.BadRequest, "Datei " + fullName + " hat keinen Inhalt.");

			string filename = Path.GetFileNameWithoutExtension(file.FileName);
			string fileext = Path.GetExtension(file.FileName);
			if (!string.IsNullOrEmpty(fileext))
				fileext = fileext.Substring(1);

			var revision = new Revision
			{
				ParentDocument = document,
				SafeName = InvalidChars.Replace(filename, ""),
				FileSize = file.ContentLength,
				UploaderID = GetCurrentUserID(),
				Extension = fileext,
				GUID = Guid.NewGuid(),
				Created = DateTime.Now
			};
			document.Revisions.Add(revision);
			document.LatestRevision = revision;
			document.DisplayName = fullName;

			try
			{
				db.SaveChanges(); // Damit die Revision seine ID bekommt. Diese wird anschließend im Dateinamen hinterlegt
				string path = Path.Combine(Serverpath, revision.FileName);
				file.SaveAs(path);
				//db.SaveChanges();
			}
			catch (DbEntityValidationException ex)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, ErrorMessageFromException(ex));
			}
			catch (IOException ex)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, "Dateisystemfehler: " + ex.Message);
			}

			// Ungelesen-Markierung aktualisieren
			if (topic != null)
				MarkAsUnread(topic);

			return HTTPStatus(HttpStatusCode.Created, Url.Action("Details", "Attachments", new {Area = "", id}));
		}

		[HttpPost]
		public ActionResult _Delete(int documentID)
		{
			var document = db.Documents.Include(d => d.LatestRevision).Single(d => d.ID == documentID);

			if (document.Deleted != null)
				return HTTPStatus(422, "Das Objekt befindet sich bereits im Papierkorb.");

			if (document.TopicID.HasValue && IsTopicLocked(document.TopicID.Value))
				return HTTPStatus(HttpStatusCode.Forbidden, "Da das Thema gesperrt ist, können Sie keine Dateien bearbeiten.");

			if (document.TopicID.HasValue && document.Topic.IsReadOnly)
			{
				return HTTPStatus(HttpStatusCode.Forbidden,
					"Da das Thema schreibgeschützt ist, können Sie keine Dateien bearbeiten.");
			}


			document.Deleted = DateTime.Now; // In den Papierkorb
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

		public ActionResult _PermanentDelete(int documentID)
		{
			var document = db.Documents.Include(d => d.LatestRevision).Single(d => d.ID == documentID);

			if (document.Deleted == null) // In den Papierkorb
				return HTTPStatus(422, "Das Objekt befindet sich noch nicht im Papierkorb.");

			try
			{
				foreach (var revision in document.Revisions)
				{
					string path = Path.Combine(Serverpath, revision.FileName);
					System.IO.File.Delete(path);
				}
			}
			catch (IOException ex)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, ex.Message);
			}

			try
			{
				document.LatestRevisionID = null;
				db.Revisions.RemoveRange(document.Revisions);
				db.SaveChanges();
				db.Documents.Remove(document);
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				var message = ErrorMessageFromException(e);
				return HTTPStatus(HttpStatusCode.InternalServerError, message);
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}

		/// <summary>
		///    Download oder öffnen einer Datei. Öffnen von Dokumenten funktioniert nur im Internet Explorer.
		/// </summary>
		/// <param name="id">ID der Datei</param>
		/// <returns></returns>
		[AllowAnonymous]
		public ActionResult Download(Guid id)
		{
			var file = db.Revisions.Include(rev => rev.ParentDocument).Single(rev => rev.GUID == id);

			var userAgent = Request.UserAgent;
			var isInternetExplorer = !string.IsNullOrEmpty(userAgent) && userAgent.Contains("Trident");
			var isAuthenticated = User.Identity != null; // TODO: Testen
			var isOfficeDocument = OfficeExtensions.Contains(file.Extension);

			if (isAuthenticated && isOfficeDocument && isInternetExplorer)
			{
				var host = Dns.GetHostName() + ".iwb.mw.tu-muenchen.de";
				return Redirect("file://" + host + "/Uploads/" + file.FileName);
			}
			else
			{
				var cd = new ContentDisposition
				{
					FileName = file.ParentDocument.DisplayName,
					Inline = true,
				};
				Response.AppendHeader("Content-Disposition", cd.ToString());

				return File(Path.Combine(Serverpath, file.FileName), MimeMapping.GetMimeMapping(file.FileName));
			}
		}

		/// <summary>
		///    Download der neuesten Version eines Dokuments.
		/// </summary>
		/// <param name="id">ID des Dokuments</param>
		/// <returns></returns>
		[AllowAnonymous]
		public ActionResult DownloadNewest(Guid id)
		{
			var document = db.Documents.Single(doc => doc.GUID == id);
			return Download(document.LatestRevision.GUID);
		}

		public ActionResult Details(int id)
		{
			Document d = db.Documents.Find(id);
			if (d == null)
				return HTTPStatus(HttpStatusCode.NotFound, "Datei nicht gefunden");

			ViewBag.ShowUpload = true;
			if (d.TopicID != null)
			{
				var topic = db.Topics.Find(d.TopicID.Value);
				ViewBag.ShowUpload = !topic.IsReadOnly && !IsTopicLocked(d.TopicID.Value);
			}

			return View(d);
		}

		public ActionResult _BeginEdit(int documentID)
		{
			var a = db.Documents.Find(documentID);
			if (a.Topic != null && a.Topic.IsReadOnly)
				return HTTPStatus(HttpStatusCode.Forbidden, "Das Thema ist schreibgeschützt!");
			return PartialView("_NameEditor", a);
		}

		public PartialViewResult _FetchDisplayName(int documentID)
		{
			var document = db.Documents.Find(documentID);

			var url = new UrlHelper(ControllerContext.RequestContext).Action("DownloadNewest", "Attachments",
				new {id = document.GUID});
			return PartialView("_NameDisplay", Tuple.Create(new MvcHtmlString(url), document.DisplayName));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _SubmitEdit(int id, string displayName)
		{
			var document = db.Documents.Find(id);

			if (document.Topic != null && document.Topic.IsReadOnly)
				return HTTPStatus(HttpStatusCode.Forbidden, "Das Thema ist schreibgeschützt!");

			displayName = Path.ChangeExtension(displayName, Path.GetExtension(document.LatestRevision.FileName));
			document.DisplayName = displayName;
			db.SaveChanges();

			var url = new UrlHelper(ControllerContext.RequestContext).Action("DownloadNewest", "Attachments",
				new {id = document.GUID});
			return PartialView("_NameDisplay", Tuple.Create(new MvcHtmlString(url), document.DisplayName));
		}
	}
}