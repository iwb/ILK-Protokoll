using System;
using System.Collections.Generic;
using System.Data.Entity;
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
		private static readonly Regex InvalidChars = new Regex(@"[^a-zA-Z0-9_-]");

		private string Serverpath
		{
			get { return Server.MapPath("~/App_Data/uploads/"); }
		}

		// GET: Attachments
		public ActionResult _List(int? topicID)
		{
			List<Attachment> files = db.Attachments
				.Where(a => a.TopicID == topicID)
				.OrderBy(a => a.DisplayName)
				.Include(a => a.Uploader)
				.ToList();
			ViewBag.TopicID = topicID;
			ViewBag.CurrentUser = GetCurrentUser();
			ViewBag.KnownExtensions = new HashSet<string>(
				from path in Directory.GetFiles(Server.MapPath("~/img/fileicons"), "*.png")
				select Path.GetFileNameWithoutExtension(path));

			return PartialView("_AttachmentList", files);
		}

		public PartialViewResult _UploadForm(int topicID)
		{
			return PartialView("_UploadForm", topicID);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _Upload(int topicID, HttpPostedFileBase file)
		{
			if (file != null && file.ContentLength > 0 && topicID > 0)
			{
				string filename = Path.GetFileNameWithoutExtension(file.FileName);
				string fileext = Path.GetExtension(file.FileName).Substring(1);

				var attachment = new Attachment
				{
					TopicID = topicID,
					DisplayName = file.FileName,
					SafeName = InvalidChars.Replace(filename, ""),
					Extension = fileext,
					FileSize = file.ContentLength,
					Uploader = GetCurrentUser(),
					Created = DateTime.Now
				};
				db.Attachments.Add(attachment);
				db.SaveChanges();

				string path = Path.Combine(Serverpath, attachment.FileName);
				file.SaveAs(path);

				return _UploadForm(topicID);
			}
			else
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
					"Der Server hat keine Datei erhalten, oder kann Sie keinem Thema zuordnen.");
		}
	}
}