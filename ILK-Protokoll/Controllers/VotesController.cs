using ILK_Protokoll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

			var displayvotes = t.Votes.Where(v => v.Voter != GetCurrentUser())
				.OrderBy(v => v.Voter.Name, StringComparer.CurrentCultureIgnoreCase);

			return PartialView("_VoteList", displayvotes);
		}
		public ActionResult _Register(int TopicID, VoteKind vote)
		{
			var cuid = GetCurrentUser().ID;
			db.Votes.First(v => v.Voter.ID == cuid && v.Topic.ID == TopicID).Kind = vote;
			db.SaveChanges();
			return _List(db.Topics.Find(TopicID));
		}
		public ActionResult _Register2(int TopicID, int VoterID, VoteKind vote)
		{
			var voter = db.Users.Find(VoterID);
			db.Votes.First(v => v.Voter.ID == voter.ID && v.Topic.ID == TopicID).Kind = vote;

			var message = string.Format("In Vertretung für {0} abgestimmt mit \"{1}\".", voter.Name, vote.GetDescription());

			db.Comments.Add(new Comment() { Author = GetCurrentUser(), TopicID = TopicID, Content = message });
			db.SaveChanges();
			return _List(db.Topics.Find(TopicID));
		}
	}
}