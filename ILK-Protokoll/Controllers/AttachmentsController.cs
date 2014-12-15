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
using EntityFramework.Extensions;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;
using JetBrains.Annotations;

namespace ILK_Protokoll.Controllers
{
	public class AttachmentsController : BaseController
	{
		private static readonly Regex InvalidChars = new Regex(@"[^a-zA-Z0-9_-]");

		/// <summary>
		///    Diese Erweiterungen sind charakteristisch für MS-Office Dateien. Wird die Seite im Internet-Explorer genutzt, werden
		///    diese Erweiterungen zum direkten Öffnen angeboten.
		/// </summary>
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")] public static HashSet<string>
			KnownExtensions = new HashSet<string>();

		private static string Serverpath
		{
			get { return @"C:\ILK-Protokoll_Uploads\"; }
		}

		private static string TemporaryServerpath
		{
			get { return @"C:\ILK-Protokoll_Temp\"; }
		}

#if DEBUG
		private readonly string _hostname = Dns.GetHostName();
#else
		private readonly string _hostname = Dns.GetHostName() + ".iwb.mw.tu-muenchen.de";
#endif

		private bool isInternetExplorer
		{
			get
			{
				var userAgent = Request.UserAgent;
				return !string.IsNullOrEmpty(userAgent) && userAgent.Contains("Trident");
			}
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
			ViewBag.OfficeExtensions = OfficeExtensions;
			ViewBag.SeamlessEnabled = isInternetExplorer;

			if (makeList)
				return PartialView("_AttachmentList", documents.ToList());
			else
			{
				return showActions
					? PartialView("_AttachmentTable", documents.ToList())
					: PartialView("~/Areas/Session/Views/Finalize/_ReportAttachments.cshtml", documents.ToList());
			}
		}

		public ActionResult Details(int id)
		{
			var document = db.Documents.Include(d => d.LockUser).Single(doc => doc.ID == id);
			if (document == null)
				return HTTPStatus(HttpStatusCode.NotFound, "Datei nicht gefunden");

			ViewBag.ShowUpload = document.LockTime == null;
			if (ViewBag.ShowUpload && document.TopicID != null)
			{
				var topic = db.Topics.Find(document.TopicID.Value);
				ViewBag.ShowUpload = !topic.IsReadOnly && !IsTopicLocked(document.TopicID.Value);
			}

			ViewBag.SeamlessEnabled = isInternetExplorer && OfficeExtensions.Contains(document.LatestRevision.Extension);
			if (document.LockUserID == GetCurrentUserID())
				ViewBag.TempFileURL = "file://" + _hostname + "/Temp/" + document.Revisions.OrderByDescending(r => r.Created).First().FileName;

			return View(document);
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
			if (file == null)
				return HTTPStatus(HttpStatusCode.BadRequest, "Keine Datei empfangen");

			// Checks
			Document document;
			Topic topic;
			var actionResult = CheckConstraints(id, out topic, out document);
			if (actionResult != null)
				return actionResult;
			//---------------------------------------------------------------

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

		/// <summary>
		///    Prüft verschiedene Kriterien, nach denen das Dokument bearbeitbar ist. Bei Einem Fehler ist der Rückgabewert
		///    ungleich null.
		/// </summary>
		/// <param name="documentID">die ID des Dokuments</param>
		/// <param name="topic">Das zugeordnete Thema, falls eines existiert.</param>
		/// <param name="document">Das Doukument, das der ID zugeordnet ist.</param>
		/// <returns></returns>
		private ActionResult CheckConstraints(int documentID, [CanBeNull] out Topic topic, out Document document)
		{
			topic = null;
			document = db.Documents.Find(documentID);

			if (document == null)
				return HTTPStatus(HttpStatusCode.NotFound, "Dokument-ID nicht gefunden.");

			if (document.LockTime != null)
				return HTTPStatus(HttpStatusCode.Forbidden, "Das Dokument ist derzeit gesperrt.");

			if (document.TopicID == null)
				return null; // Alle Checks bestanden

			topic = db.Topics.Find(document.TopicID.Value);

			if (IsTopicLocked(document.TopicID.Value))
				return HTTPStatus(HttpStatusCode.Forbidden, "Da das Thema gesperrt ist, können Sie keine Dateien hochladen.");

			if (topic.IsReadOnly)
			{
				return HTTPStatus(HttpStatusCode.Forbidden,
					"Da das Thema schreibgeschützt ist, können Sie keine Dateien bearbeiten.");
			}
			return null;
		}

		/// <summary>
		///    Beginnt die nahtlose Bearbeitung eines Dokuments. Hierzu wird die aktuelle Revision kopiert und dem Anwender werden
		///    Schreibrechte eingeräumt. Nach Abschluss seiner Bearbeitung muss der Anwender speichern, um die neue Revision zur
		///    aktuellen zu machen.
		/// </summary>
		/// <param name="id">Die ID des Dokuments, zu dem eine neue Revision erzeugt werden soll.</param>
		//[HttpPost]
		public ActionResult BeginNewRevision(int id)
		{
			// Checks
			Document document;
			Topic topic;
			var actionResult = CheckConstraints(id, out topic, out document);
			if (actionResult != null)
				return actionResult;
			
			if (!isInternetExplorer || !OfficeExtensions.Contains(document.LatestRevision.Extension))
				return HTTPStatus(HttpStatusCode.BadRequest, "Dieser Vorgang ist nur mit MSIE und Office-Dokumenten zulässig.");
			//---------------------------------------------------------------

			var now = DateTime.Now;

			document.LockUserID = GetCurrentUserID();
			document.LockTime = now;

			var revision = new Revision
			{
				ParentDocument = document,
				SafeName = document.LatestRevision.SafeName,
				FileSize = 0,
				UploaderID = GetCurrentUserID(),
				Extension = document.LatestRevision.Extension,
				GUID = Guid.NewGuid(),
				Created = now.AddMilliseconds(100)
			};
			document.Revisions.Add(revision);

			try
			{
				db.SaveChanges(); // Damit die Revision seine ID bekommt. Diese wird anschließend im Dateinamen hinterlegt
				var sourcePath = Path.Combine(Serverpath, document.LatestRevision.FileName);
				var destPath = Path.Combine(TemporaryServerpath, revision.FileName);
				System.IO.File.Copy(sourcePath, destPath);
			}
			catch (DbEntityValidationException ex)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, ErrorMessageFromException(ex));
			}
			catch (IOException ex)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError, "Dateisystemfehler: " + ex.Message);
			}
			return HTTPStatus(HttpStatusCode.Created, "file://" + _hostname + "/Temp/" + revision.FileName);
		}

