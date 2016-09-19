using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Mvc;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Swagger;

namespace API.Results
{
    /// <summary>
    /// 
    /// </summary>
    public class Success: IHttpActionResult
    {
        private readonly HttpStatusCode _statusCode;
        readonly object _responseData;
        /// <summary>
        /// 
        /// </summary>
        public HttpRequestMessage Request { get; set; }

        public Success(ApiController controller, object responseData, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            this._statusCode = statusCode;

            if (responseData is string)
            {
                _responseData = new
                {
                    Message = responseData
                };
            }
            else
            {
                this._responseData = responseData;
            }
            
            this.Request = controller.Request;
        }
        
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = Request.CreateResponse(this._statusCode, this._responseData, JsonMediaTypeFormatter.DefaultMediaType);
            return Task.FromResult(response);
        }
    }
}
