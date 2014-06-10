﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
		public ActionResult _Upload(int topicID, IEnumerable<HttpPostedFileBase> files)
		{
			bool success = false;

			foreach (HttpPostedFileBase file in files)
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
					"Der Server hat keine Datei erhalten, oder kann Sie keinem Thema zuordnen.");
		}


		public ActionResult _Delete(int attachmentID, int? topicID)
		{
			Attachment attachment = db.Attachments.Include(a => a.Uploader).First(a => a.ID == attachmentID);
			if (topicID.HasValue && attachment.TopicID == topicID) // In den Papierkorb
			{
				db.Topics.Find(topicID).Attachments.Remove(attachment);
				attachment.TopicID = null;
			}
			else if (topicID == null && attachment.TopicID == null) // Endgültig löschen
			{
				try
				{
					string path = Path.Combine(Serverpath, attachment.FileName);
					System.IO.File.Delete(path);
					db.Attachments.Remove(attachment);
				}
				catch (IOException)
				{
					return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
				}
			}
			else
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Die Daten konnten nicht zugordnet werden.");

			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException e)
			{
				return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
			}

			return new HttpStatusCodeResult(HttpStatusCode.NoContent);
		}
	}
}