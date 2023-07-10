using StackExchange.Redis;

namespace Multitenancy.Services.Constants;

public static class RedisScripts
{
    public static LuaScript SlidingRateLimiterScript => LuaScript.Prepare(SlidingRateLimiter);
    private const string SlidingRateLimiter = @"
            local current_time = redis.call('TIME')
            local trim_time = tonumber(current_time[1]) - @window
            redis.call('ZREMRANGEBYSCORE', @key, 0, trim_time)
            local request_count = redis.call('ZCARD',@key)

            if request_count < tonumber(@max_requests) then
                redis.call('ZADD', @key, current_time[1], current_time[1] .. current_time[2])
                redis.call('EXPIRE', @key, @window)
                return 0
            end
            return 1
            ";
    public static LuaScript SlidingRequestLimiterScript => LuaScript.Prepare(SlidingRequestLimiter);
    private const string SlidingRequestLimiter = @"
        local request_count = redis.call('GET', @key)
        local max_usage_count = redis.call('GET', @maxUsageKey)
        if @allowScale == '1' or 
           redis.call('EXISTS', @key) == 0 or 
           redis.call('EXISTS', @maxUsageKey) == 0 or 
           (tonumber(request_count) + tonumber(max_usage_count)) < tonumber(@maxCount)  then
             return 0
        end
        return 1
        ";

    public static LuaScript SlidingIncrementScript => LuaScript.Prepare(SlidingIncrement);
    private const string SlidingIncrement = @"
        redis.call('INCR', @key)
        ";

    public static LuaScript SlidingDecrementScript => LuaScript.Prepare(SlidingDecrement);
    private const string SlidingDecrement = @"
        redis.call('decrby', @key, @count)
        ";

    public static LuaScript HashSetValueIncrementScript => LuaScript.Prepare(HashSetValueIncrement);
    private const string HashSetValueIncrement = @"
        redis.call('hincrby', @hashsetName, @key ,@incrValue)
        ";
}
