using DistanceLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DistanceLearning {
    public class MvcApplication : HttpApplication {
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest() {
            if (User == null)
                return;

            using (DbManager db = new DbManager()) {

                String userEmail = Context.User.Identity.Name;
                User user = db.GetUserByEmail(userEmail);

                if (user == null)
                    return;

                string[] roles = db.GetUserRoles(db.GetUserByEmail(userEmail).UserId).Select(x => x.Role.Name).ToArray();

                IIdentity userIdentity = new GenericIdentity(userEmail);
                IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

                Context.User = newUserObj;
            }
        }

        //protected void Application_Error(object sender, EventArgs e) => Response.Redirect("/pages/error");
    }
}
