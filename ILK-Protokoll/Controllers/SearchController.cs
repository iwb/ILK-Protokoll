using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Session.Models.Lists;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;
using ILK_Protokoll.ViewModels;
using StackExchange.Profiling;

namespace ILK_Protokoll.Controllers
{
	/// <summary>
	///    Die Suche ermöglicht es, auch alte Diskussionen und Beschlüsse wieder zu finden. Suchbegriffe werden standardmäßig
	///    mit UND verknüpft. Bei jeder Suche wird die Datenbank einmal vollständig durchsucht. Bei zunehmenden
	///    Performanceproblemen könnten hier zwei Maßahmen eingesetzt werden: 1. Eine stored Proc, die es erspart alle Daten
	///    aus der DB zum Server zu übertragen. Da momentan aber DB und Server auf derselben Maschine sitzen, nicht
	///    vielversprechend. 2. Ein Suchindex. Eine Tabelle in der Datenbank, in der alle Wörter mit den zugehörigen
	///    Suchtreffern vermekrt sind. Eventuell auch nur als Prefixliste oder komplett als Radix-tree in der Datenbank. Die
	///    aktuelle Performance ist aber ausreichend.
	/// </summary>
	public class SearchController : BaseController
	{
		// GET: /Search
		// Erweiterte Suche 
		public ActionResult Index(ExtendedSearchVM input, [Bind(Prefix = "Tags")] Dictionary<int, bool> selectedTags)
		{
			if (selectedTags == null)
				selectedTags = db.Tags.ToDictionary(x => x.ID, x => false);

			if (string.IsNullOrWhiteSpace(input.Searchterm))
			{
				input.Tags = db.Tags.ToDictionary(x => x, x => selectedTags[x.ID]);
				return View("SearchMask", input);
			}
			else
				return ExtendendResults(input, selectedTags);
		}

		private ActionResult ExtendendResults(ExtendedSearchVM input, Dictionary<int, bool> selectedTags)
		{
			var query = new StringBuilder(input.Searchterm);

			if (input.SearchTopics)
				query.Append(" is:Topic");
			if (input.SearchComments)
				query.Append(" is:Commment");
			if (input.SearchAssignments)
				query.Append(" is:Task");
			if (input.SearchAttachments)
				query.Append(" is:File");
			if (input.SearchDecisions)
				query.Append(" is:Decision");
			if (input.SearchLists)
				query.Append(" is:Item");

			var ids = selectedTags.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
			foreach (var tagname in db.Tags.Where(x => ids.Contains(x.ID)).Select(tag => tag.Name))
			{
				if (Regex.IsMatch(tagname, @"\s", RegexOptions.IgnoreCase))
					query.Append(" hasTag:\"" + tagname + "\"");
				else
					query.Append(" hasTag:" + tagname);
			}
			return Results(query.ToString());
		}

		// GET: Results
		public ActionResult Results(string searchterm)
		{
			if (string.IsNullOrWhiteSpace(searchterm))
				return RedirectToAction("Index");

			var profiler = MiniProfiler.Current;
			var sw = new Stopwatch();
			sw.Start();

			IQueryable<Topic> query = db.Topics
				.Include(t => t.Assignments)
				.Include(t => t.Comments)
				.Include(t => t.Decision)
				.Include(t => t.Documents)
				.Include(t => t.Tags);

			ILookup<string, string> tokens = Tokenize(searchterm);
			RestrictToAllTags(ref query, tokens["hasTag"]);

			if (tokens["has"].Contains("Decision", StringComparer.InvariantCultureIgnoreCase))
				query = query.Where(t => t.Decision != null);

			Regex[] searchTerms = MakePatterns(tokens[""]).Select(x => new Regex(x, RegexOptions.IgnoreCase)).ToArray();
			var results = new SearchResultList();

			var selector = ParseDiscriminators(tokens["is"]);

			using (profiler.Step("Suche in Themen"))
			{
				foreach (Topic topic in query)
				{
					if (selector["decision"])
						SearchDecision(topic, searchTerms, results);
					if (selector["topic"])
						SearchTopic(topic, searchTerms, results);
					if (selector["comment"])
						SearchComments(topic, searchTerms, results);
					if (selector["task"])
						SearchAssignments(topic, searchTerms, results);
					if (selector["file"])
						SearchAttachments(topic, searchTerms, results);
				}
			}
			if (selector["item"])
			{
				using (profiler.Step("Suche in Listen"))
					SearchLists(searchTerms, results);
			}

			ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;
			ViewBag.SearchTerm = searchterm;
			ViewBag.SearchPatterns = searchTerms;
			results.Sort(); // Absteigend sortieren nach Score
			return View("Results", results);
		}

