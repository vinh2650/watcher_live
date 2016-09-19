using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Newtonsoft.Json.Serialization;

namespace API.Helpers
{
    public class CamelCasingFilterAttribute : ActionFilterAttribute
    {
        private static JsonMediaTypeFormatter _camelCasingFormatter = new JsonMediaTypeFormatter();

        static CamelCasingFilterAttribute()
        {
            _camelCasingFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            ObjectContent content = actionExecutedContext.Response.Content as ObjectContent;
            if (content != null)
            {
                if (content.Formatter is JsonMediaTypeFormatter)
                {
                    actionExecutedContext.Response.Content = new ObjectContent(content.ObjectType, content.Value, _camelCasingFormatter);
                }
            }
        }
    }
}