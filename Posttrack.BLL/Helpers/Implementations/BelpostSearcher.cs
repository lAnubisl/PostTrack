using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
using Posttrack.Common;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Implementations
{
    public class BelpostSearcher : IUpdateSearcher
    {
        private readonly Uri _url;
        private readonly ILogger _logger;
        private readonly IConfigurationService _configurationService;

        public BelpostSearcher(IConfigurationService configurationService, ILogger logger)
        {
            _logger = logger.CreateScope(nameof(BelpostSearcher));
            _configurationService = configurationService;
            _url = configurationService.HttpSearchUrl;
        }

        public async Task<string> SearchAsync(PackageDTO package)
        {
            if (package == null)
            {
                _logger.Error("package is null");
                return null;
            }

            using (var webClient = new WebClient())
            {
                webClient.Encoding = new UTF8Encoding();
                _logger.Debug($"Start searching '{package.Tracking}' at '{_url}");
                try
                {
                    var response = await webClient.DownloadStringTaskAsync(_url + package.Tracking);
                    _logger.Debug($"Complete searching '{package.Tracking}'. The response is '{response}'.");
                    return response;
                }
                catch (WebException ex)
                {
                    _logger.Log(ex);
                    return null;
                }
            }
        }
    }
}