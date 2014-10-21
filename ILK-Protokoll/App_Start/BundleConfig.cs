using System.Web.Optimization;

namespace ILK_Protokoll
{
	public class BundleConfig
	{
		// Weitere Informationen zu Bundling finden Sie unter "http://go.microsoft.com/fwlink/?LinkId=301862".
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
				"~/Scripts/jquery-{version}.js",
				"~/Scripts/jquery.unobtrusive-ajax.js",
				"~/Scripts/jquery-ui-{version}.js"));

			bundles.Add(new Bundle("~/bundles/other").Include( // No minification
				"~/Scripts/jquery.timeago.js",
				"~/Scripts/spin.js",
				"~/Scripts/jquery.highlight.js",
				"~/Scripts/jquery.tablesorter.js",
				"~/Scripts/jquery.json.js",
				"~/Scripts/jquery.cookie.js",
				"~/Scripts/jquery.colpick.js",
				"~/Scripts/persistentTablesorter.js",
				"~/Scripts/bootstrap-multiselect.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
				"~/Scripts/jquery.validate*",
				"~/Scripts/bootstrap-validation.js"));

			// Verwenden Sie die Entwicklungsversion von Modernizr zum Entwickeln und Erweitern Ihrer Kenntnisse. Wenn Sie dann
			// für die Produktion bereit sind, verwenden Sie das Buildtool unter "http://modernizr.com", um nur die benötigten Tests auszuwählen.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
				"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
				"~/Scripts/bootstrap.js",
				"~/Scripts/respond.js",
				"~/Scripts/masonry_grid.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/themes/iwb/jquery-ui.*",
				"~/Content/other/jquery.rtResponsiveTables.css",
				"~/Content/other/colpick.css",
				"~/Content/bootstrap-multiselect/bootstrap-multiselect.css"));

			// Festlegen von EnableOptimizations auf false für Debugzwecke. Weitere Informationen
			// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=301862

#if DEBUG
			BundleTable.EnableOptimizations = false;
#else
			BundleTable.EnableOptimizations = true;
#endif
		}
	}
}