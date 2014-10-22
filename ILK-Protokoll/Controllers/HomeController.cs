using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.ViewModels;
using StackExchange.Profiling;

namespace ILK_Protokoll.Controllers
{
	[Authorize]
	public class HomeController : BaseController
	{
		public ActionResult Index()
		{
			var userID = GetCurrentUserID();
			var dash = new DashBoard();
			var profiler = MiniProfiler.Current;

			using (profiler.Step("Push-Nachrichten"))
			{
				dash.Notifications =
				db.PushNotifications.Include(pn => pn.Topic)
					.Where(pn => pn.UserID == userID && pn.Topic.IsReadOnly && !pn.Confirmed)
					.ToList();
			}
			using (profiler.Step("Aufgaben"))
			{
				var myAssignments = db.Assignments.Where(a => a.Owner.ID == userID && !a.IsDone && a.IsActive)
					.ToLookup(a => a.Type);
				dash.MyToDos = myAssignments[AssignmentType.ToDo];
				dash.MyDuties = myAssignments[AssignmentType.Duty].Where(a => a.Topic.HasDecision(DecisionType.Resolution));
			}
			
			dash.MyTopics = null; // Delayed AJAX loading
			return View(dash);
		}

		public ActionResult _FetchTopics()
		{
			var userID = GetCurrentUserID();
			List<Topic> topics;
			using (MiniProfiler.Current.Step("Themen"))
			{
				var cutoff = DateTime.Now.AddDays(3);
				topics = db.Topics
					.Include(t => t.SessionType)
					.Include(t => t.TargetSessionType)
					.Include(t => t.Owner)
					.Where(t => !t.IsReadOnly)
					.Where(t => t.ResubmissionDate == null || t.ResubmissionDate < cutoff)
					.Where(t => t.OwnerID == userID || t.Votes.Any(v => v.Voter.ID == userID))
					.OrderByDescending(t => t.Priority)
					.ThenByDescending(t => t.ValidFrom).ToList();
			}

			return PartialView("~/Views/Home/_Topics.cshtml", topics);
		}

		public ActionResult _FetchSingleTopic(int id)
		{
			return PartialView("~/Views/Topics/_Topic.cshtml", db.Topics.Find(id));
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}


		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}