#nullable enable
namespace Etymology.Data.Cache;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCharacterCache(this IServiceCollection services) =>
        services.AddScoped<ICharacterCache, CharacterCache>();
}