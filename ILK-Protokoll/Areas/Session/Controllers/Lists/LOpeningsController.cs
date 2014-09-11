using System.Linq;
using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LOpeningsController : ParentController<Opening>
	{
		public LOpeningsController()
		{
			_dbSet = db.LOpenings;
			Entities = _dbSet.OrderBy(o => o.Start).ThenBy(o => o.Project);
		}
	}
}