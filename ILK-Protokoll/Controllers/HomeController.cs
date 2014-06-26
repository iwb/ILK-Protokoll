using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Controllers
{
	public class HomeController : BaseController
	{
		public ActionResult Index()
		{
			var user = GetCurrentUser();
			var dash = new DashBoard();

			var myAssignments = db.Assignments.Where(a => a.Owner.ID == user.ID).ToLookup(a => a.Type);

			dash.MyToDos = myAssignments[AssignmentType.ToDo];
			dash.MyDuties = myAssignments[AssignmentType.Duty];

			dash.MyTopics = db.Topics
				.Include(t => t.SessionType)
				.Include(t => t.TargetSessionType)
				.Include(t => t.Owner)
				.Include(t => t.Votes)
				.Include(t => t.Votes.Select(v => v.Voter))
				.Include(t => t.Assignments)
				.Include(t => t.Comments).ToList();

			ViewBag.CurrentUser = user;
			return View(dash);
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