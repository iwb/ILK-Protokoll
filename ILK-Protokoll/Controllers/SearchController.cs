using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using ILK_Protokoll.Models;
using WebGrease.Css.Extensions;

namespace ILK_Protokoll.Controllers
{
	public class SearchController : BaseController
	{
		// GET: Search
		public ActionResult Results(string searchterm)
		{
			if (string.IsNullOrWhiteSpace(searchterm))
				return RedirectToAction("Search");

			var score = new Dictionary<Topic, int>(new TopicByIdComparer());

			foreach (var term in searchterm.Split(' '))
			{
				// ReSharper disable AccessToForEachVariableInClosure
				foreach (var topic in db.Topics.Where(t => t.Title.Contains(term)))
					AddOrIncrement(score, topic, 20);

				foreach (var topic in db.Topics.Where(t => t.Proposal.Contains(term)))
					AddOrIncrement(score, topic, 8);

				foreach (var topic in db.Topics.Where(t => t.Description.Contains(term)))
					AddOrIncrement(score, topic, 6);

				foreach (var comment in db.Comments.Where(c => c.Content.Contains(term)))
					AddOrIncrement(score, db.Topics.Find(comment.TopicID), 2);

				foreach (var attachment in db.Attachments.Where(a => a.Deleted == null && a.DisplayName.Contains(term)))
					AddOrIncrement(score, attachment.Topic, 1);
				// ReSharper restore AccessToForEachVariableInClosure
			}

			//foreach (var kvp in score)
			//	kvp.Key.Description += " Score: " + kvp.Value;

			var results = score.OrderByDescending(t => t.Value).Select(t => t.Key).ToList();
			ViewBag.SearchTerm = searchterm;
			return View(results);
		}

		private void AddOrIncrement<TKey>(IDictionary<TKey, int> dict, TKey item, int increment)
		{
			int count;
			if (dict.TryGetValue(item, out count))
				dict[item] = count + increment;
			else
				dict.Add(item, increment);
		}

		public ActionResult Search()
		{
			return View();
		}
	}
}