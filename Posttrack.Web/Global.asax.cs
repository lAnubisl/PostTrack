using System.Web;
using System.Web.Http;
using Posttrack.DI;
using Posttrack.Web.Controllers;
using Posttrack.BLL.Interfaces;

namespace Posttrack.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute("DefaultApi", "tracking",
                new {controller = "Tracking", action = "index"});
            InversionOfControlContainer.Instance.RegisterTransient<TrackingController, TrackingController>();
            //InversionOfControlContainer.Instance.RegisterSingleton<ISettingsProvider, SettingsProvider>();
            GlobalConfiguration.Configuration.DependencyResolver =
                new DependencyResolver(InversionOfControlContainer.Instance);
        }
    }
}