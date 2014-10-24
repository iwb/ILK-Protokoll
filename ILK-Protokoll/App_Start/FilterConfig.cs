using System.Web.Mvc;

namespace ILK_Protokoll
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			//filters.Add(new RequireHttpsAttribute());
			filters.Add(new AuthorizeAttribute { Roles = @"IWBMUC\ILK,IWBMUC\ILK-Proto" });
		}
	}
}