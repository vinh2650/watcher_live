using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace API.Results
{
    public class Error : IHttpActionResult
    {
        private readonly HttpStatusCode _statusCode;
        readonly object _responseData;
        public HttpRequestMessage Request { get; set; }

        public Error(ApiController controller, object responseData, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            this._statusCode = statusCode;

            if (responseData is string)
            {
                _responseData = new
                {
                    ErrorMessage = responseData
                };
            }
            else
            {
                this._responseData = responseData;
            }

            this.Request = controller.Request;
        }

        public Error(ApiController controller, Exception exception)
        {
            this._statusCode = HttpStatusCode.BadRequest;
            this.Request = controller.Request;
            this._responseData = exception;
        }

        public Error(ApiController controller, Exception exception, HttpStatusCode statusCode)
        {
            this._statusCode = statusCode;
            this.Request = controller.Request;
            this._responseData = exception;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = Request.CreateResponse(this._statusCode, this._responseData);
            return Task.FromResult(response);
        }
    }
}
