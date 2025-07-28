using CashFlow.Application.Contracts;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CashFlow.Infrastructure.Cache
{
    public sealed class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

        public RedisCacheService(IDistributedCache cache) => _cache = cache;

        public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(value, _options);
            await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            }, ct);
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            var json = await _cache.GetStringAsync(key, ct);
            return json is null ? default : JsonSerializer.Deserialize<T>(json, _options);
        }

        public Task RemoveAsync(string key, CancellationToken ct = default) =>
            _cache.RemoveAsync(key, ct);
    }
}
