using System;
using System.Configuration;
using System.Linq;
using StackExchange.Redis;

namespace Service.CachingLayer
{
    /// <summary>
    /// use azure redis for cache
    /// </summary>
    public class RedisCacheManager:ICacheManager
    {
        private static string redisurl = ConfigurationManager.AppSettings["AzureRedisEndpoint"];
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(redisurl);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        private readonly IDatabase cache = Connection.GetDatabase();


        public T Get<T>(string key)
        {
            return cache.Get<T>(key);
        }

        public void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;
            cache.Set(key, data);
            cache.KeyExpire(key, DateTime.Now + TimeSpan.FromMinutes(cacheTime));
        }

        public bool IsSet(string key)
        {
            return cache.KeyExists(key);
        }

        public void Remove(string key)
        {
            cache.KeyDelete(key);
        }

        public void Clear()
        {
            var endpoint = Connection.GetEndPoints();

            Connection.GetServer(endpoint.FirstOrDefault()).FlushDatabase();
        }

        public void RemoveByPattern(string pattern)
        {

            foreach (var ep in Connection.GetEndPoints())
            {
                var server = Connection.GetServer(ep);
                var keys = server.Keys(pattern: "*" + pattern + "*");
                foreach (var key in keys)
                    cache.KeyDelete(key);
            }

        }
    }
}
