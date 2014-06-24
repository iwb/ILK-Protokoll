﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Controllers;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Administration.Controllers
{
	public class SessionTypesController : BaseController
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.AdminStyle = "active";
			ViewBag.ASTStyle = "active";
		}

		// GET: Administration/SessionTypes
		public ActionResult Index()
		{
			return View(db.SessionTypes.ToList());
		}

		// GET: Administration/SessionTypes/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SessionType sessionType = db.SessionTypes.Find(id);
			if (sessionType == null)
			{
				return HttpNotFound();
			}
			return View(sessionType);
		}

		// GET: SessionTypes/Create
		public ActionResult Create()
		{
			ViewBag.UserDict = db.GetUserOrdered(GetCurrentUser()).ToDictionary(u => u, u => false);
			return View();
		}

		// POST: Administration/SessionTypes/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "ID,Name")] SessionType sessionType, IEnumerable<int> Attendees)
		{
			if (ModelState.IsValid)
			{
				foreach (int userid in Attendees)
					sessionType.Attendees.Add(db.Users.Find(userid));

				db.SessionTypes.Add(sessionType);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.UserDict = db.GetUserOrdered(GetCurrentUser()).ToDictionary(u => u, u => Attendees.Contains(u.ID));
			return View(sessionType);
		}

		// GET: Administration/SessionTypes/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SessionType sessionType = db.SessionTypes.Find(id);
			if (sessionType == null)
			{
				return HttpNotFound();
			}
			ViewBag.UserDict = db.GetUserOrdered(GetCurrentUser()).ToDictionary(u => u, u => sessionType.Attendees.Contains(u));
			return View(sessionType);
		}

		// POST: Administration/SessionTypes/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "ID,Name")] SessionType input, IEnumerable<int> Attendees)
		{
			if (ModelState.IsValid)
			{
				var sessionType = db.SessionTypes.Find(input.ID);
				sessionType.Name = input.Name;
				sessionType.Attendees.Clear();

				if (Attendees != null)
					foreach (var userid in Attendees)
						sessionType.Attendees.Add(db.Users.Find(userid));

				db.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.UserDict = db.GetUserOrdered(GetCurrentUser()).ToDictionary(u => u, u => Attendees.Contains(u.ID));
			return View(input);
		}

		// GET: Administration/SessionTypes/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			SessionType sessionType = db.SessionTypes.Find(id);
			if (sessionType == null)
			{
				return HttpNotFound();
			}

			if (sessionType.Attendees.Count == 0)
				return View(sessionType);
			else
				return View("DeleteHint", sessionType);

		}

		// POST: Administration/SessionTypes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			SessionType sessionType = db.SessionTypes.Find(id);

			if (sessionType.Attendees.Count != 0)
				return View("DeleteHint", sessionType);

			db.SessionTypes.Remove(sessionType);
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}