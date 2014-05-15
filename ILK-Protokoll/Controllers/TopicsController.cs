using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
    public class TopicsController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Topics
        public ActionResult Index()
        {
            var topics = db.Topics.Include(t => t.SessionType).Include(t => t.TargetSessionType);
            return View(topics.ToList());
        }

        // GET: Topics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic topic = db.Topics.Find(id);
            if (topic == null)
            {
                return HttpNotFound();
            }
            return View(topic);
        }

        // GET: Topics/Create
        public ActionResult Create()
        {
            ViewBag.SessionTypeID = new SelectList(db.SessionTypes, "ID", "Name");
            ViewBag.TargetSessionTypeID = new SelectList(db.SessionTypes, "ID", "Name");
            return View();
        }

        // POST: Topics/Create
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SessionTypeID,TargetSessionTypeID,Title,Description,Proposal,Priority,Created,ValidFrom")] Topic topic)
        {
            if (ModelState.IsValid)
            {
                db.Topics.Add(topic);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SessionTypeID = new SelectList(db.SessionTypes, "ID", "Name", topic.SessionTypeID);
            ViewBag.TargetSessionTypeID = new SelectList(db.SessionTypes, "ID", "Name", topic.TargetSessionTypeID);
            return View(topic);
        }

        // GET: Topics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic topic = db.Topics.Find(id);
            if (topic == null)
            {
                return HttpNotFound();
            }
            ViewBag.SessionTypeID = new SelectList(db.SessionTypes, "ID", "Name", topic.SessionTypeID);
            ViewBag.TargetSessionTypeID = new SelectList(db.SessionTypes, "ID", "Name", topic.TargetSessionTypeID);
            return View(topic);
        }

        // POST: Topics/Edit/5
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SessionTypeID,TargetSessionTypeID,Title,Description,Proposal,Priority,Created,ValidFrom")] Topic topic)
        {
            if (ModelState.IsValid)
            {
                db.Entry(topic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SessionTypeID = new SelectList(db.SessionTypes, "ID", "Name", topic.SessionTypeID);
            ViewBag.TargetSessionTypeID = new SelectList(db.SessionTypes, "ID", "Name", topic.TargetSessionTypeID);
            return View(topic);
        }

        // GET: Topics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic topic = db.Topics.Find(id);
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
            Topic topic = db.Topics.Find(id);
            db.Topics.Remove(topic);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
