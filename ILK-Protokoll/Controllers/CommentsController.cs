using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;
using System.Collections.Generic;

namespace ILK_Protokoll.Controllers
{
	public class CommentsController : BaseController
	{
		public PartialViewResult _List(Topic t)
		{
			var comments = t.Comments.OrderBy(c => c.Created).ToList();
			ViewBag.TopicID = t.ID;

			var lastcomment = comments.LastOrDefault();
			ViewBag.AllowDeletion = (lastcomment != null) && (lastcomment.Author == GetCurrentUser()) ? lastcomment.ID : -1;

			return PartialView("_Listing", comments);
		}

		public PartialViewResult _CreateForm(int TopicID)
		{
			var c = new Comment() { TopicID = TopicID };
			return PartialView("_CreateForm", c);
		}

		// POST: Comments/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _Submit([Bind(Include = "TopicID,Content")] Comment comment)
		{
			if (ModelState.IsValid && !string.IsNullOrWhiteSpace(comment.Content))
			{
				comment.Created = DateTime.Now;
				comment.Author = GetCurrentUser();
				db.Comments.Add(comment);
				db.SaveChanges();

				Topic t = db.Topics.Find(comment.TopicID);
				return _List(t);
			}
			this.ControllerContext.HttpContext.Response.StatusCode = 400;
			return new EmptyResult();
		}


		// GET: Comments/Delete/5
		public ActionResult _Delete(int id)
		{
			Comment comment = db.Comments.Find(id);
			Topic t = db.Topics.Find(comment.TopicID);
			var lastcomment = t.Comments.OrderBy(c => c.Created).Last();

			if (comment == null)
			{
				return HttpNotFound();
			}
			else if (id != lastcomment.ID || lastcomment.Author != GetCurrentUser())
			{
				this.ControllerContext.HttpContext.Response.StatusCode = 400;
				return new EmptyResult();
			}
			else
			{
				db.Comments.Remove(comment);
				db.SaveChanges();

				return _List(t);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
