using System.Data.Entity;
using System.Linq;
using System.Reflection;
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
			GlobalFilters.Filters.Add(new StackExchange.Profiling.Mvc.ProfilingActionFilter());
			MiniProfilerEF6.Initialize();
			MiniProfiler.Settings.SqlFormatter = new SqlServerFormatter();

			var copy = ViewEngines.Engines.ToList();
			ViewEngines.Engines.Clear();
			foreach (var item in copy)
				ViewEngines.Engines.Add(new ProfilingViewEngine(item));
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
			{
				MiniProfiler.Start();
			} //or any number of other checks, up to you 
		}

		protected void Application_EndRequest()
		{
			MiniProfiler.Stop(); //stop as early as you can, even earlier with MvcMiniProfiler.MiniProfiler.Stop(discardResults: true);
		}
	}


	public class ProfilingViewEngine : IViewEngine
	{
		class WrappedView : IView
		{
			IView wrapped;
			string name;
			bool isPartial;

			public WrappedView(IView wrapped, string name, bool isPartial)
			{
				this.wrapped = wrapped;
				this.name = name;
				this.isPartial = isPartial;
			}

			public void Render(ViewContext viewContext, System.IO.TextWriter writer)
			{
				using (MiniProfiler.Current.Step("Render " + (isPartial ? "partial" : "") + ": " + name))
				{
					wrapped.Render(viewContext, writer);
				}
			}
		}

		IViewEngine wrapped;

		public ProfilingViewEngine(IViewEngine wrapped)
		{
			this.wrapped = wrapped;
		}

		public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			var found = wrapped.FindPartialView(controllerContext, partialViewName, useCache);
			if (found != null && found.View != null)
			{
				found = new ViewEngineResult(new WrappedView(found.View, partialViewName, isPartial: true), this);
			}
			return found;
		}

		public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			var found = wrapped.FindView(controllerContext, viewName, masterName, useCache);
			if (found != null && found.View != null)
			{
				found = new ViewEngineResult(new WrappedView(found.View, viewName, isPartial: false), this);
			}
			return found;
		}

		public void ReleaseView(ControllerContext controllerContext, IView view)
		{
			wrapped.ReleaseView(controllerContext, view);
		}
	}
}