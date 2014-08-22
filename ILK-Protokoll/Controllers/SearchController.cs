using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Controllers
{
	public class SearchController : BaseController
	{
		// GET: Search
		public ActionResult Results(string searchterm)
		{
			if (string.IsNullOrWhiteSpace(searchterm))
				return RedirectToAction("Search");

			var words = searchterm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(Regex.Escape);

			var searchpattern = @"\b(" + words.Aggregate((a, b) => a + "|" + b) + ")";
			var regex = new Regex(searchpattern, RegexOptions.IgnoreCase);


			var results = new List<SearchResult>();

			SearchTopics(regex, results);
			SearchComments(regex, results);
			SearchAssignments(regex, results);
			SearchDecisions(regex, results);

			ViewBag.SearchTerm = searchterm;
			ViewBag.SearchPattern = searchpattern;
			results.Sort((a, b) => b.Score.CompareTo(a.Score)); // Absteigend sortieren
			return View(results);
		}

		private void SearchTopics(Regex pattern, ICollection<SearchResult> resultlist)
		{
			foreach (var topic in db.Topics)
			{
				var sr = new SearchResult
				{
					Score = topic.IsReadOnly ? -5 : 0,
					EntityType = "Diskussion",
					Title = topic.Title,
					ActionURL = Url.Action("Details", "Topics", new { id = topic.ID }),
					Timestamp = topic.Created
				};

				var m = pattern.Matches(topic.Title);
				if (m.Count > 0)
				{
					sr.Score += ScoreMult(20, m.Count);
					sr.Hits.Add(new Hit
					{
						Property = "Titel",
						Text = topic.Title
					});
				}

				m = pattern.Matches(topic.Proposal);
				if (m.Count > 0)
				{
					sr.Score += ScoreMult(8, m.Count);
					sr.Hits.Add(new Hit
					{
						Property = "Beschlussvorschlag",
						Text = topic.Proposal
					});
				}

				m = pattern.Matches(topic.Description);
				if (m.Count > 0)
				{
					sr.Score += ScoreMult(6, m.Count);
					sr.Hits.Add(new Hit
					{
						Property = "Beschreibung",
						Text = topic.Description
					});
				}

				if (sr.Score > 0)
					resultlist.Add(sr);
			}
		}

		private void SearchComments(Regex pattern, ICollection<SearchResult> resultlist)
		{
			var query = from c in db.Comments
							join t in db.Topics on c.TopicID equals t.ID
							select new { topicID = t.ID, Text = c.Content, t.Title, c.Created };

			foreach (var comment in query)
			{
				var m = pattern.Matches(comment.Text);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult(comment.Text)
					{
						Score = ScoreMult(2, m.Count),
						EntityType = "Kommentar",
						Title = comment.Title,
						ActionURL = Url.Action("Details", "Topics", new { id = comment.topicID }),
						Timestamp = comment.Created
					});
				}
			}
		}

		private void SearchAssignments(Regex pattern, ICollection<SearchResult> resultlist)
		{
			foreach (var assignment in db.Assignments)
			{
				var m = pattern.Matches(assignment.Title);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult("Aufgabentitel", assignment.Title)
					{
						Score = ScoreMult(9, m.Count),
						EntityType = "Aufgabe",
						Title = assignment.Title,
						ActionURL = Url.Action("Details", "Assignments", new { id = assignment.ID }),
						Timestamp = assignment.DueDate
					});
					continue;
				}

				m = pattern.Matches(assignment.Description);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult("Aufgabentext", assignment.Description)
					{
						Score = ScoreMult(9, m.Count),
						EntityType = "Aufgabe",
						Title = assignment.Description,
						ActionURL = Url.Action("Details", "Assignments", new { id = assignment.ID }),
						Timestamp = assignment.DueDate
					});
				}

			}
		}

		private void SearchDecisions(Regex pattern, ICollection<SearchResult> resultlist)
		{
			foreach (var decision in db.Decisions.Include(d => d.OriginTopic).Include(d => d.Report))
			{
				var sr = new SearchResult
				{
					Score = decision.Type == DecisionType.Resolution ? 0 : -11,
					EntityType = "Entscheidung",
					Title = decision.OriginTopic.Title,
					ActionURL = Url.Action("Details", "Topics", new { id = decision.OriginTopic.ID }),
					Timestamp = decision.Report.End
				};

				var m = pattern.Matches(decision.OriginTopic.Title);
				if (m.Count > 0)
				{
					sr.Score += ScoreMult(21, m.Count);
					sr.Hits.Add(new Hit
					{
						Property = "Titel",
						Text = decision.OriginTopic.Title
					});
				}

				m = pattern.Matches(decision.Text);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult("Beschlusstext", decision.Text)
					{
						Score = ScoreMult(9, m.Count),
						EntityType = "Beschluss",
						Title = decision.OriginTopic.Title,
						ActionURL = Url.Action("Details", "Topics", new { id = decision.OriginTopic.ID }),
						Timestamp = decision.Report.End
					});
				}

				if (sr.Score <= 0)
					continue;

				sr.Score += 11;
				resultlist.Add(sr);
			}
		}

		private float ScoreMult(int baseScore, int count)
		{
			if (baseScore == 0 || count == 0)
				return 0;
			else
				return baseScore * (float)(5 - 4 * Math.Pow(0.8, count - 1));
		}

		public ActionResult Search()
		{
			return View();
		}
	}
}