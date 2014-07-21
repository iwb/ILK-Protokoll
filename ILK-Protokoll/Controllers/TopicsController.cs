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
		public ActionResult Index()
		{
			IQueryable<Topic> topics = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.TargetSessionType);
			return View(topics.ToList());
		}

		// GET: Topics/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
				return HTTPStatus(400, "Für diesen Vorgang ist eine TopicID ist erforderlich.");

			var topic = db.Topics
				.Include(t => t.Votes)
				.Include(t => t.Assignments)
				.Include(t => t.Attachments)
				.Include(t => t.Creator)
				.Include(t => t.Lock)
				.Include(t => t.Lock.Session.Manager)
				.Single(t => t.ID == id.Value);

			if (topic == null)
				return HttpNotFound();

			ViewBag.TopicID = id.Value;
			ViewBag.TopicHistory = db.TopicHistory.Where(t => t.TopicID == id.Value).ToList();
			ViewBag.IsEditable = topic.IsEditableBy(GetCurrentUser(), GetSession()).IsAuthorized;
			topic.IsLocked = IsTopicLocked(id.Value);

			return View(topic);
		}

		// GET: Topics/Create
		public ActionResult Create()
		{
			var viewmodel = new TopicEdit
			{
				SessionTypeList = new SelectList(db.SessionTypes, "ID", "Name"),
				TargetSessionTypeList = new SelectList(db.SessionTypes, "ID", "Name"),
				UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName")
			};
			return View(viewmodel);
		}

		// POST: Topics/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(
			[Bind(Include = "ID,Attachments,Description,Duties,OwnerID,Owner,Priority,Proposal,SessionTypeID,Title,ToDo")] TopicEdit input)
		{
			if (ModelState.IsValid)
			{
				var t = new Topic
				{
					Creator = GetCurrentUser(),
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

			input.SessionTypeList = new SelectList(db.SessionTypes, "ID", "Name", input.SessionTypeID);
			input.TargetSessionTypeList = new SelectList(db.SessionTypes, "ID", "Name", input.TargetSessionTypeID);
			input.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
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
			viewmodel.SessionTypeList = new SelectList(db.SessionTypes, "ID", "Name");
			viewmodel.TargetSessionTypeList = new SelectList(db.SessionTypes, "ID", "Name");
			viewmodel.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName", viewmodel.OwnerID);

			return View(viewmodel);
		}

		// POST: Topics/Edit/5
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(
			[Bind(Include = "ID,Attachments,Description,Duties,OwnerID,Priority,Proposal,TargetSessionTypeID,Title,ToDo")] TopicEdit input)
		{
			if (ModelState.IsValid)
			{
				Topic topic = db.Topics.Find(input.ID);

				var auth = topic.IsEditableBy(GetCurrentUser(), GetSession());
				if (!auth.IsAuthorized)
					return HTTPStatus(HttpStatusCode.Forbidden, auth.Reason);

				db.TopicHistory.Add(TopicHistory.FromTopic(topic, GetCurrentUser().ID));

				topic.IncorporateUpdates(input);
				topic.TargetSessionTypeID = input.TargetSessionTypeID;
				db.Entry(topic).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Details", new {Area = "", id = input.ID});
			}
			Topic t = db.Topics.Find(input.ID);
			input.SessionType = t.SessionType;
			input.SessionTypeList = new SelectList(db.SessionTypes, "ID", "Name", input.SessionTypeID);
			input.TargetSessionTypeList = new SelectList(db.SessionTypes, "ID", "Name", input.TargetSessionTypeID);
			input.UserList = new SelectList(db.GetUserOrdered(GetCurrentUser()), "ID", "ShortName");
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

			foreach (var p in history.Pairwise())
			{
				vm.Differences.Add(new TopicHistoryDiff()
				{
					Modified = p.Item1.ValidUntil,
					Editor = vm.Usernames[p.Item1.EditorID],
					SessionType = SimpleDiff(p.Item1.SessionTypeID, p.Item2.SessionTypeID, vm.SessionTypes),
					Owner = SimpleDiff(p.Item1.OwnerID, p.Item2.OwnerID, vm.Usernames),
					Priority = p.Item1.Priority == p.Item2.Priority ? null : p.Item2.Priority.DisplayName(),
					Title = diff.diff_main(p.Item1.Title, p.Item2.Title),
					Description = diff.diff_main(p.Item1.Description, p.Item2.Description),
					Proposal = diff.diff_main(p.Item1.Proposal, p.Item2.Proposal)
				});
			}

			return View(vm);
		}

		private string SimpleDiff(int idA, int idB, IDictionary<int, string> dict)
		{
			return idA == idB ? null : dict[idB];
		}
	}
}