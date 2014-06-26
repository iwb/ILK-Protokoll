using System;
using System.Linq;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Controllers
{
	public class AssignmentsController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.AssignmentStyle = "active";
		}

		// GET: Assignments
		public ActionResult Index([Bind(Include = "ShowPast, ShowFuture, ShowDone, UserID")] FilteredAssignments filter)
		{
			IQueryable<Assignment> query = db.Assignments;

			if (!filter.ShowToDos)
				query = query.Where(a => a.Type != AssignmentType.ToDo);

			if (!filter.ShowDuties)
				query = query.Where(a => a.Type != AssignmentType.Duty);

			if (!filter.ShowPast)
				query = query.Where(a => a.DueDate > DateTime.Today);

			if (!filter.ShowFuture)
				query = query.Where(a => a.DueDate < DateTime.Today);

			if (!filter.ShowDone)
				query = query.Where(a => !a.IsDone);

			if (filter.UserID != 0)
				query = query.Where(a => a.Owner.ID == filter.UserID);

			filter.Assignments = query.ToList();

			return View(filter);
		}

		// GET: Assignments/Details/5
		public ActionResult Details(int id)
		{
			return null;
		}
	}
}