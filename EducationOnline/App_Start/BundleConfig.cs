using System.Web;
using System.Web.Optimization;

namespace DistanceLearning {
    public class BundleConfig {
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/js/Common").Include(
                      "~/Content/js/Common.js"));

            bundles.Add(new ScriptBundle("~/js/Courses").Include(
                     "~/Content/js/Courses.js"));

            bundles.Add(new ScriptBundle("~/js/Course").Include(
                   "~/Content/js/Course.js"));

            bundles.Add(new ScriptBundle("~/js/CourseTopic").Include(
                  "~/Content/js/CourseTopic.js"));

            bundles.Add(new ScriptBundle("~/js/Login").Include(
                  "~/Content/js/Login.js"));

            bundles.Add(new ScriptBundle("~/js/Test").Include(
                  "~/Content/js/Test.js"));

            bundles.Add(new ScriptBundle("~/js/Lab").Include(
               "~/Content/js/Lab.js"));

            //ADMIN
            bundles.Add(new ScriptBundle("~/js/Admin/Table").Include(
           "~/Content/js/Admin/Table.js"));

            bundles.Add(new ScriptBundle("~/js/Admin/Search").Include(
          "~/Content/js/Admin/Search.js"));

            //STYLES
            bundles.Add(new StyleBundle("~/common").Include(
                    "~/Content/css/common.css"));

            bundles.Add(new StyleBundle("~/courses").Include(
                     "~/Content/css/courses.css"));

            bundles.Add(new StyleBundle("~/form").Include(
                     "~/Content/css/form.css"));

            bundles.Add(new StyleBundle("~/lab").Include(
                       "~/Content/css/lab.css"));

            bundles.Add(new StyleBundle("~/labzoom").Include(
                     "~/Content/css/labzoom.css"));

            bundles.Add(new StyleBundle("~/test").Include(
                     "~/Content/css/test.css"));

            bundles.Add(new StyleBundle("~/admin").Include(
                  "~/Content/css/admin.css"));

            bundles.Add(new StyleBundle("~/table").Include(
                "~/Content/css/table.css"));

            bundles.Add(new StyleBundle("~/profile").Include(
               "~/Content/css/profile.css"));

            bundles.Add(new StyleBundle("~/main").Include(
             "~/Content/css/main.css"));
        }
    }
}
