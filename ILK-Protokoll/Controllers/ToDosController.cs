using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ILK_Protokoll.Controllers
{
	public class ToDosController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.AssignmentStyle = "active";
		}
		// GET: ToDos
		public ActionResult Index()
		{
			return View();
		}

		// GET: ToDos/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: ToDos/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: ToDos/Create
		[HttpPost]
		public ActionResult Create(FormCollection collection)
		{
			try
			{
				// TODO: Add insert logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		// GET: ToDos/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: ToDos/Edit/5
		[HttpPost]
		public ActionResult Edit(int id, FormCollection collection)
		{
			try
			{
				// TODO: Add update logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		// GET: ToDos/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: ToDos/Delete/5
		[HttpPost]
		public ActionResult Delete(int id, FormCollection collection)
		{
			try
			{
				// TODO: Add delete logic here

				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}
	}
}
