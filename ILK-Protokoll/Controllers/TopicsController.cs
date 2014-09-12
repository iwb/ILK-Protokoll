using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Controllers
{
	public class TopicsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.TopicStyle = "active";
		}

		// GET: Topics
		public ActionResult Index(FilteredTopics filter)
		{
			IQueryable<Topic> query = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.TargetSessionType)
				.Include(t => t.Creator);

			if (!filter.ShowReadonly)
				query = query.Where(t => !t.IsReadOnly);

			if (filter.ShowPriority >= 0)
				query = query.Where(t => t.Priority == (Priority)filter.ShowPriority);

			if (filter.SessionTypeID > 0)
				query = query.Where(t => t.SessionTypeID == filter.SessionTypeID || t.TargetSessionTypeID == filter.SessionTypeID);

			if (filter.Timespan != 0)
			{
				if (filter.Timespan > 0) // Nur die letzten x Tage anzeigen
				{
					var cutoff = DateTime.Today.AddDays(-filter.Timespan);
					query = query.Where(t => t.Created >= cutoff);
				}
				else // Alles VOR den letzten x Tagen anzeigen
				{
					var cutoff = DateTime.Today.AddDays(filter.Timespan);
					query = query.Where(t => t.Created < cutoff);
				}
			}

			if (filter.OwnerID != 0)
				query = query.Where(a => a.OwnerID == filter.OwnerID);

			filter.UserList = CreateUserSelectList();
			filter.PriorityList = PriorityChoices(filter.ShowPriority);
			filter.SessionTypeList = new SelectList(db.GetActiveSessionTypes(), "ID", "Name");

			filter.TimespanList = TimespanChoices(filter.Timespan);
			filter.Topics = query.OrderByDescending(t => t.Priority).ThenByDescending(t => t.Title).ToList();

			return View(filter);
		}

		private static IEnumerable<SelectListItem> PriorityChoices(int preselect)
		{
			var placeholder = new SelectListItem
			{
				Text = "(Alle Prioritäten)",
				Value = "-1",
				Selected = preselect < 0
			}.ToEnumerable();

			var items = ((Priority[])Enum.GetValues(typeof(Priority)))
				.Select(p => new SelectListItem
				{
					Text = p.DisplayName(),
					Value = ((int)p).ToString()
				});

			return placeholder.Concat(items);
		}

		private static IEnumerable<SelectListItem> TimespanChoices(int preselect)
		{
			return new[]
			{
				new SelectListItem
				{
					Text = "14 Tage",
					Value = "14",
					Selected = preselect == 14
				},
				new SelectListItem
				{
					Text = "30 Tage",
					Value = "30",
					Selected = preselect == 30
				},
				new SelectListItem
				{
					Text = "Älter als 30 Tage",
					Value = "-30",
					Selected = preselect == -30
				},
			};
		}

		// GET: Topics/Details/5
		public ActionResult Details(int? id, bool reporting = false)
		{
			if (id == null)
				return HTTPStatus(HttpStatusCode.BadRequest, "Für diesen Vorgang ist eine TopicID ist erforderlich.");

			var topic = db.Topics
				.Include(t => t.Assignments)
				.Include(t => t.Creator)
				.Include(t => t.Decision)
				.Include(t => t.Decision.Report)
				.Include(t => t.Lock)
				.Include(t => t.Lock.Session.Manager)
				.Single(t => t.ID == id.Value);

			if (topic == null)
				return HttpNotFound();

			ViewBag.TopicID = id.Value;
			ViewBag.TopicHistoryCount = db.TopicHistory.Count(t => t.TopicID == id.Value);
			ViewBag.IsEditable = topic.IsEditableBy(GetCurrentUser(), GetSession()).IsAuthorized;
			ViewBag.Reporting = reporting;
			topic.IsLocked = IsTopicLocked(id.Value);

			if (reporting)
				return PartialView(topic);
			else
				return View(topic);
		}

		// GET: Topics/Create
		public ActionResult Create()
		{
			var viewmodel = new TopicEdit
			{
				SessionTypeList = new SelectList(db.GetActiveSessionTypes(), "ID", "Name"),
				TargetSessionTypeList = new SelectList(db.GetActiveSessionTypes(), "ID", "Name"),
				UserList = CreateUserSelectList()
			};
			return View(viewmodel);
		}

		// POST: Topics/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Exclude = "TargetSessionTypeID")] TopicEdit input)
		{
			if (ModelState.IsValid)
			{
				var t = new Topic
				{
					CreatorID = GetCurrentUserID(),
					SessionTypeID = input.SessionTypeID
				};
				t.IncorporateUpdates(input);

				foreach (User user in db.SessionTypes
					.Include(st => st.Attendees)
					.Single(st => st.ID == input.SessionTypeID).Attendees)
				{
					t.Votes.Add(new Vote(user, VoteKind.None));
				}

				db.Topics.Add(t);

				// Falls in einer Sitzung eine neue Diskussion erzeugt wird, kann diese der Sitzung zugeschlagen werden.
				var session = GetSession();
				if (session != null && session.SessionTypeID == input.SessionTypeID)
				{
					session.LockedTopics.Add(new TopicLock()
					{
						Topic = t,
						Session = session
					});
				}
				
				try
				{
					db.SaveChanges();
				}
				catch (DbEntityValidationException e)
				{
					var message = ErrorMessageFromException(e);
					return HTTPStatus(500, message);
				}

				return RedirectToAction("Index");
			}

			input.SessionTypeList = new SelectList(db.GetActiveSessionTypes(), "ID", "Name", input.SessionTypeID);
			input.TargetSessionTypeList = new SelectList(db.GetActiveSessionTypes(), "ID", "Name", input.TargetSessionTypeID);
			input.UserList = CreateUserSelectList();
			return View(input);
		}

		// GET: Topics/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

			Topic topic = db.Topics.Find(id);

			if (topic == null)
				return HttpNotFound();
			else
			{
				var auth = topic.IsEditableBy(GetCurrentUser(), GetSession());
				if (!auth.IsAuthorized)
					throw new TopicLockedException(auth.Reason);
			}

			TopicEdit viewmodel = TopicEdit.FromTopic(topic);
			viewmodel.SessionTypeList = new SelectList(db.GetActiveSessionTypes(), "ID", "Name");
			viewmodel.TargetSessionTypeList = new SelectList(db.GetActiveSessionTypes(), "ID", "Name");
			viewmodel.UserList = CreateUserSelectList(viewmodel.OwnerID);

			return View(viewmodel);
		}

		// POST: Topics/Edit/5
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Exclude = "SessionTypeID")] TopicEdit input)
		{
			Topic topic = db.Topics.Include(t => t.Creator).Single(t => t.ID == input.ID);
			if (ModelState.IsValid)
			{
				var auth = topic.IsEditableBy(GetCurrentUser(), GetSession());
				if (!auth.IsAuthorized)
					return HTTPStatus(HttpStatusCode.Forbidden, auth.Reason);

				db.TopicHistory.Add(TopicHistory.FromTopic(topic, GetCurrentUserID()));

				topic.IncorporateUpdates(input);
				topic.TargetSessionTypeID = input.TargetSessionTypeID == topic.SessionTypeID ? null : input.TargetSessionTypeID;

				if (topic.TargetSessionTypeID > 0)
				{
					var voters = new HashSet<int>(topic.Votes.Select(v => v.Voter.ID));

					foreach (User user in db.SessionTypes
						.Include(st => st.Attendees)
						.Single(st => st.ID == input.TargetSessionTypeID)
						.Attendees.Where(user => !voters.Contains(user.ID)))
					{
						topic.Votes.Add(new Vote(user, VoteKind.None));
					}

					if (topic.Lock != null)
						topic.Lock.Action = TopicAction.None;
				}

				try
				{
					db.SaveChanges();
				}
				catch (DbEntityValidationException e)
				{
					var message = ErrorMessageFromException(e);
					return HTTPStatus(500, message);
				}

				return RedirectToAction("Details", new {Area = "", id = input.ID});
			}
			input.SessionType = topic.SessionType;
			input.SessionTypeList = new SelectList(db.GetActiveSessionTypes(), "ID", "Name", input.SessionTypeID);
			input.TargetSessionTypeList = new SelectList(db.GetActiveSessionTypes(), "ID", "Name", input.TargetSessionTypeID);
			input.UserList = CreateUserSelectList();
			return View(input);
		}

		// GET: Topics/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			Topic topic = db.Topics.Find(id);
			if (topic == null)
				return HttpNotFound();
			return View(topic);
		}

		// POST: Topics/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Topic topic = db.Topics.Find(id);
			db.Votes.RemoveRange(db.Votes.Where(v => v.Topic.ID == id));
			db.Topics.Remove(topic);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		// GET: Topics/ViewHistory/5
		public ActionResult ViewHistory(int id)
		{
			Topic topic = db.Topics.Find(id);
			var history = db.TopicHistory.Where(th => th.TopicID == topic.ID).OrderBy(th => th.ValidFrom).ToList();

			if (history.Count == 0)
				return RedirectToAction("Details", id);

			history.Add(TopicHistory.FromTopic(topic, 0));

			var vm = new TopicHistoryViewModel()
			{
				Usernames = db.Users.Where(u => u.IsActive).ToDictionary(u => u.ID, u => u.ShortName),
				SessionTypes = db.SessionTypes.ToDictionary(s => s.ID, s => s.Name),
				Current = topic,
				Initial = history[0]
			};

			var diff = new diff_match_patch()
			{
				Diff_Timeout = 0.4f,
			};

			// Anonyme Funktion, um die Biliothek diff_match_patch handlich zu verpacken.
			Func<string, string, List<Diff>> textDiff = (a, b) =>
			{
				var list = diff.diff_main(a, b);
				diff.diff_cleanupSemantic(list);
				return list;
			};

			foreach (var p in history.Pairwise())
			{
				vm.Differences.Add(new TopicHistoryDiff()
				{
					// Ein Eintrag entspricht später einer Box auf der Seite. Wenn keine Änderung existiert, sollte hier null gespeichert werden. Bei einer Änderung wird der NEUE Wert (der in Item2 enthalten ist) genommen.
					// SimpleDiff ist eine kleine helferfunktion, da die Zeilen sonst arg lang werden würden. Hier wird kein Text vergleichen - antweder hat sich alles geändert, oder gar nichts. (Daher "simple")
					// textDiff ist komplexer, hier wird der Text analysiert und auf ähnliche Abschnitte hin untersucht.
					Modified = p.Item1.ValidUntil,
					Editor = vm.Usernames[p.Item1.EditorID],
					SessionType = SimpleDiff(p.Item1.SessionTypeID, p.Item2.SessionTypeID, vm.SessionTypes),
					TargetSessionType = SimpleDiff(p.Item1.TargetSessionTypeID, p.Item2.TargetSessionTypeID, vm.SessionTypes, "(kein)"),
					Owner = SimpleDiff(p.Item1.OwnerID, p.Item2.OwnerID, vm.Usernames),
					Priority = p.Item1.Priority == p.Item2.Priority ? null : p.Item2.Priority.DisplayName(),
					Title = textDiff(p.Item1.Title, p.Item2.Title),
					Time = p.Item1.Time == p.Item2.Time ? null : p.Item2.Time,
					Description = textDiff(p.Item1.Description, p.Item2.Description),
					Proposal = textDiff(p.Item1.Proposal, p.Item2.Proposal)
				});
			}

			return View(vm);
		}

		/// <summary>
		///    Vergleicht die beiden IDs und gibt den Unterschied zurück. Bei Gleichheit wird null zurückgegeben.
		///    Bei verschiedenen Ids wird der Rückgabewert aus dem Dictionary anhand von idB ermittelt.
		/// </summary>
		/// <param name="idA">Erste ID</param>
		/// <param name="idB">Zweite ID</param>
		/// <param name="dict">Lookup für den Rückgabewert</param>
		/// <returns>null, wenn idA == idB, dict[idB] sonst.</returns>
		private string SimpleDiff(int idA, int idB, IDictionary<int, string> dict)
		{
			return idA == idB ? null : dict[idB];
		}

		/// <summary>
		///    Vergleicht die beiden IDs und gibt den Unterschied zurück. Bei Gleichheit wird null zurückgegeben.
		///    Bei verschiedenen Ids wird der Rückgabewert aus dem Dictionary anhand von idB ermittelt.
		/// </summary>
		/// <param name="idA">Erste ID</param>
		/// <param name="idB">Zweite ID</param>
		/// <param name="dict">Lookup für den Rückgabewert</param>
		/// <param name="defaultText">Standardtext für den Rückgabewert, wenn idB null ist</param>
		/// <returns>null, wenn idA == idB, dict[idB] wenn idB != null, defaultText sonst.</returns>
		private string SimpleDiff(int? idA, int? idB, IDictionary<int, string> dict, string defaultText)
		{
			return idA == idB ? null : (idB.HasValue ? dict[idB.Value] : defaultText);
		}
	}
}