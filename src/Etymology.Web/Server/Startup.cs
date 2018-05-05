namespace Etymology.Web.Server
{
    using System.IO;
    using System.Text;
    using Etymology.Data.Models;
    using Microsoft.AspNetCore.Antiforgery;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class Startup
    {
        private const string Server = nameof(Server);

        private readonly IConfiguration configuration;

        private readonly IHostingEnvironment environment;

        public Startup(IHostingEnvironment environment)
        {
            IConfigurationBuilder configuration = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile(Path.Combine(Server, "settings.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(Path.Combine(Server, $"settings.{environment.EnvironmentName}.json"), optional: true)
                .AddEnvironmentVariables();
            if (!environment.IsDevelopment())
            {
                configuration.AddApplicationInsightsSettings(developerMode: environment.IsStaging());
            }
            this.configuration = configuration.Build();
            this.environment = environment;
        }

        public void ConfigureServices(IServiceCollection services) // Container.
        {
            services.Configure<Settings>(options => this.configuration.Bind(options));

            services.AddMvc(options => options.AddAntiforgery());
            services.AddAntiforgery();

            services.AddDataAccess(this.configuration);

            services.AddResponseCaching();

            if (!this.environment.IsDevelopment())
            {
                services.AddApplicationInsightsTelemetry(this.configuration);
            }

            // Add support for GB18030.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public void Configure(IApplicationBuilder application, ILoggerFactory loggerFactory, IAntiforgery antiforgery, IOptions<Settings> options) // HTTP pipeline.
        {
            if (this.environment.IsProduction())
            {
                application.UseExceptionHandler("/error");
                application.UseStatusCodePagesWithReExecute("/error");

                loggerFactory.AddApplicationInsights(application.ApplicationServices);
            }
            else
            {
                application.UseDeveloperExceptionPage().UseBrowserLink();

                loggerFactory.AddConsole(LogLevel.Trace, true).AddDebug().AddFile("logs/{Date}.txt");
            }

            application.UseAntiforgery(options.Value, antiforgery, loggerFactory.CreateLogger(nameof(RequestValidation)));

            application.UseDefaultFiles();
            application.UseStaticFiles();

            application.UseResponseCaching();

            application.UseMvc(routes => routes.MapRoute(name: "default", template: "{controller}/{action}"));

            // Async Configure method is not supported by ASP.NET.
            // https://github.com/aspnet/Hosting/issues/373
        }
    }
}
