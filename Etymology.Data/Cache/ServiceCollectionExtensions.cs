namespace Etymology.Data.Cache
{
    using System.IO;
    using Etymology.Data.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            services.AddSingleton(_ => new ImageCache(
                serviceProvider.GetService<EtymologyContext>(),
                Path.Combine(Directory.GetCurrentDirectory(), configuration[nameof(ImageCache)])));
            return services;
        }
    }
}
