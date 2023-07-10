using Microsoft.Extensions.Options;
using Multitenancy.Services.Options;
using StackExchange.Redis;
using IDatabase = StackExchange.Redis.IDatabase;
using IServer = StackExchange.Redis.IServer;

namespace Multitenancy.Services.Impl;

public class CacheConnector
{
    public IServer Server { get; }
    public IDatabase RedisDatabase { get; }
    public int RedisDatabaseNumber { get; }

    public CacheConnector(IConnectionMultiplexer connectionMultiplexer,
                        IOptions<RedisOptions> redisOptions)
    {
        RedisDatabaseNumber = redisOptions.Value.DatabaseNumber;
        RedisDatabase = connectionMultiplexer.GetDatabase(RedisDatabaseNumber);

        var connectionString = redisOptions.Value.ConnectionString;
        var hostAndPort = connectionString.IndexOf(",") == -1 ?
            connectionString :
            connectionString.Substring(0, connectionString.IndexOf(","));
        Server = connectionMultiplexer.GetServer(hostAndPort);
    }
}
