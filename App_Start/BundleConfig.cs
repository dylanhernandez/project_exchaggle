using System.Web.Optimization;

namespace Exchaggle
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js","~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/ajax").Include("~/Scripts/script.exchaggle.ajax.js"));
            bundles.Add(new ScriptBundle("~/bundles/dropdowns").Include("~/Scripts/script.exchaggle.dropdowns.js", "~/Scripts/script.exchaggle.ajax.js"));
            bundles.Add(new ScriptBundle("~/bundles/image").Include("~/Scripts/script.exchaggle.image.js", "~/Scripts/script.exchaggle.prompt.js", "~/Scripts/sweetalert2.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/admin").Include("~/Scripts/script.exchaggle.ajax.js", "~/Scripts/script.exchaggle.admin.js", "~/Scripts/script.exchaggle.prompt.js", "~/Scripts/sweetalert2.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/common").Include("~/Scripts/script.exchaggle.prompt.js", "~/Scripts/sweetalert2.min.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/Bootstrap/bootstrap.css", "~/Content/Base/exchaggle-common.css", "~/Content/SweetAlert/sweetalert2.min.css"));
        }
    }
}
