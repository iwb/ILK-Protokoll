using System.Web.Mvc;

namespace ILK_Protokoll.Areas.Administration
{
	public class AdministrationAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Administration"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				name: "Administration_default",
				url: "Administration/{controller}/{action}/{id}",
				defaults: new { controller = "AdminHome", action = "Index", id = UrlParameter.Optional }
				);
		}
	}
}