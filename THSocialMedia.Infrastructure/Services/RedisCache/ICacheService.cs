namespace THSocialMedia.Infrastructure.Services.RedisCache
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool SetData<T>(string key, T value, DateTimeOffset exprirationTime);
        object RemoveDate(string key);
        bool RemoveKeysByPrefix(string prefix);

        // Generic fan-out helpers (sorted-set based)
        Task FanOutOnWriteAsync<T>(
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
            bool invalidateReadCache = true);

        Task<IReadOnlyList<T>> GetTimelineFanOutOnWriteAsync<T>(
            string entityKeyPrefix,
            string timelineKeyPrefix,
            Guid userId,
            int take = 50);

        Task<IReadOnlyList<T>> GetTimelineFanOutOnReadAsync<T>(
            string entityKeyPrefix,
            string timelineKeyPrefix,
            string readCacheKeyPrefix,
            Guid userId,
            Func<Task<IEnumerable<T>>> fetchFromSource,
            Func<T, Guid> getEntityId,
            Func<T, DateTimeOffset> getScoreTime,
            int take = 50,
            TimeSpan? readCacheTtl = null,
            TimeSpan? entityTtl = null);
    }
}
