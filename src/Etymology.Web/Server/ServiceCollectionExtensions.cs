namespace Etymology.Web.Server
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSettings<TSettings>(this IServiceCollection services, IConfiguration configuration, TSettings settings)
            where TSettings : class
        {
            configuration.Bind(settings);
            return services.AddSingleton(settings);
        }
    }
}
