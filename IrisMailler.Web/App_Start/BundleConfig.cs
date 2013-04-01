using System.Web;
using System.Web.Optimization;

namespace IrisMailler.Web
{
	public class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/Librairies/JQuery/jquery-{version}.js"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/Librairies/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/Librairies/bootstrap.js"));

			bundles.Add(new ScriptBundle("~/bundles/angular").Include("~/Scripts/Librairies/Angular/angular.js"));
		}
	}
}