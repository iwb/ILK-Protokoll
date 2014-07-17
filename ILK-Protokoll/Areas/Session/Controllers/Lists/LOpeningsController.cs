using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LOpeningsController : ParentController<Opening>
	{
		public LOpeningsController()
		{
			_dbSet = db.LOpenings;
		}
	}
}