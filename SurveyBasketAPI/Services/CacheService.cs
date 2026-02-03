using Microsoft.Extensions.Caching.Distributed;
using SurveyBasketAPI.Services_Abstraction;
using System.Text.Json;

namespace SurveyBasketAPI.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IDistributedCache distributedCache, ILogger<CacheService> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken) where T : class
    {
        _logger.LogInformation("Attempting to retrieve cache for key: {Key}", key);
        var cachedData = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(cachedData))
        {
            _logger.LogWarning("Cache miss for key: {Key}", key);
            return null;
        }
        return JsonSerializer.Deserialize<T>(cachedData);
    }
    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken) where T : class
    {
        _logger.LogInformation("Set cache with key: {key}", key);
        var serializedData = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, serializedData, cancellationToken);
    }
    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Remove cache with key: {key}", key);

        await _distributedCache.RemoveAsync(key, cancellationToken);
    }

}
