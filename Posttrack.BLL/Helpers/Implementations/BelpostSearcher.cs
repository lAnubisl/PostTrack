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
        private static readonly ILog log = LogManager.GetLogger(typeof (BelpostSearcher));

	    private static readonly string postDataTemplate =
			"__VIEWSTATE=%2FwEPDwULLTE1NTQ1NzEwNDQPZBYCAgMPZBYCAgkPZBYEAgEPPCsADQBkAgUPPCsADQBkGAMFHl9fQ29udHJvbHNSZXF1aXJlUG9zdEJhY2tLZXlfXxYBBQlJQnRuQnVuZXIFCUdyaWRJbmZvMA9nZAUIR3JpZEluZm8PZ2TjE6IJun2R3lPK64PrAWINq8FGcA%3D%3D&__VIEWSTATEGENERATOR=F0F1CCD9&__EVENTVALIDATION=%2FwEWBAKKhMC%2FDQLJk%2B%2FTAwL2nKzBDgLFnfXuCmVLc4Vgdc2vuaQ%2FqFCvmC%2BlOQ%2FS&TxtNumPos={0}&BtnSearch=GO";

		string IUpdateSearcher.Search(PackageDTO package)
        {
            if (package == null)
            {
                log.Error("package is null");
                return null;
            }

            using (var webClient = new WebClient())
            {
                webClient.Encoding = new UTF8Encoding();
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                var data = string.Format(CultureInfo.CurrentCulture, postDataTemplate, package.Tracking);
                log.DebugFormat("Start searching '{0}' at '{1}", package.Tracking, url);
                try
                {
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