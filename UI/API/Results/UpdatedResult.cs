using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Mvc;
using Newtonsoft.Json.Serialization;

namespace API.Results
{
    public class Updated: IHttpActionResult
    {
        private readonly HttpStatusCode _statusCode;
        readonly object _responseData;
        public HttpRequestMessage Request { get; set; }

        public Updated(ApiController controller, object responseData)
        {
            this._statusCode = HttpStatusCode.Created;
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

        public Updated(ApiController controller)
        {
            this._statusCode = HttpStatusCode.NoContent;
            this.Request = controller.Request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = Request.CreateResponse(this._statusCode, this._responseData);
            return Task.FromResult(response);
        }
    }
}
