using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Controllers
{
	public class TopicsController : Controller
	{
		private readonly DataContext _db = new DataContext();

		private User GetCurrentUser()
		{
			string username = User.Identity.Name.Split('\\').Last();
			return _db.Users.Single(x => x.Name.Equals(username, StringComparison.CurrentCultureIgnoreCase));
		}

		// GET: Topics
		public ActionResult Index()
		{
			var topics = _db.Topics.Include(t => t.SessionType).Include(t => t.TargetSessionType);
			return View(topics.ToList());
		}

		// GET: Topics/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Topic topic = _db.Topics.Find(id);
			if (topic == null)
			{
				return HttpNotFound();
			}
			return View(topic);
		}

		// GET: Topics/Create
		public ActionResult Create()
		{
			var viewmodel = new TopicEdit
			{
				SessionTypeList = new SelectList(_db.SessionTypes, "ID", "Name"),
				TargetSessionTypeList = new SelectList(_db.SessionTypes, "ID", "Name"),
				UserList = new SelectList(_db.GetUserOrdered(GetCurrentUser()), "ID", "Name")
			};
			return View(viewmodel);
		}

		// POST: Topics/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(
			[Bind(Include = "ID,Attachments,Description,Duties,OwnerID,Owner,Priority,Proposal,SessionTypeID,Title,ToDo")]
			TopicEdit input)
		{
			if (ModelState.IsValid)
			{
				var t = new Topic();
				t.IncorporateUpdates(input);
				t.SessionTypeID = input.SessionTypeID;
				_db.Topics.Add(t);
				_db.SaveChanges();
				return RedirectToAction("Index");
			}

			input.SessionTypeList = new SelectList(_db.SessionTypes, "ID", "Name", input.SessionTypeID);
			input.TargetSessionTypeList = new SelectList(_db.SessionTypes, "ID", "Name", input.TargetSessionTypeID);
			input.UserList = new SelectList(_db.GetUserOrdered(GetCurrentUser()), "ID", "Name");
			return View(input);
		}

		// GET: Topics/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Topic topic = _db.Topics.Find(id);
			if (topic == null)
			{
				return HttpNotFound();
			}

			var viewmodel = TopicEdit.FromTopic(topic);
			viewmodel.SessionTypeList = new SelectList(_db.SessionTypes, "ID", "Name");
			viewmodel.TargetSessionTypeList = new SelectList(_db.SessionTypes, "ID", "Name");
			viewmodel.UserList = new SelectList(_db.GetUserOrdered(GetCurrentUser()), "ID", "Name", viewmodel.OwnerID);

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
				var topic = _db.Topics.Find(input.ID);
				_db.TopicHistory.Add(TopicHistory.FromTopic(topic));

				topic.IncorporateUpdates(input);
				topic.TargetSessionTypeID = input.TargetSessionTypeID;
				_db.Entry(topic).State = EntityState.Modified;
				_db.SaveChanges();
				return RedirectToAction("Index");
			}
			var t = _db.Topics.Find(input.ID);
			input.SessionType = t.SessionType;
			input.SessionTypeList = new SelectList(_db.SessionTypes, "ID", "Name", input.SessionTypeID);
			input.TargetSessionTypeList = new SelectList(_db.SessionTypes, "ID", "Name", input.TargetSessionTypeID);
			input.UserList = new SelectList(_db.GetUserOrdered(GetCurrentUser()), "ID", "Name");
			return View(input);
		}

		// GET: Topics/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Topic topic = _db.Topics.Find(id);
			if (topic == null)
			{
				return HttpNotFound();
			}
			return View(topic);
		}

		// POST: Topics/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Topic topic = _db.Topics.Find(id);
			_db.Topics.Remove(topic);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}