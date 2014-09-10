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
        public void Add()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var expected = "bar";

            cache.Add(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.Get(key);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AddOrGetExisting()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var expected = "bar";

            cache.AddOrGetExisting(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.Get(key);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetAndAddOrGetExisting()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var expected = "bar";
            var newValue = "baz";

            cache.Set(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.AddOrGetExisting(key, newValue, ObjectCache.InfiniteAbsoluteExpiration);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Contains()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var value = "bar";

            cache.Set(key, value, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.Contains(key);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void SetAndGet()
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
        public void Remove()
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
        public void Indexer()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var expected = "bar";

            cache[key] = expected;

            var actual = cache[key];

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Empty()
        {
            var cache = CreateRedisCache();

            var key = "foobarbaz";

            var actual = cache.Get(key);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void GetCount()
        {
            var cache = CreateRedisCache();

            var expected = 3;

            for (int i = 0; i < expected; i++)
            {
                cache.Set("key" + i, "value" + i, ObjectCache.InfiniteAbsoluteExpiration);
            }

            var actual = cache.GetCount();

            Assert.AreEqual(expected, actual);
        }

        private ObjectCache CreateRedisCache()
        {
#if false
            var cache = MemoryCache.Default;
#else
            var cache = new RedisCache(ConfigurationManager.AppSettings["RedisConfiguration"]);
#endif

            foreach (var item in cache)
            {
                cache.Remove(item.Key);
            }

            return cache;
        }
    }
}
