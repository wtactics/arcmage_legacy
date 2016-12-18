using System.Web;
using System.Web.Optimization;
using WTacticsService.Helpers;

namespace WTacticsService
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif

            // framework scripts
            bundles.Add(new ScriptBundle("~/bundles/sharedCode").Include(
                
                    "~/Scripts/angular.js",
                    "~/Scripts/angular-resource.js",
                    "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                    "~/Scripts/angular-sanitize.js",
                    "~/Scripts/angular-ui-router.js",
                    "~/Scripts/angular-md5.js",
                    "~/Scripts/slick/slick.js",
                    "~/Scripts/angular-slick.js",
                    "~/Scripts/angular-strap.js",
                    "~/Scripts/angular-strap.tpl.js",
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/modernizr-*",
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/xeditable.js",
                    "~/Scripts/respond.js"
            ));

            // application scripts
            bundles.Add(new ScriptBundle("~/bundles/code").Include(
                   "~/Views/Home/App.js",
                   "~/Views/Home/SharedServices.js",
                   "~/Views/Home/Cards/Cards.js",
                   "~/Views/Home/CardDetail/CardDetail.js",
                   "~/Views/Home/Decks/Decks.js",
                   "~/Views/Home/DeckDetail/DeckDetail.js",
                   "~/Views/Home/Games/Games.js",
                   "~/Views/Home/SignUp/SignUp.js",
                   "~/Views/Home/Index.js"
           ));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Scripts/slick/slick.css",
                      "~/Scripts/slick/slick-theme.css",
                      "~/Content/bootstrap.css",
                      "~/Content/theme.css",
                      "~/Content/site.css"));

            bundles.Add(new AngularTemplateBundle("wtacticsApp", "~/bundles/angularTemplates")
             .IncludeDirectory("~/Views/Home", "*.html", true));
        }
    }
}
