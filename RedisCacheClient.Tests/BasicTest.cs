using System;
using System.Collections;
using System.Runtime.Caching;

using Xunit;

namespace RedisCacheClient.Tests
{
    public class BasicTest
    {
        [Fact]
        public void Add()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var expected = "bar";

            cache.Add(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.Get(key);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddOrGetExisting()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var expected = "bar";

            cache.AddOrGetExisting(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.Get(key);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SetAndAddOrGetExisting()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var expected = "bar";
            var newValue = "baz";

            cache.Set(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.AddOrGetExisting(key, newValue, ObjectCache.InfiniteAbsoluteExpiration);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Contains()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var value = "bar";

            cache.Set(key, value, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.Contains(key);

            Assert.True(actual);
        }

        [Fact]
        public void SetAndGet()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var expected = "bar";

            cache.Set(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.Get(key);

            Assert.Equal(expected, actual);
        }

        [Fact]
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

            Assert.Equal(expected, (ICollection)actual.Values);
        }

        [Fact]
        public void Remove()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var value = "bar";

            cache.Set(key, value, ObjectCache.InfiniteAbsoluteExpiration);

            cache.Remove(key);

            var actual = cache.Get(key);

            Assert.Null(actual);
        }

        [Fact]
        public void RemoveAndReturn()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var excepted = "bar";

            cache.Set(key, excepted, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = cache.Remove(key);

            Assert.Equal(excepted, actual);
        }

        [Fact]
        public void Indexer()
        {
            var cache = CreateRedisCache();

            var key = "foo";
            var expected = "bar";

            cache[key] = expected;

            var actual = cache[key];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Empty()
        {
            var cache = CreateRedisCache();

            var key = "foobarbaz";

            var actual = cache.Get(key);

            Assert.Null(actual);
        }

        [Fact]
        public void GetCount()
        {
            var cache = CreateRedisCache();

            var expected = 3;

            for (int i = 0; i < expected; i++)
            {
                cache.Set("key" + i, "value" + i, ObjectCache.InfiniteAbsoluteExpiration);
            }

            var actual = cache.GetCount();

            Assert.Equal(expected, actual);
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
