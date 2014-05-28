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
			ViewBag.TopicID = t.ID;
			return PartialView("_Listing", t.Comments);
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
			if (ModelState.IsValid)
			{
				comment.Created = DateTime.Now;
				comment.Author = GetCurrentUser();
				_db.Comments.Add(comment);
				_db.SaveChanges();

				Topic t = _db.Topics.Find(comment.TopicID);
				ViewBag.TopicID = t.ID;
				return PartialView("_Listing", t.Comments);
			}
			this.ControllerContext.HttpContext.Response.StatusCode = 400;
			return new EmptyResult();
		}


		// GET: Comments/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Comment comment = _db.Comments.Find(id);
			if (comment == null)
			{
				return HttpNotFound();
			}
			return View(comment);
		}

		// POST: Comments/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Comment comment = _db.Comments.Find(id);
			_db.Comments.Remove(comment);
			_db.SaveChanges();
			return RedirectToAction("Index");
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
