using System.Web;
using System.Web.Optimization;

namespace NtccSteward
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Scripts
            //
            bundles.Add(new ScriptBundle("~/bundles/angular", "//cdnjs.cloudflare.com/ajax/libs/angular.js/1.6.2/angular.min.js")
                 .Include("~/Scripts/angular.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery", "//code.jquery.com/jquery-3.1.1.min.js")
                 .Include("~/Scripts/jquery-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui", "//code.jquery.com/ui/1.11.4/jquery-ui.min.js")
                 .Include("~/Scripts/jquery-ui-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval", "//cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.11.1/jquery.validate.min.js")
                .Include("~/Scripts/jquery.validate.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr", "//cdnjs.cloudflare.com/ajax/libs/modernizr/2.6.2/modernizr.min.js")
                .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap", "//maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js")
                .Include("~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/respond", "//oss.maxcdn.com/respond/1.2.0/respond.min.js")
                .Include("~/Scripts/respond.min.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/site")
                .Include("~/Scripts/site.js"));
            //"~/App/appModule.js",
            //          "~/App/service/appService.js"
            // Styles
            bundles.Add(new StyleBundle("~/Content/bscss", "//maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css")
                .Include("~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/sitecss")
                .Include("~/Content/site.css"));
        }
    }
}
