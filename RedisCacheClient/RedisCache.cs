using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;

using StackExchange.Redis;

namespace RedisCacheClient
{
    public class RedisCache : ObjectCache, IDisposable
    {
        public RedisCache(string connectionString)
        {
            _configuration = ConfigurationOptions.Parse(connectionString);
        }

        private readonly ConfigurationOptions _configuration;

        private int _disposed;
        private ConnectionMultiplexer _connection;

        private ConnectionMultiplexer Connection
        {
            get
            {
                if (_connection == null || !_connection.IsConnected)
                {
                    _connection = ConnectionMultiplexer.Connect(_configuration);
                }
                return _connection;
            }
        }

        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override bool Contains(string key, string regionName = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (regionName != null)
            {
                throw new NotSupportedException("regionName");
            }

            var database = Connection.GetDatabase();

            return database.KeyExists(key);
        }

        public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            return AddOrGetExistingInternal(key, value, regionName);
        }

        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return new CacheItem(value.Key, AddOrGetExistingInternal(value.Key, value.Value, value.RegionName), value.RegionName);
        }

        public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            return AddOrGetExistingInternal(key, value, regionName);
        }

        public override object Get(string key, string regionName = null)
        {
            return GetInternal(key, regionName);
        }

        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            return new CacheItem(key, GetInternal(key, regionName));
        }

        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            SetInternal(key, value, regionName);
        }

        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            SetInternal(item.Key, item.Value, item.RegionName);
        }

        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            SetInternal(key, value, regionName);
        }

        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            var database = Connection.GetDatabase();

            var arrayKeys = keys.ToArray();

            var result = database.Get(arrayKeys);

            var dic = new Dictionary<string, object>();

            for (int i = 0; i < arrayKeys.Length; i++)
            {
                dic.Add(arrayKeys[i], result[i]);
            }

            return dic;
        }

        public override object Remove(string key, string regionName = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            var value = GetInternal(key, regionName);

            var database = Connection.GetDatabase();

            database.KeyDelete(key);

            return value;
        }

        public override long GetCount(string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get { return DefaultCacheCapabilities.OutOfProcessProvider; }
        }

        public override string Name
        {
            get { return "Default"; }
        }

        public override object this[string key]
        {
            get { return GetInternal(key, null); }
            set { SetInternal(key, value, null); }
        }

        public bool IsDisposed
        {
            get { return _disposed == 1; }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            if (_connection != null)
            {
                _connection.Dispose();
            }
        }

        private object AddOrGetExistingInternal(string key, object value, string regionName)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (regionName != null)
            {
                throw new NotSupportedException("regionName");
            }

            var database = Connection.GetDatabase();

            return database.GetSet(key, value);
        }

        private object GetInternal(string key, string regionName)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (regionName != null)
            {
                throw new NotSupportedException("regionName");
            }

            var database = Connection.GetDatabase();

            return database.Get(key);
        }

        private void SetInternal(string key, object value, string regionName)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (regionName != null)
            {
                throw new NotSupportedException("regionName");
            }

            var database = Connection.GetDatabase();

            database.Set(key, value);
        }
    }
}
