using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

using StackExchange.Redis;

namespace RedisCacheClient
{
    public class RedisCache : ObjectCache, IDisposable
    {
        static RedisCache()
        {
            SetDefaultSerializers(Serialize, Deserialize);
        }

        public RedisCache(string configuration)
            : this(DefaultDb, configuration)
        {
        }

        public RedisCache(int db, string configuration)
            : this(db, ConfigurationOptions.Parse(configuration))
        {
        }

        public RedisCache(ConfigurationOptions configuration)
            : this(DefaultDb, configuration)
        {
        }

        public RedisCache(int db, ConfigurationOptions configuration)
        {
            _db = db;
            _configuration = configuration;

            _serializer = _defaultSerializer;
            _deserializer = _defaultDeserializer;
        }

        private const int DefaultDb = 0;

        private readonly int _db;
        private readonly ConfigurationOptions _configuration;

        private int _disposed;
        private ConnectionMultiplexer _connection;

        private Func<object, byte[]> _serializer;
        private Func<byte[], object> _deserializer;

        internal ConnectionMultiplexer Connection
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

        private static Func<object, byte[]> _defaultSerializer;
        private static Func<byte[], object> _defaultDeserializer;

        public void SetSerializers(Func<object, byte[]> serializer, Func<byte[], object> deserializer)
        {
            _serializer = serializer;
            _deserializer = deserializer;
        }

        public static void SetDefaultSerializers(Func<object, byte[]> serializer, Func<byte[], object> deserializer)
        {
            _defaultSerializer = serializer;
            _defaultDeserializer = deserializer;
        }

        #region ObjectCache

        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            throw new NotSupportedException();
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotSupportedException();
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

            var database = Connection.GetDatabase(_db);

            return database.KeyExists(key);
        }

        public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            return AddOrGetExistingInternal(key, value, regionName, new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration });
        }

        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return new CacheItem(value.Key, AddOrGetExistingInternal(value.Key, value.Value, value.RegionName, policy), value.RegionName);
        }

        public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            return AddOrGetExistingInternal(key, value, regionName, policy);
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
            SetInternal(key, value, regionName, new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration });
        }

        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            SetInternal(item.Key, item.Value, item.RegionName, policy);
        }

        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            SetInternal(key, value, regionName, policy);
        }

        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            return GetInternal(keys.ToArray(), regionName);
        }

        public override object Remove(string key, string regionName = null)
        {
            var value = GetInternal(key, regionName);

            var database = Connection.GetDatabase(_db);

            database.KeyDelete(key);

            return value;
        }

        public override long GetCount(string regionName = null)
        {
            throw new NotSupportedException();
        }

        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get { return DefaultCacheCapabilities.OutOfProcessProvider | DefaultCacheCapabilities.AbsoluteExpirations | DefaultCacheCapabilities.SlidingExpirations; }
        }

        public override string Name
        {
            get { return "Default"; }
        }

        public override object this[string key]
        {
            get { return GetInternal(key, null); }
            set { SetInternal(key, value, null, null); }
        }

        #endregion

        #region IDisposable

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

        #endregion

        #region private

        private object AddOrGetExistingInternal(string key, object value, string regionName, CacheItemPolicy policy)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (regionName != null)
            {
                throw new NotSupportedException("regionName");
            }

            var database = Connection.GetDatabase(_db);

            var result = _deserializer(database.StringGetSet(key, _serializer(value)));

            database.Expire(key, policy);

            return result;
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

            var database = Connection.GetDatabase(_db);

            return _deserializer(database.StringGet(key));
        }

        private IDictionary<string, object> GetInternal(string[] keys, string regionName)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }

            if (regionName != null)
            {
                throw new NotSupportedException("regionName");
            }

            var database = Connection.GetDatabase(_db);

            var values = database.StringGet(keys.Select(p => (RedisKey)p).ToArray());

            return Enumerable.Range(0, keys.Length).ToDictionary(p => keys[p], p => _deserializer(values[p]));
        }

        private void SetInternal(string key, object value, string regionName, CacheItemPolicy policy)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (regionName != null)
            {
                throw new NotSupportedException("regionName");
            }

            var database = Connection.GetDatabase(_db);

            database.StringSet(key, _serializer(value));
            database.Expire(key, policy);
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
            var formatter = new BinaryFormatter();

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, value);

                return stream.ToArray();
            }
        }

        #endregion
    }
}