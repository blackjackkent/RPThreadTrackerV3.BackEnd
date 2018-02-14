namespace RPThreadTrackerV3.Interfaces.Data
{
	public interface IRedisClient
	{
		T Get<T>(string key);
		void Set<T>(string key, T value);
		void Delete(string key);
	}
}
