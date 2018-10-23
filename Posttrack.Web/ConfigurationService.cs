using Microsoft.Extensions.Configuration;
using Posttrack.Data.Interfaces;

namespace Posttrack.Web
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetConnectionString()
        {
            return Startup.Configuration.GetConnectionString("default");
        }
    }
}