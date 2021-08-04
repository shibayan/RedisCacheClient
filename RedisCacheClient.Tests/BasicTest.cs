using System;
using System.Collections;
using System.Linq;
using System.Runtime.Caching;

using Xunit;

namespace RedisCacheClient.Tests
{
    public class BasicTest
    {
        public BasicTest()
        {
            _redisCache = new RedisCache(Environment.GetEnvironmentVariable("RedisConfiguration"));
        }

        private readonly RedisCache _redisCache;

        [Fact]
        public void Add()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "bar";

            _redisCache.Add(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = _redisCache.Get(key);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddOrGetExisting()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "bar";

            _redisCache.AddOrGetExisting(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = _redisCache.Get(key);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SetAndAddOrGetExisting()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "bar";
            var newValue = "baz";

            _redisCache.Set(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = _redisCache.AddOrGetExisting(key, newValue, ObjectCache.InfiniteAbsoluteExpiration);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Contains()
        {
            var key = Guid.NewGuid().ToString();
            var value = "bar";

            _redisCache.Set(key, value, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = _redisCache.Contains(key);

            Assert.True(actual);
        }

        [Fact]
        public void SetAndGet()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "bar";

            _redisCache.Set(key, expected, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = _redisCache.Get(key);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValues()
        {
            var keys = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid().ToString()).ToArray();
            var expected = new[] { "bar", "baz", "foo" };

            for (var i = 0; i < keys.Length; i++)
            {
                _redisCache.Set(keys[i], expected[i], ObjectCache.InfiniteAbsoluteExpiration);
            }

            var actual = _redisCache.GetValues(keys);

            Assert.Equal(expected, (ICollection)actual.Values);
        }

        [Fact]
        public void Remove()
        {
            var key = Guid.NewGuid().ToString();
            var value = "bar";

            _redisCache.Set(key, value, ObjectCache.InfiniteAbsoluteExpiration);

            _redisCache.Remove(key);

            var actual = _redisCache.Get(key);

            Assert.Null(actual);
        }

        [Fact]
        public void RemoveAndReturn()
        {
            var key = Guid.NewGuid().ToString();
            var excepted = "bar";

            _redisCache.Set(key, excepted, ObjectCache.InfiniteAbsoluteExpiration);

            var actual = _redisCache.Remove(key);

            Assert.Equal(excepted, actual);
        }

        [Fact]
        public void Indexer()
        {
            var key = Guid.NewGuid().ToString();
            var expected = "bar";

            _redisCache[key] = expected;

            var actual = _redisCache[key];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Empty()
        {
            var key = Guid.NewGuid().ToString();

            var actual = _redisCache.Get(key);

            Assert.Null(actual);
        }
    }
}
