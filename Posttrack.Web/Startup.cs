using Microsoft.AspNetCore.Builder;
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
        public static IConfiguration Configuration {get; private set;}

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            LoggerConfiguration.ConfigureLogger("Posttrack.Web", Configuration.GetConnectionString("log"));
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
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
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