using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
	public class CommentsController : BaseController
	{
		public PartialViewResult _List(Topic topic)
		{
			List<Comment> comments = topic.Comments.OrderBy(c => c.Created).ToList();
			ViewBag.TopicID = topic.ID;

			Comment lastcomment = comments.LastOrDefault();
			ViewBag.AllowDeletion = (lastcomment != null) && (lastcomment.Author == GetCurrentUser()) ? lastcomment.ID : -1;

			return PartialView("_CommentList", comments);
		}

		public PartialViewResult _CreateForm(int TopicID)
		{
			var c = new Comment { TopicID = TopicID };
			ViewBag.TopicID = TopicID;
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
			else
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
					"Dieser Kommentar kann nicht erstellt werden: Es fehlen Informationen.");
			}
		}


		// GET: Comments/Delete/5
		public ActionResult _Delete(int id)
		{
			Comment comment = db.Comments.Find(id);
			if (comment == null)
				return HttpNotFound();

			Topic t = db.Topics.Find(comment.TopicID);
			Comment lastcomment = t.Comments.OrderBy(c => c.Created).Last();

			if (id != lastcomment.ID)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
					"Dieser Kommentar kann nicht gelöscht werden: Er ist nicht der letzte Kommentar der Diskussion.");
			}
			else if (lastcomment.Author != GetCurrentUser())
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
					"Dieser Kommentar kann nicht gelöscht werden: Sie sind nicht der Autor des Kommentars.");
			}
			else
			{
				db.Comments.Remove(comment);
				db.SaveChanges();

				return _List(t);
			}
		}
	}
}