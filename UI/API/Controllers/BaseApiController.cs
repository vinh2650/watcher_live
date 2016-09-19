using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using API.Helpers;
using API.Results;
using System.Reflection;
using API.Filters;

namespace API.Controllers
{
    /// <summary>
    /// base api 
    /// </summary>
    [Authorize]
    [ValidateRefreshTokenEverywhere]
    public class BaseApiController : ApiController
    {
        #region constant

        /// <summary>
        /// error 404
        /// </summary>
        public const string PageNotFound = "Page not found";

        /// <summary>
        /// error 401
        /// </summary>
        public const string PermissionDontHave = "Page not found";

        /// <summary>
        /// error 500
        /// </summary>
        public const string ServerError = "Internal Server Error";

        /// <summary>
        /// 204
        /// </summary>
        public const string NoContent = "No content";

        /// <summary>
        /// 200
        /// </summary>
        public const string ResponseSuccesss = "Ok";

        /// <summary>
        /// 201
        /// </summary>
        public const string CreateSuccess = "Created";

        #endregion

        /// <summary>
        /// return success result
        /// </summary>
        /// <param name="data"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IHttpActionResult Success(object data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new Success(this, data, statusCode);
        }

        /// <summary>
        /// return created result
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        protected IHttpActionResult Created(object data)
        {
            return new Created(this, data);
        }

        /// <summary>
        /// return updated result
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IHttpActionResult Updated(object data)
        {
            return new Updated(this, data);
        }

        /// <summary>
        /// return updated result
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult Updated()
        {
            return new Updated(this);
        }

        /// <summary>
        /// return deleted result
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IHttpActionResult Deleted()
        {
            return new Deleted(this);
        }

        /// <summary>
        /// return error result
        /// </summary>
        /// <param name="data"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IHttpActionResult Error(object data, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new Error(this, data, statusCode);
        }

        /// <summary>
        /// return error result
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IHttpActionResult Error(Exception exception)
        {
            return new Error(this, exception);
        }

        /// <summary>
        /// return error result
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected IHttpActionResult Error(Exception exception, HttpStatusCode statusCode)
        {
            return new Error(this, exception, statusCode);
        }

        /// <summary>
        /// validate model state
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public List<string> ValidateModel(System.Web.Http.ModelBinding.ModelStateDictionary modelState)
        {
            var errorMessages = new List<string>();

            if (ModelState.IsValid)
                return errorMessages;

            foreach (var state in modelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    errorMessages.Add(error.ErrorMessage);
                }
            }

            return errorMessages;
        }

        /// <summary>
        /// Prepair success result
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public IHttpActionResult SuccesResult(string message)
        {
            var dataResult = new
            {
                Status = true,
                Message = message
            };

            return Json(CamelCaseResult.Convert(dataResult));
        }

        /// <summary>
        /// prepare success result
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        public IHttpActionResult SuccesResult<TEntity>(TEntity data, string message)
        {
            var dataResult = new
            {
                Status = true,
                Message = message,
                Data = data
            };

            return Json(CamelCaseResult.Convert(dataResult));
        }
    }
}