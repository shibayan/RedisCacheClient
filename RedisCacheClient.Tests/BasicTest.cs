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
            var cache = new RedisCache(ConfigurationManager.AppSettings["RedisConfiguration"]);

            var key = "foo";
            var expected = "bar";

            cache.Set(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.Get(key);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetValues()
        {
            var cache = new RedisCache(ConfigurationManager.AppSettings["RedisConfiguration"]);

            var keys = new[] { "foo", "bar", "baz" };
            var expected = new[] { "bar", "baz", "foo" };

            for (int i = 0; i < keys.Length; i++)
            {
                cache.Set(keys[i], expected[i], ObjectCache.InfiniteAbsoluteExpiration);
            }
            
            var actual = cache.GetValues(keys);

            CollectionAssert.AreEqual(expected, (ICollection)actual.Values);
        }
    }
}
