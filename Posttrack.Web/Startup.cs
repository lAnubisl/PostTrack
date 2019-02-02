using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Posttrack.BLL;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
using Posttrack.Common;
using Posttrack.Data.Interfaces;
using Posttrack.Data.MySql;

namespace Posttrack.Web
{
    public class Startup
    {
        private static ILogger _logger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            LoggerConfiguration.ConfigureLogger("Posttrack.Web", Configuration.GetConnectionString("log"));
            _logger = new Logger("Root");
            services.AddSingleton<Data.Interfaces.IConfigurationService, ConfigurationService>();
            services.AddSingleton<IPackageDAO, PackageDAO>();
            services.AddSingleton<ISettingDAO, SettingDAO>();
            services.AddSingleton<IPackagePresentationService, PackagePresentationService>();
            services.AddSingleton<IPackageValidator, PackageValidator>();
            services.AddSingleton<BLL.Interfaces.IConfigurationService, BLL.ConfigurationService>();
            services.AddSingleton<IUpdateSearcher, BelpostSearcher>();
            services.AddSingleton<IMessageSender, EmailMessageSender>();
            services.AddSingleton<ISparkPostTemplateProvider, SparkPostTemplateProvider>();
            services.AddSingleton<IResponseReader, ResponseReader>();
            services.AddSingleton<ILogger>(new Logger("Root"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            _logger.Info("Application started.");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions()
            {
                ExceptionHandler = (ctx) =>
                {
                    var feature = ctx.Features.Get<IExceptionHandlerFeature>();
                    _logger.Log(feature.Error);
                    return Task.FromResult(0);
                }
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseTraceId();
            app.UseMvc();
        }
    }
}