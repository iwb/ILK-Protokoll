using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Controllers
{
	public class VotesController : BaseController
	{
		public ActionResult _List(Topic topic, VoteLinkLevel linkLevel)
		{
			if (!ModelState.IsValid)
				throw new InvalidOperationException("The Topic is invalid.");

			if (linkLevel > VoteLinkLevel.OnlyMine && IsTopicLocked(topic.ID))
				linkLevel = VoteLinkLevel.OnlyMine;

			if (topic.IsReadOnly)
				linkLevel = VoteLinkLevel.None;

			var vm = new VoteListViewModel
			{
				TopicID = topic.ID,
				OwnVote = topic.Votes.SingleOrDefault(v => v.Voter.Equals(GetCurrentUser())),
				LinkLevel = linkLevel,
				OtherVotes = topic.Votes
					.Where(v => !v.Voter.Equals(GetCurrentUser()))
					.OrderBy(v => v.Voter.ShortName, StringComparer.CurrentCultureIgnoreCase)
					.ToList()
			};
			return PartialView("_VoteList", vm);
		}

		public ActionResult _Register(int topicID, VoteKind vote, VoteLinkLevel linkLevel)
		{
			if (db.Topics.Find(topicID).IsReadOnly)
				return HTTPStatus(HttpStatusCode.Forbidden, "Das Thema ist schreibgeschützt.");

			int cuid = GetCurrentUserID();
			Vote dbvote = db.Votes.SingleOrDefault(v => v.Voter.ID == cuid && v.Topic.ID == topicID);

			if (dbvote == null)
				return HTTPStatus(HttpStatusCode.Forbidden, "Es liegt keine Stimmberechtigung vor.");

			dbvote.Kind = vote;
			db.SaveChanges();
			return _List(db.Topics.Find(topicID), linkLevel);
		}

		public ActionResult _Register2(int topicID, int voterID, VoteKind vote, VoteLinkLevel linkLevel)
		{
			if (db.Topics.Find(topicID).IsReadOnly)
				return HTTPStatus(HttpStatusCode.Forbidden, "Das Thema ist schreibgeschützt.");

			if (IsTopicLocked(topicID))
				return HTTPStatus(HttpStatusCode.Forbidden, "Das Thema ist gesperrt.");

			User voter = db.Users.Find(voterID);
			db.Votes.Single(v => v.Voter.ID == voter.ID && v.Topic.ID == topicID).Kind = vote;

			string message = string.Format("{0} hat in Vertretung für {1} abgestimmt mit \"{2}\".", GetCurrentUser().ShortName,
				voter.ShortName,
				vote.DisplayName());

			db.Comments.Add(new Comment {Author = db.Users.Find(voterID), TopicID = topicID, Content = message});
			db.SaveChanges();
			return _List(db.Topics.Find(topicID), linkLevel);
		}
	}
}