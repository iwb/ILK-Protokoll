using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;

namespace ILK_Protokoll.Controllers
{
	public class VotesController : BaseController
	{
		public ActionResult _List(Topic topic, bool linkAllAuditors = false)
		{
			if (!ModelState.IsValid)
				throw new InvalidOperationException("The Topic is invalid.");

			ViewBag.ownvote = topic.Votes.SingleOrDefault(v => v.Voter.Equals(GetCurrentUser()));
			ViewBag.TopicID = topic.ID;
			ViewBag.CurrentUser = GetCurrentUser();
			ViewBag.LinkAllAuditors = linkAllAuditors && !IsTopicLocked(topic.ID);

			IOrderedEnumerable<Vote> displayvotes = topic.Votes.Where(v => !v.Voter.Equals(GetCurrentUser()))
				.OrderBy(v => v.Voter.ShortName, StringComparer.CurrentCultureIgnoreCase);

			return PartialView("_VoteList", displayvotes);
		}

		public ActionResult _Register(int topicID, VoteKind vote, bool linkAllAuditors = false)
		{
			int cuid = GetCurrentUser().ID;
			Vote dbvote = db.Votes.SingleOrDefault(v => v.Voter.ID == cuid && v.Topic.ID == topicID);

			if (dbvote == null)
				return HTTPStatus(HttpStatusCode.Forbidden, "Es liegt keine Stimmberechtigung vor.");

			dbvote.Kind = vote;
			db.SaveChanges();
			return _List(db.Topics.Find(topicID), linkAllAuditors);
		}

		public ActionResult _Register2(int topicID, int voterID, VoteKind vote, bool linkAllAuditors = false)
		{
			if (IsTopicLocked(topicID))
				return HTTPStatus(HttpStatusCode.Forbidden, "Das Thema ist gesperrt.");

			User voter = db.Users.Find(voterID);
			db.Votes.Single(v => v.Voter.ID == voter.ID && v.Topic.ID == topicID).Kind = vote;

			string message = string.Format("In Vertretung für {0} abgestimmt mit \"{1}\".", voter.ShortName,
				vote.GetDescription());

			db.Comments.Add(new Comment {Author = GetCurrentUser(), TopicID = topicID, Content = message});
			db.SaveChanges();
			return _List(db.Topics.Find(topicID), linkAllAuditors);
		}
	}
}