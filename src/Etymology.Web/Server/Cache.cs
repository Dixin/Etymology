namespace Etymology.Web.Server
{
    using System;
    using Microsoft.Extensions.Caching.Memory;

    internal static class Cache
    {
        internal static int ClientCacheMaxAge { get; } = (int)TimeSpan.FromDays(180).TotalSeconds;

        internal static MemoryCacheEntryOptions ServerCacheOptions { get; } = new() { SlidingExpiration = TimeSpan.FromHours(1) };
    }
}