using System;
using StackExchange.Redis;

namespace RedisExchangeApi.Web.Services;

public class RedisService
{
    private readonly string? _redisHost;
    private readonly string? _redisPort;
    private ConnectionMultiplexer? _redis;
    public IDatabase? db {get;set;}

    public RedisService(IConfiguration configuration)
    {
        _redisHost = configuration["RedisSettings:Host"];
        _redisPort = configuration["RedisSettings:Port"];
    }

    public async void Connect()
    {
        var configString=$"{_redisHost}:{_redisPort}";
         _redis=await ConnectionMultiplexer.ConnectAsync(configString);

    }

    public IDatabase GetDb(int db)
    {
        return _redis.GetDatabase(db);
    }

}
