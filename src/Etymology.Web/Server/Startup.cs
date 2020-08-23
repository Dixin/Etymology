namespace Etymology.Web.Server
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Etymology.Data.Cache;
    using Etymology.Data.Models;
    using Microsoft.AspNetCore.Antiforgery;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;

    public class Startup
    {
        private const string Server = nameof(Server);

        private readonly IConfiguration configuration;

        private readonly IWebHostEnvironment environment;

        public Startup(IWebHostEnvironment environment)
        {
            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile(Path.Combine(Server, "settings.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(Path.Combine(Server, $"settings.{environment.EnvironmentName}.json"), optional: true, true)
                .AddEnvironmentVariables();
            if (!environment.IsDevelopment())
            {
                configurationBuilder.AddApplicationInsightsSettings(developerMode: environment.IsStaging());
            }

            this.configuration = configurationBuilder.Build();
            this.environment = environment;
        }

        public void ConfigureServices(IServiceCollection services) // Container.
        {
            services
                .AddSettings(this.configuration, out Settings settings)
                .AddAntiforgery(settings)
                .AddDataAccess(settings.Connections.TryGetValue(nameof(Etymology), out string? connection) && !string.IsNullOrWhiteSpace(connection)
                    ? connection
                    : this.configuration.GetSection(nameof(Etymology)).Value) // Environment variable.
                .AddResponseCaching()
                .AddCharacterCache()
                .AddLogging(loggingBuilder =>
                {
                    if (this.environment.IsProduction())
                    {
                        loggingBuilder.AddApplicationInsights();
                    }
                    else
                    {
                        loggingBuilder
                            .AddConsole(consoleLoggerOptions => consoleLoggerOptions.IncludeScopes = true)
                            .AddDebug();
                    }
                })
                .AddHostFiltering(hostFiltering => hostFiltering.AllowedHosts = settings.AllowedHosts)
                .AddMvc(options => options.AddAntiforgery()); // services.AddControllersWithViews(options => options.AddAntiforgery());

            services.AddRazorPages().AddRazorRuntimeCompilation();

            if (settings.IsHttpsOnly)
            {
                services.AddHttpsRedirection(options => options.HttpsPort = 443);
            }

            if (!this.environment.IsDevelopment())
            {
                services.AddApplicationInsightsTelemetry(this.configuration);
            }
        }

        // Async Configure method is not supported by ASP.NET.
        // https://github.com/aspnet/Hosting/issues/373
        public void Configure(IApplicationBuilder application, ILoggerFactory loggerFactory, IAntiforgery antiforgery, Settings settings) // HTTP pipeline.
        {
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (this.environment.IsProduction())
            {
                application
                    .UseExceptionHandler(settings.ErrorPagePath)
                    .UseStatusCodePagesWithReExecute(settings.ErrorPagePath);
            }
            else
            {
                application.UseDeveloperExceptionPage().UseBrowserLink();
            }

            if (settings.IsHttpsOnly)
            {
                application
                    .UseHsts()
                    .UseHttpsRedirection();
            }

            application
                .UseAntiforgery(settings, antiforgery, loggerFactory.CreateLogger(nameof(RequestValidation)))
                .UseDefaultFiles()
                .UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = staticFileResponseContext => staticFileResponseContext.Context.Response.Headers[HeaderNames.CacheControl] = $"public,max-age={Cache.ClientCacheMaxAge}",
                })
                .UseResponseCaching()
                .UseRouting()
                .UseEndpoints(endpoints => settings.Routes.ForEach(route => endpoints.MapControllerRoute(route.Key, route.Value)))
                .UseHostFiltering();

            // Add support for GB18030.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
