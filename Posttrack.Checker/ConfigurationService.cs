using Posttrack.Data.Interfaces;

namespace PostTrack.Checker
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetConnectionString()
        {
            return Program.Configuration["ConnectionStrings"];
        }
    }
}