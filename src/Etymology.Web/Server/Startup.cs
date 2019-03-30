namespace Etymology.Web.Server
{
    using System.IO;
    using System.Text;
    using Etymology.Data.Cache;
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
            services.AddSettings(this.configuration, out Settings settings)
                .AddAntiforgery()
                .AddDataAccess(settings.Connections[nameof(Etymology)])
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
                .AddMvc(options => options.AddAntiforgery());

            if (this.environment.IsProduction())
            {
                services.AddHttpsRedirection(options => options.HttpsPort = 443);
            }

            if (!this.environment.IsDevelopment())
            {
                services.AddApplicationInsightsTelemetry(this.configuration);
            }
        }

        public void Configure(IApplicationBuilder application, ILoggerFactory loggerFactory, IAntiforgery antiforgery, Settings settings, EtymologyContext etymologyContext, ICharacterCache characterCache) // HTTP pipeline.
        {
            if (this.environment.IsProduction())
            {
                application
                    .UseExceptionHandler("/error")
                    .UseStatusCodePagesWithReExecute("/error")
                    .UseHsts()
                    .UseHttpsRedirection();
            }
            else
            {
                application.UseDeveloperExceptionPage().UseBrowserLink();
            }

            application
                .UseAntiforgery(settings, antiforgery, loggerFactory.CreateLogger(nameof(RequestValidation)))
                .UseDefaultFiles()
                .UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = staticFileResponseContext => staticFileResponseContext.Context.Response.Headers[HeaderNames.CacheControl] = $"public,max-age={Cache.ClientCacheMaxAge}"
                })
                .UseResponseCaching()
                .UseMvc(routes => routes.MapRoute(name: "default", template: "{controller}/{action}"));

            characterCache.Initialize(etymologyContext).Wait();
            // Async Configure method is not supported by ASP.NET.
            // https://github.com/aspnet/Hosting/issues/373

            // Add support for GB18030.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
