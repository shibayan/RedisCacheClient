using StackExchange.Redis;

namespace RedisCacheClient
{
    internal static class ConnectionExtensions
    {
        internal static IServer GetServer(this ConnectionMultiplexer connection)
        {
            var endPoints = connection.GetEndPoints();

            return connection.GetServer(endPoints[0]);
        }
    }
}
