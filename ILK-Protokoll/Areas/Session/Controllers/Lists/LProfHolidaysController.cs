using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LProfHolidaysController : ParentController<ProfHoliday>
	{
		public LProfHolidaysController()
		{
			_dbSet = db.LProfHolidays;
		}
	}
}