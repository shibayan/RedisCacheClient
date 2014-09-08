RedisCacheClient
================

RedisCache is a compatible client and ObjectCache added in .NET 4

## Usage

```
var cache = new RedisCache("localhost");

cache.Set("foo", "bar", ObjectCache.InfiniteAbsoluteExpiration);

cache.Set("baz", 123, DateTimeOffset.Now.AddSeconds(10));

var results = cache.GetValues(new[] { "foo", "baz" });

foreach (var item in results)
{
    Console.WriteLine("{0} - {1}", item.Key, item.Value);
}
```