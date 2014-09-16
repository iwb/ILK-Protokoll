using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
			var comments = db.Comments.Include(c => c.Author).Where(c => c.TopicID == topic.ID).OrderBy(c => c.Created).ToList();
			ViewBag.TopicID = topic.ID;
			ViewBag.ShowCreateForm = !topic.IsReadOnly && !IsTopicLocked(topic);

			Comment lastcomment = comments.LastOrDefault();
			ViewBag.AllowDeletion = (lastcomment != null) && !topic.IsReadOnly && (lastcomment.AuthorID == GetCurrentUserID())
				? lastcomment.ID
				: -1;

			return PartialView("_CommentList", comments);
		}

		public ActionResult _CreateForm(int topicID)
		{
			var c = new Comment {TopicID = topicID};
			ViewBag.TopicID = topicID;
			return PartialView("_CreateForm", c);
		}

		// POST: Comments/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult _Submit([Bind(Include = "TopicID,Content")] Comment comment)
		{
			if (string.IsNullOrWhiteSpace(comment.Content))
				return HTTPStatus(422, "Der Kommentar enthält keinen Text.");

			if (!ModelState.IsValid)
			{
				return HTTPStatus(HttpStatusCode.InternalServerError,
					"Dieser Kommentar kann nicht erstellt werden: Es fehlen Informationen.");
			}

			var topic = db.Topics
				.Include(t => t.Lock)
				.Include(t => t.Lock.Session.Manager)
				.Single(t => t.ID == comment.TopicID);

			if (topic.IsReadOnly || IsTopicLocked(topic))
				throw new TopicLockedException();

			comment.Created = DateTime.Now;
			comment.AuthorID = GetCurrentUserID();
			comment.Content = comment.Content.Trim();
			db.Comments.Add(comment);

			// Ungelesen-Markierung aktualisieren
			MarkAsUnread(topic);

			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException ex)
			{
				var message = ErrorMessageFromException(ex);
				return HTTPStatus(HttpStatusCode.InternalServerError, message);
			}

			return _List(topic);
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
			else if (!lastcomment.Author.Equals(GetCurrentUser()))
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
					"Dieser Kommentar kann nicht gelöscht werden: Sie sind nicht der Autor des Kommentars.");
			}
			else if (IsTopicLocked(comment.TopicID))
				throw new TopicLockedException();
			else
			{
				db.Comments.Remove(comment);
				db.SaveChanges();

				return _List(t);
			}
		}
	}
}