		private static ILookup<string, string> Tokenize(string str)
		{
			return Regex.Matches(str, @"(?<=^|\s)((?<disc>\w+):)?((?<token>[^\s""]+)|""(?<token>[^""]+)"")")
				.Cast<Match>()
				.ToLookup(match => match.Groups["disc"].ToString(),
					match => match.Groups["token"].ToString(),
					StringComparer.InvariantCultureIgnoreCase);
		}

		private static Dictionary<string, bool> ParseDiscriminators(IEnumerable<string> tokens)
		{
			var any = tokens.Any();
			return new[] {"topic", "task", "decision", "comment", "file", "item"}.ToDictionary(s => s,
				s => !any || tokens.Contains(s, StringComparer.OrdinalIgnoreCase));
		}

		private void RestrictToAllTags(ref IQueryable<Topic> query, IEnumerable<string> tags)
		{
			string[] names = tags.ToArray();
			int[] tagIDs = db.Tags.Where(t => names.Contains(t.Name)).Select(t => t.ID).ToArray();

			query = query.Where(topic => (from tt in topic.Tags
				where tagIDs.Contains(tt.TagID)
				select tt).Count() == tagIDs.Count());
		}

		private void RestrictToAnyTag(ref IQueryable<Topic> query, IEnumerable<string> tags)
		{
			string[] names = tags.ToArray();
			int[] tagIDs = db.Tags.Where(t => names.Contains(t.Name)).Select(t => t.ID).ToArray();

			query = query.Where(topic => (from tt in topic.Tags
				where tagIDs.Contains(tt.TagID)
				select tt).Any());
		}

		/// <summary>
		///    Generiert aus einer Menge von Tokens eine Menge von Patterns. Die Tokens werden nach "or" durchsucht,
		///    und die benachbarten Tokens werden schließlich in einem kombinierten Regex zurückgegeben. Die Tokens
		///    werden für den Regex passend escapet.
		/// </summary>
		/// <param name="items">Die Token, aus denen die Patterns erzeugt werden sollen.</param>
		/// <returns>Patterns, die sich aus den Tokens ergeben.</returns>
		private static IEnumerable<string> MakePatterns(IEnumerable<string> items)
		{
			const string delimiter = "or";
			Func<IEnumerable<string>, string> escapeAndJoin = x => x.Select(Regex.Escape).Aggregate((a, b) => a + "|" + b);

			var orItems = new List<string>();
			string lastToken = null;
			foreach (string item in items)
			{
				if (string.Equals(item, delimiter, StringComparison.OrdinalIgnoreCase))
				{
					if (lastToken != null)
						orItems.Add(lastToken);
					lastToken = null;
				}
				else
				{
					if (lastToken != null)
					{
						if (orItems.Count > 0)
						{
							orItems.Add(lastToken);
							yield return @"\b(" + escapeAndJoin(orItems) + ")";
							orItems.Clear();
						}
						else
							yield return @"\b" + lastToken;
					}
					lastToken = item;
				}
			}

			if (orItems.Count > 0)
			{
				if (lastToken != null)
					orItems.Add(lastToken);

				yield return @"\b(" + escapeAndJoin(orItems) + ")";
			}
			else if (lastToken != null)
				yield return @"\b" + lastToken;
		}

