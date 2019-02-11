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
    using Microsoft.Net.Http.Headers;

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
            Settings settings = new Settings();
            this.configuration.Bind(settings);
            services.AddSingleton(settings);

            services.AddMvc(options => options.AddAntiforgery());
            services.AddAntiforgery();

            services.AddDataAccess(settings.Connections[nameof(Etymology)]);

            services.AddResponseCaching();

            if (this.environment.IsProduction())
            {
                services.AddHttpsRedirection(options => options.HttpsPort = 443);
            }

            if (!this.environment.IsDevelopment())
            {
                services.AddApplicationInsightsTelemetry(this.configuration);
            }

            services.AddLogging(loggingBuilder =>
            {
                if (!this.environment.IsProduction())
                {
                    loggingBuilder.AddConsole(consoleLoggerOptions => consoleLoggerOptions.IncludeScopes = true)
                        .AddDebug();
                }
            });

            // Add support for GB18030.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public void Configure(IApplicationBuilder application, ILoggerFactory loggerFactory, IAntiforgery antiforgery, Settings settings) // HTTP pipeline.
        {
            if (this.environment.IsProduction())
            {
                application.UseExceptionHandler("/error");
                application.UseStatusCodePagesWithReExecute("/error");
                application.UseHsts();
                application.UseHttpsRedirection();

                loggerFactory.AddApplicationInsights(application.ApplicationServices);
            }
            else
            {
                application.UseDeveloperExceptionPage().UseBrowserLink();
            }

            application.UseAntiforgery(settings, antiforgery, loggerFactory.CreateLogger(nameof(RequestValidation)));

            application.UseDefaultFiles();
            application.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = staticFileResponseContext => staticFileResponseContext.Context.Response.Headers[HeaderNames.CacheControl] = $"public,max-age={Cache.ClientCacheMaxAge}"
            });

            application.UseResponseCaching();

            application.UseMvc(routes => routes.MapRoute(name: "default", template: "{controller}/{action}"));

            // Async Configure method is not supported by ASP.NET.
            // https://github.com/aspnet/Hosting/issues/373
        }
    }
}
