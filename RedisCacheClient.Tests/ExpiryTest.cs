using System;
using System.Configuration;
using System.Runtime.Caching;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RedisCacheClient.Tests
{
    [TestClass]
    public class ExpiryTest
    {
        [TestMethod]
        public void AbsoluteLiving()
        {
            var cache = CreateRedisCache();

            var key = "absolute";
            var expected = "value";

            cache.Add(key, expected, DateTimeOffset.Now.AddSeconds(5));

            Thread.Sleep(4000);

            var actual = cache.Get(key);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AbsoluteExpired()
        {
            var cache = CreateRedisCache();

            var key = "absolute";
            var value = "value";

            cache.Add(key, value, DateTimeOffset.Now.AddSeconds(5));

            Thread.Sleep(6000);

            var actual = cache.Get(key);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void SlidingLiving()
        {
            var cache = CreateRedisCache();

            var key = "sliding";
            var expected = "value";

            cache.Add(key, expected, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(5) });

            Thread.Sleep(4000);

            var actual = cache.Get(key);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SlidingExpired()
        {
            var cache = CreateRedisCache();

            var key = "sliding";
            var value = "value";

            cache.Add(key, value, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(5) });

            Thread.Sleep(6000);

            var actual = cache.Get(key);

            Assert.IsNull(actual);
        }

        private ObjectCache CreateRedisCache()
        {
            return new RedisCache(ConfigurationManager.AppSettings["RedisConfiguration"]);
        }
    }
}
