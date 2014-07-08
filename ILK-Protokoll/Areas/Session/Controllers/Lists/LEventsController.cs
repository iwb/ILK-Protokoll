using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LEventsController : ParentController<Event>
	{
		public LEventsController()
		{
			_dbSet = db.LEvents;
		}
	}
}