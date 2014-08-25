using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Controllers
{
    public class PushController : BaseController
    {
        // GET: Push
		 public PartialViewResult _EditListTopic(Topic topic)
		 {
			 var targets = new HashSet<int>(topic.PushTargets.Select(pt => pt.UserID));
			 var users = db.GetUserOrdered(GetCurrentUser());
			 var pushlist = users.ToDictionary(u => u, u => targets.Contains(u.ID));
			 ViewBag.TopicID = topic.ID;

			 return PartialView("_EditListTopic", pushlist);
		 }
		 public object _RegisterPushTargets(int topicID, Dictionary<int, bool> users)
		 {
			 var desired = new HashSet<int>(users.Where(kvp => kvp.Value).Select(kvp => kvp.Key));
			 var actual = new HashSet<int>(db.PushNotifications.Where(pn => pn.TopicID == topicID).Select(pn => pn.UserID));
			 
			 foreach (var userID in desired.Except(actual))
				 db.PushNotifications.Add(new PushNotification()
				 {
					 TopicID = topicID,
					 UserID = userID
				 });

			 foreach (var userID in actual.Except(desired))
				 db.PushNotifications.RemoveRange(db.PushNotifications.Where(pn => pn.TopicID == topicID && pn.UserID == userID));

			 db.SaveChanges();

			 return _EditListTopic(db.Topics.Find(topicID));
		 }
    }
}