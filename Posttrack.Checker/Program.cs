using Microsoft.Extensions.Configuration;
using Posttrack.BLL;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.BLL.Interfaces;
using Posttrack.Common;
using Posttrack.Data.MySql;
using System;
using System.Diagnostics;
using System.IO;

namespace PostTrack.Checker
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; private set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            LoggerConfiguration.ConfigureLogger("Posttrack.Checker", Configuration["log"]);
            var configurationService = new ConfigurationService();
            var logger = new Logger(nameof(Main));
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            logger.Info("Checker started.");
            var anotherConfigurationService = new Posttrack.BLL.ConfigurationService(
                new SettingDAO(configurationService, logger));
            IPackagePresentationService presentationService = new PackagePresentationService(
                new PackageDAO(configurationService, logger),
                new EmailMessageSender(
                    anotherConfigurationService,
                    new SparkPostTemplateProvider(anotherConfigurationService, logger),
                    logger), 
                new BelpostSearcher(anotherConfigurationService, logger), 
                new ResponseReader(anotherConfigurationService, logger), 
                anotherConfigurationService,
                logger);
            presentationService.UpdateComingPackages().Wait();
            logger.Info("Checker finished.");
        }
    }
}