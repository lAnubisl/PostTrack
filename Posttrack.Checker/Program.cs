using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Posttrack.BLL;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.Common;
using Posttrack.Data.MySql;

namespace PostTrack.Checker
{
    public static class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }

        public static void Main()
        {
            InitConfigurationRoot();
            LoggerConfiguration.ConfigureLogger("Posttrack.Checker", Configuration["log"]);
            var configurationService = new ConfigurationService();
            var logger = new Logger(nameof(Main));
            try
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                logger.Info("Checker started.");
                var settingsService = new SettingsService(new SettingDAO(configurationService, logger));
                var presentationService = new PackagePresentationService(
                    new PackageDAO(configurationService, logger),
                    new EmailMessageSender(settingsService, new SparkPostTemplateProvider(settingsService, logger), logger),
                    new BelpostSearcher(settingsService, logger),
                    new ResponseReader(settingsService, logger),
                    settingsService,
                    logger);
                presentationService.UpdateComingPackages().Wait();
            }
            catch (Exception ex)
            {
                logger.Log(ex);
                if (ex.InnerException != null)
                {
                    logger.Log(ex.InnerException);
                }
            }

            logger.Info("Checker finished.");
        }

        private static void InitConfigurationRoot()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Production.json", true);
            Configuration = builder.Build();
        }
    }
}