using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Controllers
{
	public class SearchController : BaseController
	{
		// GET: Results
		// Einfache Suche über die Navbar
		public ActionResult Results(string searchterm)
		{
			if (string.IsNullOrWhiteSpace(searchterm))
				return RedirectToAction("Index");

			var sw = new Stopwatch();
			sw.Start();

			var words = Regex.Split(searchterm, @"\s+").Select(Regex.Escape);

			var searchpattern = @"\b(" + words.Aggregate((a, b) => a + "|" + b) + ")";
			var regex = new Regex(searchpattern, RegexOptions.IgnoreCase);

			var results = new List<SearchResult>();

			SearchTopics(regex, results);
			SearchComments(regex, results);
			SearchAssignments(regex, results);
			SearchAttachments(regex, results);
			SearchDecisions(regex, results);
			SearchLists(regex, results);

			ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;
			ViewBag.SearchTerm = searchterm;
			ViewBag.SearchPattern = searchpattern;
			results.Sort((a, b) => b.Score.CompareTo(a.Score)); // Absteigend sortieren
			return View(results);
		}

		// GET: /Search
		// Erweiterte Suche 
		public ActionResult Index(ExtendedSearchVM input)
		{
			if (string.IsNullOrWhiteSpace(input.Searchterm))
				return View("SearchMask", input);
			else
				return ExtendendResults(input);
		}

		private static IEnumerable<string> Tokenize(string str)
		{
			return Regex.Matches(str, @"(?<match>\w+)|\""(?<match>[\w\s]*)""")
				.Cast<Match>()
				.Select(m => m.Groups["match"].Value);
		}

		private static IEnumerable<string> MakePatterns(IEnumerable<string> items, string delimiter)
		{
			Func<IEnumerable<string>, string> escapeAndJoin = x => x.Select(Regex.Escape).Aggregate((a, b) => a + "|" + b);

			var currentItems = new List<string>();
			foreach (var item in items)
			{
				if (item == delimiter && currentItems.Count > 0)
				{
					yield return @"\b(" + escapeAndJoin(currentItems) + ")";
					currentItems.Clear();
				}
				else
					currentItems.Add(item);
			}
			if (currentItems.Count > 0)
				yield return @"\b(" + escapeAndJoin(currentItems) + ")";
		}

		private ActionResult ExtendendResults(ExtendedSearchVM input)
		{
			var sw = new Stopwatch();
			sw.Start();

			var tokens = Tokenize(input.Searchterm).ToList();
			var searchpatterns = MakePatterns(tokens, "AND");
			var regexes = searchpatterns.Select(pattern => new Regex(pattern, RegexOptions.IgnoreCase));

			var sets = new List<HashSet<SearchResult>>();

			foreach (var regex in regexes)
			{
				var currentset = new HashSet<SearchResult>();
				sets.Add(currentset);

				if (input.SearchTopics)
					SearchTopics(regex, currentset);
				if (input.SearchComments)
					SearchComments(regex, currentset);
				if (input.SearchAssignments)
					SearchAssignments(regex, currentset);
				if (input.SearchAttachments)
					SearchAttachments(regex, currentset);
				if (input.SearchDecisions)
					SearchDecisions(regex, currentset);
				if (input.SearchLists)
					SearchLists(regex, currentset);
			}

			var results = sets.Aggregate((a, b) =>
			{
				a.IntersectWith(b);
				return a;
			}).ToList();


			ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;
			ViewBag.SearchTerm = input.Searchterm;
			ViewBag.SearchPattern = @"\b(" + tokens.Where(x => x != "AND").Aggregate((a, b) => a + "|" + b) + ")";
			results.Sort((a, b) => b.Score.CompareTo(a.Score)); // Absteigend sortieren

			return View("Results", results);
		}

		private void SearchTopics(Regex pattern, ICollection<SearchResult> resultlist)
		{
			foreach (var topic in db.Topics)
			{
				var sr = new SearchResult
				{
					ID = topic.ID,
					Score = topic.IsReadOnly ? -5 : 0,
					EntityType = "Diskussion",
					Title = topic.Title,
					ActionURL = Url.Action("Details", "Topics", new {id = topic.ID}),
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
				select new {commentID = c.ID, topicID = t.ID, Text = c.Content, t.Title, c.Created};

			foreach (var comment in query)
			{
				var m = pattern.Matches(comment.Text);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult(comment.Text)
					{
						ID = comment.commentID,
						Score = ScoreMult(2, m.Count),
						EntityType = "Kommentar",
						Title = comment.Title,
						ActionURL = Url.Action("Details", "Topics", new {id = comment.topicID}),
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
						ID = assignment.ID,
						Score = ScoreMult(9, m.Count),
						EntityType = "Aufgabe",
						Title = assignment.Title,
						ActionURL = Url.Action("Details", "Assignments", new {id = assignment.ID}),
						Timestamp = assignment.DueDate
					});
					continue;
				}

				m = pattern.Matches(assignment.Description);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult("Aufgabentext", assignment.Description)
					{
						ID = assignment.ID,
						Score = ScoreMult(7, m.Count),
						EntityType = "Aufgabe",
						Title = assignment.Description,
						ActionURL = Url.Action("Details", "Assignments", new {id = assignment.ID}),
						Timestamp = assignment.DueDate
					});
				}
			}
		}

		private void SearchAttachments(Regex pattern, ICollection<SearchResult> resultlist)
		{
			foreach (var attachment in db.Documents)
			{
				var m = pattern.Matches(attachment.DisplayName);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult("Dateiname", attachment.DisplayName)
					{
						ID = attachment.ID,
						Score = ScoreMult(9, m.Count),
						EntityType = "Datei",
						Title = attachment.DisplayName,
						ActionURL = Url.Action("Details", "Attachments", new {id = attachment.ID}),
						Timestamp = attachment.Created
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
					ID = decision.ID,
					Score = decision.Type == DecisionType.Resolution ? 0 : -11,
					EntityType = decision.Type.DisplayName(),
					Title = decision.OriginTopic.Title,
					ActionURL = Url.Action("Details", "Topics", new {id = decision.OriginTopic.ID}),
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
					sr.Score += 16;
					sr.Hits.Add(new Hit
					{
						Property = "Beschlusstext",
						Text = decision.Text
					});
				}

				if (sr.Score <= 0)
					continue;

				sr.Score += 11;
				resultlist.Add(sr);
			}
		}

		private void SearchLists(Regex pattern, ICollection<SearchResult> resultlist)
		{
			foreach (var item in db.LEvents)
			{
				var m = pattern.Matches(item.Description);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult(item.Description)
					{
						ID = item.ID,
						Score = 7,
						EntityType = "Listeneintrag",
						Title = "Termin",
						ActionURL = Url.Content("~/ViewLists#event_table"),
						Timestamp = item.Created
					});
					continue;
				}
				m = pattern.Matches(item.Place);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult(item.Place)
					{
						ID = item.ID,
						Score = 5,
						EntityType = "Listeneintrag",
						Title = "Termin",
						ActionURL = Url.Content("~/ViewLists#event_table"),
						Timestamp = item.Created
					});
				}
			}

			foreach (var item in db.LConferences)
			{
				var m = pattern.Matches(item.Description);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult(item.Description)
					{
						ID = item.ID,
						Score = 7,
						EntityType = "Listeneintrag",
						Title = "Auslandskonferenz",
						ActionURL = Url.Content("~/ViewLists#conference_table"),
						Timestamp = item.Created
					});
				}
			}

			foreach (var item in db.LIlkDays)
			{
				var m = pattern.Matches(item.Topics);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult(item.Topics)
					{
						ID = item.ID,
						Score = 7,
						EntityType = "Listeneintrag",
						Title = "ILK-Tag",
						ActionURL = Url.Content("~/ViewLists#ilkDay_table"),
						Timestamp = item.Created
					});
				}
			}

			foreach (var item in db.LIlkMeetings)
			{
				var m = pattern.Matches(item.Comments);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult(item.Comments)
					{
						ID = item.ID,
						Score = 7,
						EntityType = "Listeneintrag",
						Title = "ILK-Regeltermin",
						ActionURL = Url.Content("~/ViewLists#ilkMeeting_table"),
						Timestamp = item.Created
					});
				}
			}

			foreach (var item in db.LOpenings)
			{
				var m = pattern.Matches(item.Description);
				if (m.Count > 0)
				{
					resultlist.Add(new SearchResult(item.Description)
					{
						ID = item.ID,
						Score = 6.5f,
						EntityType = "Listeneintrag",
						Title = "Vakante Stelle",
						ActionURL = Url.Content("~/ViewLists#opening_table"),
						Timestamp = item.Created
					});
				}
			}
		}

		private static float ScoreMult(int baseScore, int count)
		{
			if (baseScore == 0 || count == 0)
				return 0;
			else
				return baseScore * (float)(5 - 4 * Math.Pow(0.8, count - 1));
		}
	}
}