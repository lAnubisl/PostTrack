using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Posttrack.BLL;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.BLL.Interfaces;
using Posttrack.Common;
using Posttrack.Data.MySql;

namespace PostTrack.Checker
{
    public static class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }

        public static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Production.json", true);
            Configuration = builder.Build();
            LoggerConfiguration.ConfigureLogger("Posttrack.Checker", Configuration["log"]);
            var configurationService = new ConfigurationService();
            var logger = new Logger(nameof(Main));
            try
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                logger.Warning("Checker started.");
                var anotherConfigurationService = new Posttrack.BLL.ConfigurationService(
                    new SettingDAO(configurationService, logger));
                IPackagePresentationService presentationService = new PackagePresentationService(
                    new PackageDAO(configurationService, logger),
                    new EmailMessageSender(anotherConfigurationService, new SparkPostTemplateProvider(anotherConfigurationService, logger), logger),
                    new BelpostSearcher(anotherConfigurationService, logger),
                    new ResponseReader(anotherConfigurationService, logger),
                    anotherConfigurationService,
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

            logger.Warning("Checker finished.");
        }
    }
}