		//[HttpPost]
		public ActionResult CancelNewRevision(int id)
		{
			try
			{
				ForceReleaseLock(id);
			}
			catch (IOException)
			{
				// The file is still in use
				return HTTPStatus(HttpStatusCode.Conflict, "Dieser Vorgang kann nicht ausgeführt werden, da die Datei noch in Verwendung ist. Bitte schließen Sie die Datei und versuchen Sie es erneut.");
			}
			return RedirectToAction("Details", new {id});
		}

		public static void ForceReleaseLock(int documentID)
		{
			using (var db = new DataContext())
			{
				var doc = db.Documents.Find(documentID); 
				var cutoff = doc.LatestRevision.Created;
				var unused = doc.Revisions.Where(r => r.Created > cutoff).ToArray();

				if (unused.Length <= 0)
					return;

				foreach (var revision in unused)
					System.IO.File.Delete(Path.Combine(TemporaryServerpath, revision.FileName));

				var unusedids = unused.Select(r => r.ID).ToArray();
				db.Revisions.Where(r => unusedids.Contains(r.ID)).Delete();
				
				doc.LockTime = null;
				doc.LockUserID = null;
				db.SaveChanges();
			}
		}

		//[HttpPost]
		public ActionResult FinishNewRevision(int id)
		{
			// Checks
			var document = db.Documents.Find(id);

			if (document == null)
				return HTTPStatus(HttpStatusCode.NotFound, "Dokument-ID nicht gefunden.");

			if (document.LockTime == null)
				return HTTPStatus(HttpStatusCode.Forbidden, "Das Dokument ist derzeit nicht in Bearbeitung.");

			if (document.TopicID != null)
			{
				var topic = db.Topics.Find(document.TopicID.Value);

				if (IsTopicLocked(document.TopicID.Value))
					return HTTPStatus(HttpStatusCode.Forbidden, "Da das Thema gesperrt ist, können Sie keine Dateien hochladen.");

				if (topic.IsReadOnly)
				{
					return HTTPStatus(HttpStatusCode.Forbidden,
						"Da das Thema schreibgeschützt ist, können Sie keine Dateien bearbeiten.");
				}
			}

			if (GetCurrentUserID() != document.LockUserID)
				return HTTPStatus(HttpStatusCode.Forbidden, "Das Dokument ist auf einen anderen Nutzer gesperrt, Sie sind nicht autorisiert.");
			//---------------------------------------------------------------

			var newrevision = document.Revisions.OrderByDescending(r => r.Created).First();

			var sourcePath = Path.Combine(TemporaryServerpath, newrevision.FileName);
			var destPath = Path.Combine(Serverpath, newrevision.FileName);

			if (newrevision.ID == document.LatestRevisionID || !System.IO.File.Exists(sourcePath))
				return HTTPStatus(HttpStatusCode.InternalServerError, "Vorgang kann nicht abgeschlossen werden, da das Dokument zwar gesperrt, aber die Revison nicht in Bearbeitung ist.");

			newrevision.FileSize = (int)new FileInfo(sourcePath).Length;
			newrevision.Created = DateTime.Now;
			try
			{
				System.IO.File.Move(sourcePath, destPath);
			}
			catch (IOException)
			{
				// The file is still in use
				return HTTPStatus(HttpStatusCode.Conflict, "Dieser Vorgang kann nicht ausgeführt werden, da die Datei noch in Verwendung ist. Bitte schließen Sie die Datei und versuchen Sie es erneut.");
			}

			document.LatestRevisionID = newrevision.ID;
			document.LockTime = null;
			document.LockUserID = null;
			db.SaveChanges();

			return RedirectToAction("Details", new {id});
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

			var isAuthenticated = User.Identity != null;

			if (isAuthenticated && OfficeExtensions.Contains(file.Extension) && isInternetExplorer)
				return Redirect("file://" + _hostname + "/Uploads/" + file.FileName);
			else
			{
				var cd = new ContentDisposition
				{
					FileName = file.ParentDocument.DisplayName,
					Inline = true
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

		public string FetchNewestRevURL(int documentID)
		{
			var file = db.Documents.Find(documentID).LatestRevision;
			return "file://" + _hostname + "/Uploads/" + file.FileName;
		}

		public PartialViewResult _FetchTableRow(int documentID)
		{
			var document = db.Documents.Find(documentID);

			ViewBag.KnownExtensions = KnownExtensions;
			ViewBag.OfficeExtensions = OfficeExtensions;
			ViewBag.SeamlessEnabled = isInternetExplorer;

			return PartialView("_AttachmentRow", document);
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