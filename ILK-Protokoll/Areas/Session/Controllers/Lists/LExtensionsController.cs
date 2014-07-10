using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Areas.Session.Controllers.Lists
{
	public class LExtensionsController : ParentController<Extension>
	{
		public LExtensionsController()
		{
			_dbSet = db.LExtensions;
		}
	}
}