using Posttrack.Data.Interfaces;
using System;

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
