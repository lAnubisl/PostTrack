using Microsoft.Extensions.Configuration;
using Posttrack.BLL;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.BLL.Interfaces;
using Posttrack.Data.MySql;
using System;
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
            var configurationService = new ConfigurationService();
            var anotherConfigurationService = new Posttrack.BLL.ConfigurationService(new SettingDAO(configurationService));
            IPackagePresentationService presentationService = new PackagePresentationService(
                new PackageDAO(configurationService),
                new EmailMessageSender(
                    anotherConfigurationService,
                    new SparkPostTemplateProvider(anotherConfigurationService)), 
                new BelpostSearcher(anotherConfigurationService), 
                new ResponseReader(anotherConfigurationService), 
                anotherConfigurationService);
            presentationService.UpdateComingPackages().Wait();
            Console.WriteLine("Done!");
        }
    }
}