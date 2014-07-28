using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ILK_Protokoll.Controllers
{
    public class ScheduledTasksController : Controller
    {
        // GET: ScheduledTasks
		 [AllowAnonymous]
        public string Index()
        {
            return "Diese Seite sollte auch ohne Authentifizierung sichtbar sein :-)";
        }
    }
}