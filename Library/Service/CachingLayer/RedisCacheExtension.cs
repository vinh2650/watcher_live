using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Service.CachingLayer
{
    public static class RedisCacheExtension
    {
        private static readonly JsonSerializerSettings _jsonSettings;

        static RedisCacheExtension()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented, // for readability, change to None for compactness
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            _jsonSettings.Converters.Add(new StringEnumConverter());
        }
        public static T Get<T>(this IDatabase cache, string key)
        {
            return Deserialize<T>(cache.StringGet(key));
        }

        public static object Get(this IDatabase cache, string key)
        {
            return Deserialize<object>(cache.StringGet(key));
        }

        public static void Set(this IDatabase cache, string key, object value)
        {
            cache.StringSet(key, Serialize(value));
        }

        private static string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o, _jsonSettings);
        }

        private static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
        }
    }

}