		private void SearchDecision(Topic topic, Regex[] searchterms, SearchResultList resultlist)
		{
			Decision decision = topic.Decision;
			if (decision == null)
				return;

			float score = decision.Type == DecisionType.Resolution ? 0.0f : -5;
			var hitlist = new HashSet<Hit>(new HitPropertyComparer());

			if (searchterms.Length == 0)
			{
				score = ScoreAge(25, decision.Report.End);
				hitlist.Add(new Hit("Beschlusstext", decision.Text));
			}
			else
			{
				foreach (Regex pattern in searchterms)
				{
					float oldScore = score;

					MatchCollection m = pattern.Matches(decision.OriginTopic.Title);
					if (m.Count > 0)
					{
						score += ScoreMult(21, m.Count);
						hitlist.Add(new Hit("Titel", decision.OriginTopic.Title));
					}

					m = pattern.Matches(decision.Text);
					if (m.Count > 0)
					{
						score += 16;
						hitlist.Add(new Hit("Beschlusstext", decision.Text));
					}

					if (score <= oldScore)
						return;
				}
			}

			if (score > 0)
			{
				resultlist.Add(topic.ID, new SearchResult
				{
					ID = decision.ID,
					Score = score,
					EntityType = decision.Type.DisplayName(),
					Title = decision.OriginTopic.Title,
					ActionURL = Url.Action("Details", "Topics", new {id = decision.OriginTopic.ID}),
					Timestamp = decision.Report.End,
					Hits = hitlist.ToList(),
					Tags = topic.Tags.Select(tt => tt.Tag).ToArray()
				});
			}
		}

		private void SearchTopic(Topic topic, Regex[] searchterms, SearchResultList resultlist)
		{
			float score = topic.IsReadOnly ? -5 : 0.0f;
			var hitlist = new HashSet<Hit>(new HitPropertyComparer());

			if (searchterms.Length == 0)
			{
				score = ScoreAge(20, topic.ValidFrom);
				hitlist.Add(new Hit("Titel", topic.Title));
				hitlist.Add(new Hit("Beschreibung", topic.Description));
			}
			else
			{
				foreach (Regex pattern in searchterms)
				{
					float oldScore = score;

					MatchCollection m;
					if (!resultlist.Contains(topic.ID)) // Duplikat Beschlussvorschlag/Beschlusstext vermeiden
					{
						m = pattern.Matches(topic.Title);
						if (m.Count > 0)
						{
							score += ScoreMult(20, m.Count);
							hitlist.Add(new Hit("Titel", topic.Title));
						}

						m = pattern.Matches(topic.Proposal);
						if (m.Count > 0)
						{
							score += ScoreMult(8, m.Count);
							hitlist.Add(new Hit("Beschlusstext", topic.Proposal));
						}
					}

					m = pattern.Matches(topic.Description);
					if (m.Count > 0)
					{
						score += ScoreMult(6, m.Count);
						hitlist.Add(new Hit("Beschreibung", topic.Description));
					}
					if (score <= oldScore)
						return;
				}
			}
			if (score <= 0)
				return;

			if (resultlist.Contains(topic.ID))
				resultlist.Amend(topic.ID, score, hitlist);
			else
			{
				resultlist.Add(topic.ID, new SearchResult
				{
					ID = topic.ID,
					Score = score,
					EntityType = topic.HasDecision() ? topic.Decision.Type.DisplayName() : "Diskussion",
					Title = topic.Title,
					ActionURL = Url.Action("Details", "Topics", new {id = topic.ID}),
					Timestamp = topic.ValidFrom,
					Hits = hitlist.ToList(),
					Tags = topic.Tags.Select(tt => tt.Tag).ToArray()
				});
			}
		}

