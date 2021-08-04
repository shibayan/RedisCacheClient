# RedisCacheClient

![Build](https://github.com/shibayan/RedisCacheClient/workflows/Build/badge.svg)
[![License](https://img.shields.io/github/license/shibayan/RedisCacheClient)](https://github.com/shibayan/RedisCacheClient/blob/master/LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/RedisCacheClient)](https://www.nuget.org/packages/RedisCacheClient/)

RedisCacheClient is a compatible ObjectCache client for .NET Standard

## NuGet Package

Package Name | Target Framework | NuGet
---|---|---
RedisCacheClient | .NET Standard 2.0 | [![NuGet](https://img.shields.io/nuget/v/RedisCacheClient)](https://www.nuget.org/packages/RedisCacheClient/)

## Install

```
Install-Package RedisCacheClient
```

```
dotnet add package RedisCacheClient
```

## Usage

```csharp
var cache = new RedisCache("localhost");

cache.Set("foo", "bar", ObjectCache.InfiniteAbsoluteExpiration);

cache.Set("baz", 123, DateTimeOffset.Now.AddSeconds(10));

var results = cache.GetValues(new[] { "foo", "baz" });

foreach (var item in results)
{
    Console.WriteLine("{0} - {1}", item.Key, item.Value);
}
```

## License

This project is licensed under the [MIT License](https://github.com/shibayan/RedisCacheClient/blob/master/LICENSE)
