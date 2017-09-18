namespace Etymology.Data.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContextPool<EtymologyContext>(dbOptions => dbOptions
                .UseSqlServer(
                    configuration.GetConnectionString(nameof(Etymology)),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)));
    }
}
