using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Binary;

using StackExchange.Redis;

namespace RedisCacheClient
{
    internal static class DatabaseExtensions
    {
        internal static void Set(this IDatabase database, string key, object value)
        {
            database.StringSet(key, Serialize(value));
        }

        internal static object Get(this IDatabase database, string key)
        {
            return Deserialize(database.StringGet(key));
        }

        internal static object[] Get(this IDatabase database, string[] keys)
        {
            var result = database.StringGet(keys.Select(p => (RedisKey)p).ToArray());

            return result.Select(p => Deserialize(p)).ToArray();
        }

        internal static object GetSet(this IDatabase database, string key, object value)
        {
            return Deserialize(database.StringGetSet(key, Serialize(value)));
        }

        internal static void Expire(this IDatabase database, string key, CacheItemPolicy policy)
        {
            if (policy == null)
            {
                return;
            }

            if (policy.AbsoluteExpiration != ObjectCache.InfiniteAbsoluteExpiration)
            {
                database.KeyExpire(key, policy.AbsoluteExpiration.UtcDateTime);
            }
            else if (policy.SlidingExpiration != ObjectCache.NoSlidingExpiration)
            {
                database.KeyExpire(key, policy.SlidingExpiration);
            }
        }

        private static object Deserialize(byte[] value)
        {
            if (value == null)
            {
                return null;
            }

            var formatter = new BinaryFormatter();

            using (var stream = new MemoryStream(value))
            {
                return formatter.Deserialize(stream);
            }
        }

        private static byte[] Serialize(object value)
        {
            if (value == null)
            {
                return null;
            }

            var formatter = new BinaryFormatter();

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, value);

                return stream.ToArray();
            }
        }
    }
}