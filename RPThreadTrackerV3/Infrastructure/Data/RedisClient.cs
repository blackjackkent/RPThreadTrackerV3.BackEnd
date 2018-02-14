namespace RPThreadTrackerV3.Infrastructure.Data
{
	using Interfaces.Data;
	using Microsoft.Extensions.Configuration;
	using Newtonsoft.Json;
	using StackExchange.Redis;

	public class RedisClient : IRedisClient
    {
	    private IConfiguration _config;
	    public ConnectionMultiplexer Connection { get; set; }

	    public RedisClient(IConfiguration config)
	    {
		    _config = config;
			Connection = ConnectionMultiplexer.Connect(config["Data:RedisConnectionString"]);
		}

	    public T Get<T>(string key)
	    {
		    var cache = Connection.GetDatabase();
		    var resultJson = cache.StringGet(key);
		    if (!resultJson.HasValue)
		    {
			    return default(T);
		    }
		    return JsonConvert.DeserializeObject<T>(resultJson.ToString());
	    }

	    public void Set<T>(string key, T value)
	    {
		    var valueJson = JsonConvert.SerializeObject(value);
		    var cache = Connection.GetDatabase();
		    cache.StringSet(key, valueJson);
	    }

	    public void Delete(string key)
	    {
		    var cache = Connection.GetDatabase();
		    cache.KeyDelete(key);
	    }
    }
}
