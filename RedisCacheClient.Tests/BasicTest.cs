using System.Collections;
using System.Configuration;
using System.Runtime.Caching;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RedisCacheClient.Tests
{
    [TestClass]
    public class BasicTest
    {
        [TestMethod]
        public void SetGet()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var expected = "bar";

            cache.Set(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.Get(key);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetValues()
        {
            var cache = CreateRedisCache();

            var keys = new[] { "foo", "bar", "baz" };
            var expected = new[] { "bar", "baz", "foo" };

            for (int i = 0; i < keys.Length; i++)
            {
                cache.Set(keys[i], expected[i], ObjectCache.InfiniteAbsoluteExpiration);
            }
            
            var actual = cache.GetValues(keys);

            CollectionAssert.AreEqual(expected, (ICollection)actual.Values);
        }

        [TestMethod]
        public void RemoveAndReturn()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var excepted = "bar";

            cache.Set(key, excepted, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.Remove(key);

            Assert.AreEqual(excepted, actual);
        }

        [TestMethod]
        public void RemoveKey()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var value = "bar";

            cache.Set(key, value, ObjectCache.InfiniteAbsoluteExpiration);

            cache.Remove(key);

            var actual = cache.Get(key);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void IndexerGetSet()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var expected = "bar";

            cache[key] = expected;

            var actual = cache[key];

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EmptyGet()
        {
            var cache = CreateRedisCache();

            var key = "foobarbaz";

            var actual = cache.Get(key);

            Assert.IsNull(actual);
        }

        private ObjectCache CreateRedisCache()
        {
            return new RedisCache(ConfigurationManager.AppSettings["RedisConfiguration"]);
        }
    }
}
