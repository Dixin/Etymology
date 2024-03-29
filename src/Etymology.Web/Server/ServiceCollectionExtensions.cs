﻿namespace Etymology.Web.Server;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSettings<TSettings>(this IServiceCollection services, IConfiguration configuration, out TSettings settings)
        where TSettings : class
    {
        settings = configuration.Get<TSettings>();
        return services.AddSingleton(settings);
    }
}