		/// <summary>
		/// Durchsucht die Kommentare einer Diskussion
		/// </summary>
		/// <param name="topic">Diskussion</param>
		/// <param name="searchterms">Suchbegriffe</param>
		/// <param name="resultlist">Ergebnisliste</param>
		private void SearchComments(Topic topic, Regex[] searchterms, SearchResultList resultlist)
		{
			// Hier wird das NaN-Schema erstmal eingesetzt. Wird ein Suchbegriff nicht gefunden, dann wird der score auf NaN gesetzt. Nur wenn alle Suchbegriffe in einem Kommentar vorkamen, existiert ein gültiger score, der Kommentar wird dann zru ergebnisliste hinzugefügt.
			foreach (Comment comment in topic.Comments)
			{
				float score = 0.0f;
				foreach (Regex pattern in searchterms)
				{
					MatchCollection m = pattern.Matches(comment.Content);
					if (m.Count > 0)
						score += ScoreMult(2, m.Count);
					else
						score = float.NaN;
				}
				if (float.IsNaN(score))
					continue;

				if (resultlist.Contains(topic.ID))
					resultlist.Amend(topic.ID, score, new Hit("Kommentar", comment.Content));
				else
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

		private void SearchAssignments(Topic topic, Regex[] searchterms, SearchResultList resultlist)
		{
			var hitlist = new HashSet<Hit>(new HitPropertyComparer());
			foreach (Assignment assignment in topic.Assignments)
			{
				float score = 0.0f;
				foreach (Regex pattern in searchterms)
				{
					MatchCollection m = pattern.Matches(assignment.Title);
					if (m.Count > 0)
					{
						score += ScoreMult(9, m.Count);
						hitlist.Add(new Hit("Aufgabentitel", assignment.Title));
						continue;
					}

					m = pattern.Matches(assignment.Description);
					if (m.Count > 0)
					{
						score += ScoreMult(7, m.Count);
						hitlist.Add(new Hit("Aufgabentext", assignment.Title));
						continue;
					}
					score = float.NaN;
				}
				if (float.IsNaN(score))
					continue;

				if (resultlist.Contains(topic.ID))
					resultlist.Amend(topic.ID, score, hitlist);
				else
				{
					resultlist.Add(new SearchResult("Aufgabe", assignment.Description)
					{
						ID = assignment.ID,
						Score = score,
						EntityType = "Aufgabe",
						Title = assignment.Description,
						ActionURL = Url.Action("Details", "Assignments", new {id = assignment.ID}),
						Timestamp = assignment.DueDate,
						Hits = hitlist.ToList(),
						Tags = topic.Tags.Select(tt => tt.Tag).ToArray()
					});
				}
				hitlist.Clear();
			}
		}

		private void SearchAttachments(Topic topic, Regex[] searchterms, SearchResultList resultlist)
		{
			foreach (Document attachment in topic.Documents)
			{
				float score = 0.0f;
				foreach (Regex pattern in searchterms)
				{
					MatchCollection m = pattern.Matches(attachment.DisplayName);
					if (m.Count > 0)
						score += ScoreMult(9, m.Count);
					else
						score = float.NaN;
				}
				if (float.IsNaN(score))
					continue;

				if (resultlist.Contains(topic.ID))
					resultlist.Amend(topic.ID, score, new Hit("Dateiname", attachment.DisplayName));
				else
				{
					resultlist.Add(new SearchResult("Dateiname", attachment.DisplayName)
					{
						ID = attachment.ID,
						Score = score,
						EntityType = "Dokument",
						Title = attachment.DisplayName,
						ActionURL = Url.Action("Details", "Attachments", new {id = attachment.ID}),
						Timestamp = attachment.Created,
						Tags = topic.Tags.Select(tt => tt.Tag).ToArray()
					});
				}
			}
		}

		/// <summary>
		///    Durchsucht alle vorhandenen Listen. Jede Liste wird separat durchsucht, und die Treffer ggf. zur Trefferliste
		///    hinzugefügt. Falsl kein Suchbegriff eingegbeen wurde, werden alle Listeneinträge gefunden.
		/// </summary>
		/// <param name="searchterms">Die Suchbegriffe, nach denen gesucht wird.</param>
		/// <param name="resultlist">Die Ergebnisliste, zu der die Ergebnisse hinzugefügt werden.</param>
		private void SearchLists(Regex[] searchterms, SearchResultList resultlist)
		{
			foreach (var item in db.LEvents)
			{
				float score = 0.0f;
				foreach (Regex pattern in searchterms)
				{
					var m = pattern.Matches(item.Description);
					if (m.Count > 0)
					{
						score += 7;
						continue;
					}
					m = pattern.Matches(item.Place);
					if (m.Count > 0)
					{
						score += 5;
						continue;
					}
					score = float.NaN;
				}
				if (!float.IsNaN(score))
				{
					resultlist.Add(new SearchResult
					{
						ID = item.ID,
						Score = score,
						EntityType = "Listeneintrag",
						Title = "Termin",
						ActionURL = Url.Content("~/ViewLists#event_table"),
						Timestamp = item.StartDate,
						Hits = new List<Hit>
						{
							new Hit("Datum", item.StartDate.ToShortDateString()),
							Hit.FromProperty(item, x => x.Place),
							Hit.FromProperty(item, x => x.Description)
						}
					});
				}
			}

			// Da ohnehin nur ein feld durchsucht wird, kann die Schleife über die Suchbegriffe mit der .All() Methode verkürzt weren. Negativ: Mehrfache Vorkommen des Suchbegriffs resultieren nicht in einem höheren score.
			foreach (var item in db.LConferences)
			{
				if (searchterms.All(pattern => pattern.IsMatch(item.Description)))
				{
					resultlist.Add(new SearchResult
					{
						ID = item.ID,
						Score = 7,
						EntityType = "Listeneintrag",
						Title = "Auslandskonferenz",
						ActionURL = Url.Content("~/ViewLists#conference_table"),
						Timestamp = item.LastChanged,
						Hits = new List<Hit>
						{
							new Hit("Datum", item.StartDate.ToShortDateString()),
							Hit.FromProperty(item, x => x.Description),
							Hit.FromProperty(item, x => x.Employee)
						}
					});
				}
			}

			foreach (var item in db.LExtensions)
			{
				if (searchterms.All(pattern => pattern.IsMatch(item.Comment)))
				{
					resultlist.Add(new SearchResult
					{
						ID = item.ID,
						Score = 7,
						EntityType = "Listeneintrag",
						Title = "Vertragsverlängerung",
						ActionURL = Url.Content("~/ViewLists#conference_table"),
						Timestamp = item.LastChanged,
						Hits = new List<Hit>
						{
							Hit.FromProperty(item, x => x.Employee),
							new Hit("Vertragsende", item.EndDate.ToShortDateString()),
							Hit.FromProperty(item, x => x.Comment)
						}
					});
				}
			}

			//-----------------------------------------------------------------------------------------------------
			// Da es quasi unmöglich ist, dass alle Suchbegriffe auf ein Mitarbeiterkürzel zutreffen, werden die Kürzel mit .Any() durchsucht. Eine Suche nach "ab ba" findet also beide Mitarbeiter (statt keinem).
			foreach (var item in db.LEmployeePresentations)
			{
				if (searchterms.Any(pattern => pattern.IsMatch(item.Employee)))
				{
					resultlist.Add(new SearchResult("Mitarbeiter")
					{
						ID = item.ID,
						Score = 7,
						EntityType = "Listeneintrag",
						Title = "Mitarbeiterpräsentation",
						ActionURL = Url.Content("~/ViewLists#conference_table"),
						Timestamp = item.LastChanged,
						Hits = new List<Hit>
						{
							Hit.FromProperty(item, x => x.Employee),
							Hit.FromProperty<EmployeePresentation>(x => x.Ilk, item.Ilk.ShortName)
						}
					});
				}
			}
			// Die Dokumente, die Diskussionen zugeordnet sind, wurden oben bereits durchsucht.
			foreach (var doc in db.Documents.Where(doc => doc.EmployeePresentationID != null))
			{
				if (searchterms.All(pattern => pattern.IsMatch(doc.DisplayName)))
				{
					resultlist.Add(new SearchResult("Dateiname", doc.DisplayName)
					{
						ID = doc.ID,
						Score = 7,
						EntityType = "Dokument",
						Title = doc.DisplayName,
						ActionURL = Url.Action("Details", "Attachments", new {id = doc.ID}),
						Timestamp = doc.Created
					});
				}
			}
			//-----------------------------------------------------------------------------------------------------

			foreach (var item in db.LIlkDays)
			{
				if (searchterms.All(pattern => pattern.IsMatch(item.Topics)))
				{
					resultlist.Add(new SearchResult
					{
						ID = item.ID,
						Score = 7,
						EntityType = "Listeneintrag",
						Title = "ILK-Tag",
						ActionURL = Url.Content("~/ViewLists#ilkDay_table"),
						Timestamp = item.LastChanged,
						Hits = new List<Hit>
						{
							Hit.FromProperty<IlkDay>(x => x.Start, item.Start.ToShortDateString()),
							Hit.FromProperty(item, x => x.Place),
							Hit.FromProperty(item, x => x.Topics)
						}
					});
				}
			}

			foreach (var item in db.LIlkMeetings)
			{
				if (searchterms.All(pattern => pattern.IsMatch(item.Comments)))
				{
					resultlist.Add(new SearchResult
					{
						ID = item.ID,
						Score = 7,
						EntityType = "Listeneintrag",
						Title = "ILK-Regeltermin",
						ActionURL = Url.Content("~/ViewLists#ilkMeeting_table"),
						Timestamp = item.LastChanged,
						Hits = new List<Hit>
						{
							Hit.FromProperty<IlkDay>(x => x.Start, item.Start.ToShortDateString()),
							Hit.FromProperty(item, x => x.Place),
							Hit.FromProperty(item, x => x.Comments)
						}
					});
				}
			}

			foreach (var item in db.LOpenings)
			{
				if (searchterms.All(pattern => pattern.IsMatch(item.Description) || pattern.IsMatch(item.Project)))
				{
					resultlist.Add(new SearchResult
					{
						ID = item.ID,
						Score = 6.5f,
						EntityType = "Listeneintrag",
						Title = "Vakante Stelle",
						ActionURL = Url.Content("~/ViewLists#opening_table"),
						Timestamp = item.LastChanged,
						Hits = new List<Hit>
						{
							Hit.FromProperty(item, x => x.Project),
							Hit.FromProperty<IlkDay>(x => x.Start, item.Start.ToShortDateString()),
							Hit.FromProperty(item, x => x.Description)
						}
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

		private static float ScoreAge(int baseScore, DateTime lastChange)
		{
			var age = (DateTime.Now - lastChange).TotalDays;
			return baseScore * (float)Math.Exp(-age / 1000) + 7;
		}
	}

	public class HitPropertyComparer : IEqualityComparer<Hit>
	{
		public bool Equals(Hit x, Hit y)
		{
			return string.Equals(x.Property, y.Property);
		}

		public int GetHashCode(Hit obj)
		{
			return obj.Property.GetHashCode();
		}
	}

	public class SREntityComparer : IEqualityComparer<SearchResult>
	{
		private readonly string _entity;

		public SREntityComparer(string entity)
		{
			_entity = entity;
		}

		public bool Equals(SearchResult x, SearchResult y)
		{
			return x.EntityType == _entity && y.EntityType == _entity && x.ID == y.ID;
		}

		public int GetHashCode(SearchResult obj)
		{
			return obj.ID;
		}
	}
}