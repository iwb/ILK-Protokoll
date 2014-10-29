using System.IO;
using System.Web.Mvc;
using StackExchange.Profiling;

namespace ILK_Protokoll
{
	public class ProfilingViewEngine : IViewEngine
	{
		private readonly IViewEngine wrapped;

		public ProfilingViewEngine(IViewEngine wrapped)
		{
			this.wrapped = wrapped;
		}

		public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			var found = wrapped.FindPartialView(controllerContext, partialViewName, useCache);
			if (found != null && found.View != null)
				found = new ViewEngineResult(new WrappedView(found.View, partialViewName, isPartial: true), this);
			return found;
		}

		public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName,
			bool useCache)
		{
			var found = wrapped.FindView(controllerContext, viewName, masterName, useCache);
			if (found != null && found.View != null)
				found = new ViewEngineResult(new WrappedView(found.View, viewName, isPartial: false), this);
			return found;
		}

		public void ReleaseView(ControllerContext controllerContext, IView view)
		{
			wrapped.ReleaseView(controllerContext, view);
		}

		private class WrappedView : IView
		{
			private readonly bool isPartial;
			private readonly string name;
			private readonly IView wrapped;

			public WrappedView(IView wrapped, string name, bool isPartial)
			{
				this.wrapped = wrapped;
				this.name = name;
				this.isPartial = isPartial;
			}

			public void Render(ViewContext viewContext, TextWriter writer)
			{
				using (MiniProfiler.Current.Step("Render " + (isPartial ? "partial" : "") + ": " + name))
				{
					wrapped.Render(viewContext, writer);
				}
			}
		}
	}
}