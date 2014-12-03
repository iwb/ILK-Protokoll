using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ILK_Protokoll.DataLayer;
using ILK_Protokoll.Migrations;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;
using StackExchange.Profiling.SqlFormatters;

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

			// Profiler Setup
			GlobalFilters.Filters.Add(new ProfilingActionFilter());
			MiniProfilerEF6.Initialize();
			MiniProfiler.Settings.SqlFormatter = new SqlServerFormatter();

			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new ProfilingViewEngine(new RazorViewEngine()));
			// Profiler Setup beendet

			Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());

#if DEBUG
			foreach (Bundle bundle in BundleTable.Bundles)
				bundle.Transforms.Clear();
#endif
		}

		protected void Application_BeginRequest()
		{
			if (true)
				MiniProfiler.Start();
		}

		protected void Application_EndRequest()
		{
			MiniProfiler.Stop();
				//stop as early as you can, even earlier with MvcMiniProfiler.MiniProfiler.Stop(discardResults: true);
		}
	}
}