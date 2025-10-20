using Microsoft.Extensions.Logging;
using NHibernate.Cache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UserManagement.Application.Interfaces;

namespace UserManagment.Cache.Redis
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private bool _disposed;

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _db = _redis.GetDatabase();

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                if (!_redis.IsConnected)
                    return false;

                var pingResult = await _db.PingAsync();
                return pingResult != TimeSpan.Zero;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis connection check failed");
                return false;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            ValidateKey(key);

            if (value == null)
            {
                _logger.LogWarning("Attempted to cache null value for key: {Key}", key);
                return;
            }

            try
            {
                var jsonData = JsonSerializer.Serialize(value, _jsonOptions);
                await _db.StringSetAsync(key, jsonData, expiry);
                _logger.LogDebug("Cached value for key: {Key} with expiry: {Expiry}", key, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set cache for key: {Key}", key);
                throw new CacheException($"Failed to set cache for key: {key}", ex);
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            ValidateKey(key);

            try
            {
                var data = await _db.StringGetAsync(key);
                if (data.IsNullOrEmpty)
                {
                    _logger.LogDebug("Cache miss for key: {Key}", key);
                    return default;
                }

                var result = JsonSerializer.Deserialize<T>(data!, _jsonOptions);
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cache for key: {Key}", key);
                return default;
            }
        }

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
        {
            ValidateKey(key);

            // Try to get cached value first
            var cached = await GetAsync<T>(key);
            if (cached != null && !IsDefaultValue(cached))
                return cached;

            // If cache miss, get from factory and set cache
            try
            {
                var value = await factory();
                if (value != null && !IsDefaultValue(value))
                {
                    await SetAsync(key, value, expiry);
                }
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get or set cache for key: {Key}", key);
                throw;
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            ValidateKey(key);

            try
            {
                bool result = await _db.KeyDeleteAsync(key);
                _logger.LogDebug("Key removal {Result} for key: {Key}",
                    result ? "successful" : "failed", key);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove key: {Key}", key);
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            ValidateKey(key);

            try
            {
                return await _db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check existence for key: {Key}", key);
                return false;
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException("Pattern cannot be null or empty", nameof(pattern));

            try
            {
                var server = GetServer();
                var keys = new List<RedisKey>();

                // Use SCAN instead of KEYS to avoid blocking
                await foreach (var key in ScanKeysAsync(server, pattern))
                {
                    keys.Add(key);
                }

                if (keys.Count > 0)
                {
                    // Delete in batches to avoid overwhelming Redis
                    const int batchSize = 100;
                    for (int i = 0; i < keys.Count; i += batchSize)
                    {
                        var batch = keys.Skip(i).Take(batchSize).ToArray();
                        await _db.KeyDeleteAsync(batch);
                    }

                    _logger.LogDebug("Removed {Count} keys matching pattern: {Pattern}", keys.Count, pattern);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove keys by pattern: {Pattern}", pattern);
                throw new CacheException($"Failed to remove keys by pattern: {pattern}", ex);
            }
        }

        private async IAsyncEnumerable<RedisKey> ScanKeysAsync(IServer server, string pattern)
        {
            if (server == null)
                throw new ArgumentNullException(nameof(server));

            await foreach (var key in server.KeysAsync(pattern: pattern + "*", pageSize: 100))
            {
                yield return key;
            }
        }

        private IServer GetServer()
        {
            try
            {
                var endpoint = _redis.GetEndPoints().FirstOrDefault()
                    ?? throw new InvalidOperationException("No Redis endpoints available");
                return _redis.GetServer(endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Redis server");
                throw new CacheException("Failed to get Redis server", ex);
            }
        }

        private static void ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        private static bool IsDefaultValue<T>(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default);
        }

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                _redis?.Dispose();
                _disposed = true;
                _logger.LogDebug("RedisCacheService disposed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RedisCacheService disposal");
            }
        }
    }
}