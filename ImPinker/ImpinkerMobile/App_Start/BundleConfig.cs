using System.Web;
using System.Web.Optimization;

namespace ImpinkerMobile
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

        }
    }
}