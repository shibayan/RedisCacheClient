using System;
using System.Runtime.Caching;
using System.Threading;

using Xunit;

namespace RedisCacheClient.Tests
{
    public class ExpiryTest
    {
        [Fact]
        public void AbsoluteLiving()
        {
            var cache = CreateRedisCache();

            var key = "absolute";
            var expected = "value";

            cache.Add(key, expected, DateTimeOffset.Now.AddSeconds(3));

            Thread.Sleep(2000);

            var actual = cache.Get(key);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AbsoluteExpired()
        {
            var cache = CreateRedisCache();

            var key = "absolute";
            var value = "value";

            cache.Add(key, value, DateTimeOffset.Now.AddSeconds(3));

            Thread.Sleep(4000);

            var actual = cache.Get(key);

            Assert.Null(actual);
        }

        private static ObjectCache CreateRedisCache()
        {
            var cache = new RedisCache(Environment.GetEnvironmentVariable("RedisConfiguration"));

            foreach (var item in cache)
            {
                cache.Remove(item.Key);
            }

            return cache;
        }
    }
}
