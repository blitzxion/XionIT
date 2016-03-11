using System.Web;
using System.Web.Optimization;

namespace XionIT
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.UseCdn = true;

			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/respond.js"));

			// Sparkline
			bundles.Add(new ScriptBundle("~/plugins/sparkline").Include(
				"~/Scripts/plugins/sparkline/jquery.sparkline.min.js"));

			// Select2 (CDN and Fallback)
			bundles.Add(new ScriptBundle(@"~/plugins/select2", @"https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.2/js/select2.min.js").Include(
				@"~/Scripts/plugins/select2/select2.min.js"));

			bundles.Add(new StyleBundle(@"~/Content/select2", @"https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.2/css/select2.min.css").Include(
				@"~/Content/select2/select2.min.css"));

			// Overral Site CSS
			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap.css",
					  "~/Content/yeti.theme.css",
					  "~/Content/site.css"));

			// Site JS
			bundles.Add(new ScriptBundle("~/bundles/sitejs").Include(
				"~/Scripts/site.js"
			));

		}
	}
}
