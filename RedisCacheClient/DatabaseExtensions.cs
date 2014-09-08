using System.Runtime.Caching;

using StackExchange.Redis;

namespace RedisCacheClient
{
    internal static class DatabaseExtensions
    {
        internal static void Expire(this IDatabase database, string key, CacheItemPolicy policy)
        {
            if (policy == null)
            {
                return;
            }

            if (policy.AbsoluteExpiration != ObjectCache.InfiniteAbsoluteExpiration)
            {
                database.KeyExpire(key, policy.AbsoluteExpiration.UtcDateTime, CommandFlags.FireAndForget);
            }
            else if (policy.SlidingExpiration != ObjectCache.NoSlidingExpiration)
            {
                database.KeyExpire(key, policy.SlidingExpiration, CommandFlags.FireAndForget);
            }
        }
    }
}