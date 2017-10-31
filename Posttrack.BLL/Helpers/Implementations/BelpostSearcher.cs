using System;
using System.Net;
using System.Text;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Implementations
{
    public class BelpostSearcher : IUpdateSearcher
    {
        private readonly Uri url;
        //private static readonly ILog log = LogManager.GetLogger(typeof (BelpostSearcher));
        private readonly ISettingsProvider settings;

        public BelpostSearcher(ISettingsProvider settings)
        {
            this.settings = settings;
            this.url = new Uri(settings.HttpSearchUrl);
        }

        string IUpdateSearcher.Search(PackageDTO package)
        {
            if (package == null)
            {
                //log.Error("package is null");
                return null;
            }

            using (var webClient = new WebClient())
            {
                webClient.Encoding = new UTF8Encoding();
                //log.DebugFormat("Start searching '{0}' at '{1}", package.Tracking, url);
                try
                {
                    var response = webClient.DownloadString(url + "?search=" + package.Tracking);
                    //log.DebugFormat("Complete searching '{0}'. The response is '{1}'", package.Tracking, response);
                    return response;
                }
                catch (WebException ex)
                {
                    //log.ErrorFormat("Error checking '{0}': {1} \n {2}", package.Tracking, ex.Message, ex.StackTrace);
                    return null;
                }
            }
        }
    }
}