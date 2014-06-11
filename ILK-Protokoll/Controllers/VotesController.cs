using System;
using System.Linq;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;

namespace ILK_Protokoll.Controllers
{
	public class VotesController : BaseController
	{
		public ActionResult _List(Topic t, bool linkAllAuditors = false)
		{
			ViewBag.ownvote = t.Votes.SingleOrDefault(v => v.Voter == GetCurrentUser());
			ViewBag.TopicID = t.ID;
			ViewBag.CurrentUser = GetCurrentUser();
			ViewBag.LinkAllAuditors = linkAllAuditors;

			IOrderedEnumerable<Vote> displayvotes = t.Votes.Where(v => v.Voter != GetCurrentUser())
				.OrderBy(v => v.Voter.Name, StringComparer.CurrentCultureIgnoreCase);

			return PartialView("_VoteList", displayvotes);
		}

		public ActionResult _Register(int topicID, VoteKind vote)
		{
			int cuid = GetCurrentUser().ID;
			db.Votes.First(v => v.Voter.ID == cuid && v.Topic.ID == topicID).Kind = vote;
			db.SaveChanges();
			return _List(db.Topics.Find(topicID));
		}

		public ActionResult _Register2(int topicID, int voterID, VoteKind vote)
		{
			User voter = db.Users.Find(voterID);
			db.Votes.First(v => v.Voter.ID == voter.ID && v.Topic.ID == topicID).Kind = vote;

			string message = string.Format("In Vertretung für {0} abgestimmt mit \"{1}\".", voter.Name, vote.GetDescription());

			db.Comments.Add(new Comment { Author = GetCurrentUser(), TopicID = topicID, Content = message });
			db.SaveChanges();
			return _List(db.Topics.Find(topicID));
		}
	}
}