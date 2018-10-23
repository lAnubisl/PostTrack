using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Posttrack.BLL;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}