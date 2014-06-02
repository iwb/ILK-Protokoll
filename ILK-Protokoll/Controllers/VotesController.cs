using ILK_Protokoll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ILK_Protokoll.Controllers
{
	public class VotesController : BaseController
	{
		public ActionResult _List(Topic t)
		{
			ViewBag.ownvote = t.Votes.SingleOrDefault(v => v.Voter == GetCurrentUser());
			ViewBag.TopicID = t.ID;
			ViewBag.CurrentUser = GetCurrentUser();

			var displayvotes = t.Votes.Where(v => v.Voter != GetCurrentUser())
				.OrderBy(v => v.Voter.Name, StringComparer.CurrentCultureIgnoreCase);

			return PartialView("_Listing", displayvotes);
		}
		public ActionResult _Register(int TopicID, VoteKind vote)
		{
			var cuid = GetCurrentUser().ID;
			db.Votes.First(v => v.Voter.ID == cuid && v.Topic.ID == TopicID).Kind = vote;
			db.SaveChanges();
			return _List(db.Topics.Find(TopicID));
		}
	}
}