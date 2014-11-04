using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using StackExchange.Profiling;

namespace ILK_Protokoll
{
	/// <summary>
	/// Diese Klasse sogt für das einfache Profiling von Controlleraktionen.
	/// Quelle: http://samsaffron.com/archive/2011/07/25/Automatically+instrumenting+an+MVC3+app
	/// </summary>
	public class ProfilingActionFilter : ActionFilterAttribute
	{
		private const string StackKey = "ProfilingActionFilterStack";

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			MiniProfiler current = MiniProfiler.Current;
			if (current != null)
			{
				Stack<IDisposable> stack = HttpContext.Current.Items["ProfilingActionFilterStack"] as Stack<IDisposable>;
				if (stack == null)
				{
					stack = new Stack<IDisposable>();
					HttpContext.Current.Items["ProfilingActionFilterStack"] = stack;
				}
				RouteValueDictionary dataTokens = filterContext.RouteData.DataTokens;
				string str1 = !dataTokens.ContainsKey("area") || string.IsNullOrWhiteSpace((string)dataTokens["area"])
					? ""
					: (string)dataTokens["area"] + (object)".";
				string str2 = filterContext.Controller.ToString().Split(new char[1]
				{
					'.'
				}).Last() + ".";
				string actionName = filterContext.ActionDescriptor.ActionName;
				stack.Push(current.Step("Controller: " + str1 + str2 + actionName));
			}
			base.OnActionExecuting(filterContext);
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			base.OnActionExecuted(filterContext);
			Stack<IDisposable> stack = HttpContext.Current.Items["ProfilingActionFilterStack"] as Stack<IDisposable>;
			if (stack == null || stack.Count <= 0)
				return;
			stack.Pop().Dispose();
		}
	}
}