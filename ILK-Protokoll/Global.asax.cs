using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Migrations;

namespace ILK_Protokoll
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
			
#if DEBUG
			foreach (Bundle bundle in BundleTable.Bundles)
				bundle.Transforms.Clear();
#endif
		}
	}

}