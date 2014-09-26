using System;
using System.Globalization;
using System.Net;
using System.Text;
using log4net;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Properties;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Implementations
{
    public class BelpostSearcher : IUpdateSearcher
    {
        private static readonly Uri url = new Uri(Settings.Default.HttpSearchUrl);
        private static readonly ILog log = LogManager.GetLogger(typeof(BelpostSearcher));

        string IUpdateSearcher.Search(PackageDTO package)
        {
            using (var webClient = new WebClient())
            {
                webClient.Encoding = new UTF8Encoding();
                webClient.Headers.Add("Accept", "text/html, */*");
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                var data = String.Format(CultureInfo.CurrentCulture, "item={0}&internal=2", package.Tracking);
                log.DebugFormat("Start searching '{0}' at '{1}", package.Tracking, url);
                try {
                    var response = webClient.UploadString(url, data);
                    log.DebugFormat("Complete searching '{0}'. The response is '{1}'", package.Tracking, response);
                    return response;
                }
                catch (WebException ex)
                {
                    log.ErrorFormat("Error checking '{0}': {1} \n {2}", package.Tracking, ex.Message, ex.StackTrace);
                    return null;
                }     
            }
        }
    }
}