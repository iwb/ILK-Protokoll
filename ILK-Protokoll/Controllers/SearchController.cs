using System;
using System.Collections.Generic;
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
			IQueryable<Topic> query = db.Topics;

			var tokens = Tokenize(searchterm);
			RestrictToAllTags(ref query, tokens["hasTag"]);


			if (tokens["has"].Contains("Decision", StringComparer.InvariantCultureIgnoreCase))
				query = query.Where(t => t.Decision != null);

			var searchTerms = tokens[""].Select(x => new Regex(Regex.Escape(x), RegexOptions.IgnoreCase)).ToArray();
			var results = new List<SearchResult>();

			foreach (var topic in query)
			{
				SearchTopic(topic, searchTerms, results);
				SearchComments(topic, searchTerms, results);
				SearchAssignments(topic, searchTerms, results);
				SearchAttachments(topic, searchTerms, results);
				SearchDecisions(topic, searchTerms, results);
			}
			SearchLists(searchTerms, results);


			ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;
			ViewBag.SearchTerm = searchterm;
			ViewBag.SearchPattern = "";
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

		private static ILookup<string, string> Tokenize(string str)
		{
			return Regex.Matches(str, @"(?<=^|\s)((?<disc>\w+):)?((?<token>[^\s""]+)|""(?<token>[^""]+)"")")
				.Cast<Match>()
				.ToLookup(match => match.Groups["disc"].ToString(),
					match => match.Groups["token"].ToString(),
					StringComparer.InvariantCultureIgnoreCase);
		}

		private void RestrictToAllTags(ref IQueryable<Topic> query, IEnumerable<string> tags)
		{
			var names = tags.ToArray();
			var tagIDs = db.Tags.Where(t => names.Contains(t.Name)).Select(t => t.ID).ToArray();

			query = query.Where(topic => (from tt in topic.Tags
				where tagIDs.Contains(tt.TagID)
				select tt).Count() == tagIDs.Count());
		}

		private void RestrictToAnyTag(ref IQueryable<Topic> query, IEnumerable<string> tags)
		{
			var names = tags.ToArray();
			var tagIDs = db.Tags.Where(t => names.Contains(t.Name)).Select(t => t.ID).ToArray();

			query = query.Where(topic => (from tt in topic.Tags
				where tagIDs.Contains(tt.TagID)
				select tt).Any());
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
			//var sw = new Stopwatch();
			//sw.Start();

			//var tokens = Tokenize(input.Searchterm).ToList();
			//var searchpatterns = MakePatterns(tokens, "AND");
			//var regexes = searchpatterns.Select(pattern => new Regex(pattern, RegexOptions.IgnoreCase));

			//var sets = new List<HashSet<SearchResult>>();

			//foreach (var regex in regexes)
			//{
			//	var currentset = new HashSet<SearchResult>();
			//	sets.Add(currentset);

			//	if (input.SearchTopics)
			//		SearchTopics(regex, currentset);
			//	if (input.SearchComments)
			//		SearchComments(regex, currentset);
			//	if (input.SearchAssignments)
			//		SearchAssignments(regex, currentset);
			//	if (input.SearchAttachments)
			//		SearchAttachments(regex, currentset);
			//	if (input.SearchDecisions)
			//		SearchDecisions(regex, currentset);
			//	if (input.SearchLists)
			//		SearchLists(regex, currentset);
			//}

			//var results = sets.Aggregate((a, b) =>
			//{
			//	a.IntersectWith(b);
			//	return a;
			//}).ToList();


			//ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;
			//ViewBag.SearchTerm = input.Searchterm;
			//ViewBag.SearchPattern = @"\b(" + tokens.Where(x => x != "AND").Aggregate((a, b) => a + "|" + b) + ")";
			//results.Sort((a, b) => b.Score.CompareTo(a.Score)); // Absteigend sortieren

			return View("Results", null);
		}

		private void SearchTopic(Topic topic, IEnumerable<Regex> searchterms, ICollection<SearchResult> resultlist)
		{
			var score = topic.IsReadOnly ? -5 : 0.0f;
			var hitlist = new List<Hit>();

			foreach (var pattern in searchterms)
			{
				var oldScore = score;

				var m = pattern.Matches(topic.Proposal);
				if (m.Count > 0)
				{
					score += ScoreMult(20, m.Count);
					hitlist.Add(new Hit
					{
						Property = "Titel",
						Text = topic.Title
					});
				}

				m = pattern.Matches(topic.Proposal);
				if (m.Count > 0)
				{
					score += ScoreMult(8, m.Count);
					hitlist.Add(new Hit
					{
						Property = "Beschlussvorschlag",
						Text = topic.Proposal
					});
				}

				m = pattern.Matches(topic.Description);
				if (m.Count > 0)
				{
					score += ScoreMult(6, m.Count);
					hitlist.Add(new Hit
					{
						Property = "Beschreibung",
						Text = topic.Description
					});
				}
				if (score <= oldScore)
					return;
			}
			if (score > 0)
			{
				// Späte Instanziierung, um Zeit zu sparen
				resultlist.Add(new SearchResult
				{
					ID = topic.ID,
					Score = score,
					EntityType = "Diskussion",
					Title = topic.Title,
					ActionURL = Url.Action("Details", "Topics", new {id = topic.ID}),
					Timestamp = topic.Created,
					Hits = hitlist,
					Tags = topic.Tags.Select(tt => tt.Tag).ToArray()
				});
			}
		}

		private void SearchComments(Topic topic, Regex[] searchterms, ICollection<SearchResult> resultlist)
		{
			foreach (var comment in topic.Comments)
			{
				var score = 0.0f;
				foreach (var pattern in searchterms)
				{
					var m = pattern.Matches(comment.Content);
					if (m.Count > 0)
						score += ScoreMult(2, m.Count);
					else
						score = float.NaN;
				}
				if (!float.IsNaN(score))
				{
					resultlist.Add(new SearchResult(comment.Content)
					{
						ID = comment.ID,
						Score = score,
						EntityType = "Kommentar",
						Title = topic.Title,
						ActionURL = Url.Action("Details", "Topics", new {id = topic.ID}),
						Timestamp = comment.Created,
						Tags = topic.Tags.Select(tt => tt.Tag).ToArray()
					});
				}
			}
		}

		private void SearchAssignments(Topic topic, IEnumerable<Regex> searchterms, ICollection<SearchResult> resultlist)
		{
			foreach (var assignment in topic.Assignments)
			{
				var score = 0.0f;
				foreach (var pattern in searchterms)
				{
					var m = pattern.Matches(assignment.Title);
					if (m.Count > 0)
					{
						score += ScoreMult(9, m.Count);
						continue;
					}

					m = pattern.Matches(assignment.Description);
					if (m.Count > 0)
					{
						score += ScoreMult(7, m.Count);
						continue;
					}
					score = float.NaN;
				}
				if (!float.IsNaN(score))
				{
					resultlist.Add(new SearchResult("Aufgabe", assignment.Description)
					{
						ID = assignment.ID,
						Score = score,
						EntityType = "Aufgabe",
						Title = assignment.Description,
						ActionURL = Url.Action("Details", "Assignments", new {id = assignment.ID}),
						Timestamp = assignment.DueDate,
						Tags = topic.Tags.Select(tt => tt.Tag).ToArray()
					});
				}
			}
		}

		private void SearchAttachments(Topic topic, IEnumerable<Regex> searchterms, ICollection<SearchResult> resultlist)
		{
			foreach (var attachment in topic.Documents)
			{
				var score = 0.0f;
				foreach (var pattern in searchterms)
				{
					var m = pattern.Matches(attachment.DisplayName);
					if (m.Count > 0)
						score += ScoreMult(9, m.Count);
					else
						score = float.NaN;
				}
				if (!float.IsNaN(score))
				{
					resultlist.Add(new SearchResult("Dateiname", attachment.DisplayName)
					{
						ID = attachment.ID,
						Score = score,
						EntityType = "Datei",
						Title = attachment.DisplayName,
						ActionURL = Url.Action("Details", "Attachments", new {id = attachment.ID}),
						Timestamp = attachment.Created,
						Tags = topic.Tags.Select(tt => tt.Tag).ToArray()
					});
				}
			}
		}

		private void SearchDecisions(Topic topic, IEnumerable<Regex> searchterms, ICollection<SearchResult> resultlist)
		{
			var decision = topic.Decision;
			if (decision == null)
				return;

			var score = decision.Type == DecisionType.Resolution ? 0.0f : -5;
			var hitlist = new List<Hit>();

			foreach (var pattern in searchterms)
			{
				var oldScore = score;

				var m = pattern.Matches(decision.OriginTopic.Title);
				if (m.Count > 0)
				{
					score += ScoreMult(21, m.Count);
					hitlist.Add(new Hit
					{
						Property = "Titel",
						Text = decision.OriginTopic.Title
					});
				}

				m = pattern.Matches(decision.Text);
				if (m.Count > 0)
				{
					score += 16;
					hitlist.Add(new Hit
					{
						Property = "Beschlusstext",
						Text = decision.Text
					});
				}

				if (score <= oldScore)
					return;
			}

			if (score > 0)
				resultlist.Add(new SearchResult
			{
				ID = decision.ID,
				Score = score,
				EntityType = decision.Type.DisplayName(),
				Title = decision.OriginTopic.Title,
				ActionURL = Url.Action("Details", "Topics", new {id = decision.OriginTopic.ID}),
				Timestamp = decision.Report.End,
				Tags = topic.Tags.Select(tt => tt.Tag).ToArray()
			});
		}

		private void SearchLists(Regex[] searchterms, ICollection<SearchResult> resultlist)
		{
			//foreach (var item in db.LEvents)
			//{
			//	var m = pattern.Matches(item.Description);
			//	if (m.Count > 0)
			//	{
			//		resultlist.Add(new SearchResult(item.Description)
			//		{
			//			ID = item.ID,
			//			Score = 7,
			//			EntityType = "Listeneintrag",
			//			Title = "Termin",
			//			ActionURL = Url.Content("~/ViewLists#event_table"),
			//			Timestamp = item.Created
			//		});
			//		continue;
			//	}
			//	m = pattern.Matches(item.Place);
			//	if (m.Count > 0)
			//	{
			//		resultlist.Add(new SearchResult(item.Place)
			//		{
			//			ID = item.ID,
			//			Score = 5,
			//			EntityType = "Listeneintrag",
			//			Title = "Termin",
			//			ActionURL = Url.Content("~/ViewLists#event_table"),
			//			Timestamp = item.Created
			//		});
			//	}
			//}

			//foreach (var item in db.LConferences)
			//{
			//	var m = pattern.Matches(item.Description);
			//	if (m.Count > 0)
			//	{
			//		resultlist.Add(new SearchResult(item.Description)
			//		{
			//			ID = item.ID,
			//			Score = 7,
			//			EntityType = "Listeneintrag",
			//			Title = "Auslandskonferenz",
			//			ActionURL = Url.Content("~/ViewLists#conference_table"),
			//			Timestamp = item.Created
			//		});
			//	}
			//}

			//foreach (var item in db.LIlkDays)
			//{
			//	var m = pattern.Matches(item.Topics);
			//	if (m.Count > 0)
			//	{
			//		resultlist.Add(new SearchResult(item.Topics)
			//		{
			//			ID = item.ID,
			//			Score = 7,
			//			EntityType = "Listeneintrag",
			//			Title = "ILK-Tag",
			//			ActionURL = Url.Content("~/ViewLists#ilkDay_table"),
			//			Timestamp = item.Created
			//		});
			//	}
			//}

			//foreach (var item in db.LIlkMeetings)
			//{
			//	var m = pattern.Matches(item.Comments);
			//	if (m.Count > 0)
			//	{
			//		resultlist.Add(new SearchResult(item.Comments)
			//		{
			//			ID = item.ID,
			//			Score = 7,
			//			EntityType = "Listeneintrag",
			//			Title = "ILK-Regeltermin",
			//			ActionURL = Url.Content("~/ViewLists#ilkMeeting_table"),
			//			Timestamp = item.Created
			//		});
			//	}
			//}

			//foreach (var item in db.LOpenings)
			//{
			//	var m = pattern.Matches(item.Description);
			//	if (m.Count > 0)
			//	{
			//		resultlist.Add(new SearchResult(item.Description)
			//		{
			//			ID = item.ID,
			//			Score = 6.5f,
			//			EntityType = "Listeneintrag",
			//			Title = "Vakante Stelle",
			//			ActionURL = Url.Content("~/ViewLists#opening_table"),
			//			Timestamp = item.Created
			//		});
			//	}
			//}
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