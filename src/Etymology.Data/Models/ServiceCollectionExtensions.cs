#nullable enable
namespace Etymology.Data.Models;

using Microsoft.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connection)
    {
        if (string.IsNullOrWhiteSpace(connection))
        {
            throw new InvalidOperationException("Failed to get connection string.");
        }

        return services.AddDbContextPool<EtymologyContext>(dbOptions => dbOptions.UseSqlServer(
            connection,
            sqlOptions => sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)));
    }
}