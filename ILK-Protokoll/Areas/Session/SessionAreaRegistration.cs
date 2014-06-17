using System.Web.Mvc;

namespace ILK_Protokoll.Areas.Session
{
	public class SessionAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Session"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				name: "Session_default",
				url: "Session/{controller}/{action}/{id}",
				defaults: new { controller = "Master", action = "Index", id = UrlParameter.Optional }
				);
		}
	}
}