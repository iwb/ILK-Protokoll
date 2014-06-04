using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ILK_Protokoll.Controllers
{
	public class AttachmentsController : BaseController
	{
		// GET: Attachments
		public ActionResult Index()
		{
			return View();
		}

		// GET: Attachments/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: Attachments/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Attachments/Create
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

		// GET: Attachments/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: Attachments/Edit/5
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

		// GET: Attachments/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: Attachments/Delete/5
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
