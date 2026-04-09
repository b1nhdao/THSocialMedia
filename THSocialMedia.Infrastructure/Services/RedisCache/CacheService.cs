using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace THSocialMedia.Infrastructure.Services.RedisCache
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cacheDb;
        private readonly ILogger<CacheService> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly IConfiguration _configuration;

        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public CacheService(IConnectionMultiplexer redis, ILogger<CacheService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _redis = redis;
            _cacheDb = _redis.GetDatabase();
            _configuration = configuration;
        }

        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value!, _jsonOptions)!;
            }
            return default!;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset exprirationTime)
        {
            var expriTime = exprirationTime - DateTimeOffset.UtcNow;
            var isSEt = _cacheDb.StringSet(key, JsonSerializer.Serialize(value, _jsonOptions), expriTime);
            return isSEt;
        }

        public object RemoveDate(string key)
        {
            var _exist = _cacheDb.KeyExists(key);
            if (_exist)
                return _cacheDb.KeyDelete(key);

            return false;
        }

        public bool RemoveKeysByPrefix(string prefix)
        {
            try
            {
                if (string.IsNullOrEmpty(prefix))
                    throw new ArgumentNullException(nameof(prefix), "Prefix cannot be null or empty.");

                var endpoints = _redis.GetEndPoints();
                var server = _redis.GetServer(endpoints.FirstOrDefault());

                if (server == null)
                    throw new InvalidOperationException("Redis server không khả dụng.");

                var keys = server.Keys(database: _cacheDb.Database, pattern: $"{prefix}*").ToArray();

                if (keys.Any())
                {
                    _cacheDb.KeyDelete(keys);
                    return true;
                }

            }
            catch (Exception)
            {
            }

            return false;
        }

        // ----------------------
        // Generic fan-out caching
        // ----------------------

        private static string EntityKey(string entityKeyPrefix, Guid entityId) => $"{entityKeyPrefix}:{entityId:N}";
        private static string TimelineKey(string timelineKeyPrefix, Guid userId) => $"{timelineKeyPrefix}:{userId:N}";
        private static string ReadCacheKey(string readCacheKeyPrefix, Guid userId) => $"{readCacheKeyPrefix}:{userId:N}";

        public async Task FanOutOnWriteAsync<T>(
            string entityKeyPrefix,
            string timelineKeyPrefix,
            string readCacheKeyPrefix,
            Guid actorId,
            T entity,
            Func<T, Guid> getEntityId,
            Func<T, DateTimeOffset> getScoreTime,
            IEnumerable<Guid> recipientIds,
            TimeSpan? entityTtl = null,
            bool includeActor = true,
            bool invalidateReadCache = true)
        {
            var ttl = entityTtl ?? TimeSpan.FromHours(6);

            var entityId = getEntityId(entity);
            if (entityId == Guid.Empty)
                throw new ArgumentException("Entity id cannot be empty.", nameof(getEntityId));

            // Store entity payload
            await _cacheDb.StringSetAsync(
                EntityKey(entityKeyPrefix, entityId),
                JsonSerializer.Serialize(entity, _jsonOptions),
                expiry: (Expiration)ttl);

            // Fan-out by adding entityId to recipient timelines
            var targets = (recipientIds ?? Array.Empty<Guid>()).Distinct().ToList();
            if (includeActor && !targets.Contains(actorId))
                targets.Add(actorId);

            var score = getScoreTime(entity).ToUnixTimeMilliseconds();

            var batch = _cacheDb.CreateBatch();
            var tasks = new List<Task>(capacity: targets.Count * (invalidateReadCache ? 2 : 1));

            foreach (var userId in targets)
            {
                tasks.Add(batch.SortedSetAddAsync(TimelineKey(timelineKeyPrefix, userId), entityId.ToString("N"), score));

                if (invalidateReadCache)
                    tasks.Add(batch.KeyDeleteAsync(ReadCacheKey(readCacheKeyPrefix, userId)));
            }

            batch.Execute();
            await Task.WhenAll(tasks);
        }

        public async Task<IReadOnlyList<T>> GetTimelineFanOutOnWriteAsync<T>(
            string entityKeyPrefix,
            string timelineKeyPrefix,
            Guid userId,
            int take = 50)
        {
            var ids = await _cacheDb.SortedSetRangeByRankAsync(TimelineKey(timelineKeyPrefix, userId), 0, take - 1, Order.Descending);
            if (ids.Length == 0)
                return [];

            var keys = ids
                .Select(v => (RedisKey)EntityKey(entityKeyPrefix, Guid.ParseExact(v!, "N")))
                .ToArray();

            var values = await _cacheDb.StringGetAsync(keys);

            var result = new List<T>(values.Length);
            foreach (var val in values)
            {
                if (val.IsNullOrEmpty) continue;
                var obj = JsonSerializer.Deserialize<T>(val!, _jsonOptions);
                if (obj is not null) result.Add(obj);
            }

            return result;
        }

        public async Task<IReadOnlyList<T>> GetTimelineFanOutOnReadAsync<T>(
            string entityKeyPrefix,
            string timelineKeyPrefix,
            string readCacheKeyPrefix,
            Guid userId,
            Func<Task<IEnumerable<T>>> fetchFromSource,
            Func<T, Guid> getEntityId,
            Func<T, DateTimeOffset> getScoreTime,
            int take = 50,
            TimeSpan? readCacheTtl = null,
            TimeSpan? entityTtl = null)
        {
            var effectiveReadTtl = readCacheTtl ?? TimeSpan.FromMinutes(2);
            var effectiveEntityTtl = entityTtl ?? TimeSpan.FromHours(6);

            // 1) read-cache hit
            var cached = await _cacheDb.StringGetAsync(ReadCacheKey(readCacheKeyPrefix, userId));
            if (!cached.IsNullOrEmpty)
            {
                var des = JsonSerializer.Deserialize<List<T>>(cached!, _jsonOptions);
                return des ?? [];
            }

            // 2) miss -> fetch, cache list + per-entity payload + index
            var entities = (await fetchFromSource()).Take(take).ToList();

            var batch = _cacheDb.CreateBatch();
            var tasks = new List<Task>(capacity: entities.Count * 2 + 1);

            tasks.Add(batch.StringSetAsync(ReadCacheKey(readCacheKeyPrefix, userId), JsonSerializer.Serialize(entities, _jsonOptions), expiry: (Expiration)effectiveReadTtl));

            foreach (var e in entities)
            {
                var entityId = getEntityId(e);
                if (entityId == Guid.Empty) continue;

                tasks.Add(batch.StringSetAsync(EntityKey(entityKeyPrefix, entityId), JsonSerializer.Serialize(e, _jsonOptions), expiry: (Expiration)effectiveEntityTtl));
                tasks.Add(batch.SortedSetAddAsync(TimelineKey(timelineKeyPrefix, userId), entityId.ToString("N"), getScoreTime(e).ToUnixTimeMilliseconds()));
            }

            batch.Execute();
            await Task.WhenAll(tasks);

            return entities;
        }
    }
}
