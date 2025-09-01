using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ZiyoNur.Domain.Services;

namespace ZiyoNur.Service.Services.Implementations;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;

    public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            if (_memoryCache.TryGetValue(key, out var cachedValue))
            {
                if (cachedValue is T directValue)
                    return directValue;

                if (cachedValue is string jsonString)
                    return JsonSerializer.Deserialize<T>(jsonString);
            }

            return default(T);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cached value for key: {Key}", key);
            return default(T);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        try
        {
            var cacheOptions = new MemoryCacheEntryOptions();

            if (expiry.HasValue)
            {
                cacheOptions.AbsoluteExpirationRelativeToNow = expiry;
            }
            else
            {
                cacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1); // Default 1 hour
            }

            // Set cache priority
            cacheOptions.Priority = CacheItemPriority.Normal;

            if (value is string || value is ValueType)
            {
                _memoryCache.Set(key, value, cacheOptions);
            }
            else
            {
                var jsonString = JsonSerializer.Serialize(value);
                _memoryCache.Set(key, jsonString, cacheOptions);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cached value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        // MemoryCache doesn't support pattern removal
        // This would need to be implemented differently for Redis
        _logger.LogWarning("Pattern removal not supported in MemoryCache: {Pattern}", pattern);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return _memoryCache.TryGetValue(key, out _);
    }
}