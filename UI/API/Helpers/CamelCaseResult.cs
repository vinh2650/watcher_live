using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace API.Helpers
{
    public static class CamelCaseResult
    {
        public static object Convert<TEntity>(TEntity data)
        {
            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return JsonConvert.DeserializeObject<object>(JsonConvert.SerializeObject(data, Formatting.Indented, jsonSerializerSettings));
        }
    }
}