using Posttrack.DI;
using Posttrack.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Posttrack.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute("DefaultApi", "tracking", new { controller = "Tracking", action = "index" });
            InversionOfControlContainer.Instance.RegisterController(typeof(TrackingController));
            GlobalConfiguration.Configuration.DependencyResolver = new DependencyResolver(InversionOfControlContainer.Instance);
        }
    }
}