using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class AttachmentsController : BaseController
	{
		// GET: Attachments
		public ActionResult _List(int? topicID)
		{
			var files = db.Attachments.Where(a => a.TopicID == topicID).Include(a => a.Uploader).ToList();
			ViewBag.TopicID = topicID;
			ViewBag.CurrentUser = GetCurrentUser();
			return PartialView("_AttachmentList", files);
		}

		public PartialViewResult _UploadForm(int topicID)
		{
			var a = new Attachment() { TopicID = topicID };
			return PartialView("_UploadForm", a);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _Upload(int topicID, HttpPostedFileBase file)
		{
			if (file != null && file.ContentLength > 0)
			{
				var fileName = Path.GetFileName(file.FileName);
				Debug.Assert(fileName != null, "fileName == null");
				var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
				file.SaveAs(path);

				return _UploadForm(topicID);
			}
			else
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Es wurde keine Datei erhalten.");
		}
	}
}
