namespace Etymology.Web
{
    using Etymology.Data.Cache;
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
        public Startup(IHostingEnvironment environment)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("settings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"settings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.AddAntiforgery());
            services.AddDataAccess(this.Configuration);
            services.AddCache(this.Configuration);
            services.AddAntiforgery();
            services.Configure<Settings>(options => this.Configuration.Bind(options));
        }

        public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment environment, ILoggerFactory loggerFactory, ImageCache imageCache, IAntiforgery antiforgery, IOptions<Settings> options)
        {
            applicationBuilder.UseAntiforgery(options.Value, antiforgery);

            if (environment.IsProduction())
            {
                applicationBuilder.UseExceptionHandler("/error");
                applicationBuilder.UseStatusCodePagesWithReExecute("/error");

                // Async Configure method is not supported by ASP.NET.
                // https://github.com/aspnet/Hosting/issues/373
                // imageCache.SaveWithRetryAsync().Wait();
            }
            else
            {
                loggerFactory.AddConsole(LogLevel.Trace, true).AddDebug().AddFile("logs/{Date}.txt");
                applicationBuilder.UseDeveloperExceptionPage().UseBrowserLink();
            }

            applicationBuilder.UseDefaultFiles();
            applicationBuilder.UseStaticFiles();

            applicationBuilder.UseMvc(routes => routes.MapRoute(name: "default", template: "{controller}/{action}"));
        }
    }
}
