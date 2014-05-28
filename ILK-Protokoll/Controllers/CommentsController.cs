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
	public class CommentsController : Controller
	{
		private DataContext _db = new DataContext();

		private User GetCurrentUser()
		{
			string username = User.Identity.Name.Split('\\').Last();
			return _db.Users.Single(x => x.Name.Equals(username, StringComparison.CurrentCultureIgnoreCase));
		}

		public PartialViewResult _List(Topic t)
		{
			var comments = t.Comments.OrderBy(c => c.Created).ToList();
			ViewBag.TopicID = t.ID;

			var lastcomment = comments.Last();
			ViewBag.AllowDeletion = lastcomment.Author == GetCurrentUser() ? lastcomment.ID : -1;
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
				_db.Comments.Add(comment);
				_db.SaveChanges();

				Topic t = _db.Topics.Find(comment.TopicID);
				return _List(t);
			}
			this.ControllerContext.HttpContext.Response.StatusCode = 400;
			return new EmptyResult();
		}


		// GET: Comments/Delete/5
		public ActionResult _Delete(int id)
		{
			Comment comment = _db.Comments.Find(id);
			Topic t = _db.Topics.Find(comment.TopicID);
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
				_db.Comments.Remove(comment);
				_db.SaveChanges();

				return _List(t);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
