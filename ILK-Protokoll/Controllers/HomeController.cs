using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.ViewModels;
using StackExchange.Profiling;

namespace ILK_Protokoll.Controllers
{
	/// <summary>
	///    Dies ist die Startseite, deren Index()-methode auch ohne Angabe des Controllers ausgelöst werden kann. ~/ verweist
	///    also auf diese Index()-Methode.
	/// </summary>
	[Authorize]
	public class HomeController : BaseController
	{
		// GET: Home/Index/
		/// <summary>
		///    Hier wird das Dashboard generiert. Das Dashboard ist die Startseite des ILK-Protokolls und der Ausgangspunkt für
		///    alle weiteren Aktionenn des Nutzers. Es wird das ViewModel <see cref="DashBoard" /> genutzt, um die Daten zu
		///    verpacken. Wird verzögertes Laden eingesetzt, muss die Property <see cref="DashBoard.MyTopics" /> auf null gesetzt
		///    werden. Die View generiert dann Code, der die Themen nachträglich von der Methode <see cref="_FetchTopics" />
		///    anfordert.
		/// </summary>
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

		// AJAX: Home/_FetchTopics/
		/// <summary>
		///    Die Auflistung aller Themen des Dashboard wird generiert. Ausgehend von dieser kann der Benutzer direkt abstimmen,
		///    Kommentare schreiben oder Anhänge aufrufen.
		/// </summary>
		public ActionResult _FetchTopics()
		{
			var userID = GetCurrentUserID();
			List<Topic> topics;
			using (MiniProfiler.Current.Step("Themen"))
			{
				var cutoff = DateTime.Now.AddDays(3);
				topics = db.Topics
					.Include(t => t.Comments)
					.Include(t => t.Documents)
					.Include(t => t.Lock)
					.Include(t => t.Owner)
					.Include(t => t.SessionType)
					.Include(t => t.Tags)
					.Include(t => t.TargetSessionType)
					.Include(t => t.UnreadBy)
					.Include(t => t.Votes)
					.Where(t => !t.IsReadOnly)
					.Where(t => t.ResubmissionDate == null || t.ResubmissionDate < cutoff)
					.Where(t => t.OwnerID == userID || t.Votes.Any(v => v.Voter.ID == userID))
					.OrderByDescending(t => t.Priority)
					.ThenByDescending(t => t.ValidFrom).ToList();
			}

			return PartialView("~/Views/Home/_Topics.cshtml", topics);
		}

		/// <summary>
		///    Diese Methode rendert eine einzige Diskussion und gibt diese zurück.
		/// </summary>
		/// <param name="id">TopicID</param>
		public ActionResult _FetchSingleTopic(int id)
		{
			return PartialView("~/Views/Topics/_Topic.cshtml", db.Topics.Find(id));
		}

		/// <summary>
		///    Grundlegende Informationen über das ILK-Protokoll. Diese Seite ist nicht verlinkt.
		/// </summary>
		public ActionResult About()
		{
			return View();
		}
	}
}