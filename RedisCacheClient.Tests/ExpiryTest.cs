using System;
using System.Threading;

using Xunit;

namespace RedisCacheClient.Tests
{
    public class ExpiryTest
    {
        public ExpiryTest()
        {
            _redisCache = new RedisCache(Environment.GetEnvironmentVariable("RedisConfiguration"));
        }

        private readonly RedisCache _redisCache;

        [Fact]
        public void AbsoluteLiving()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "value";

            _redisCache.Add(key, expected, DateTimeOffset.Now.AddSeconds(5));

            Thread.Sleep(2000);

            var actual = _redisCache.Get(key);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AbsoluteExpired()
        {
            var key = Guid.NewGuid().ToString();
            var value = "value";

            _redisCache.Add(key, value, DateTimeOffset.Now.AddSeconds(2));

            Thread.Sleep(4000);

            var actual = _redisCache.Get(key);

            Assert.Null(actual);
        }
    }
}
