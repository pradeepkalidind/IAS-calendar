using System;
using Microsoft.Extensions.Caching.Memory;

namespace Calendar.Service.Utils;

public class MemoryCacheAdapter
{
    private readonly MemoryCache cache = new MemoryCache(new MemoryCacheOptions
    {
        ExpirationScanFrequency = TimeSpan.FromMinutes(10)
    });

    public static MemoryCacheAdapter Default { get; } = new MemoryCacheAdapter();

    public bool Contains(string key)
    {
        return cache.TryGetValue(key, out _);
    }

    public object Get(string key)
    {
        return cache.TryGetValue(key,out var result) ? result : default;
    }

    public void Set(string key, object value)
    {
        cache.Set(key, value, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromHours(12)
        });
    }
}