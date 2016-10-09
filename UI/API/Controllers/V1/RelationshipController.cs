using System;
using System.Web.Http;
using API.Helpers;
using API.Models.Business;
using Common.Helpers;
using Core.Domain.Authentication;
using Core.Domain.Business;
using Microsoft.AspNet.Identity;
using Service.Interface.Authentication;
using Service.Interface.Business;
using Swashbuckle.Swagger.Annotations;

namespace API.Controllers.V1
{
    /// <summary>
    /// Relationship controller
    /// </summary>
    [RoutePrefix("api/v1/relationship")]
    public class RelationshipController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IRelationshipService _relationshipService;
        private readonly IRelationshipRequestService _relationshipRequestService;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="workContext"></param>
        /// <param name="roleService"></param>
        /// <param name="relationshipService"></param>
        /// <param name="relationshipRequestService"></param>
        public RelationshipController(
            IUserService userService,
            IRelationshipService relationshipService,
            IRelationshipRequestService relationshipRequestService)
        {
            _userService = userService;
            _relationshipService = relationshipService;
            _relationshipRequestService = relationshipRequestService;
        }

        /// <summary>
        /// Create new relationship, return partner Id
        /// </summary>
        /// <returns></returns>
        [Route("request/new")]
        [HttpPost]
        [SwaggerResponse(200, "", typeof(string))]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult CreateNewRelationship([FromBody] RelationshipModel model)
        {
            try
            {
                var currentUser = User.GetValueOfClaim(ClaimName.UseridKey);

                var findPartner = _userService.GetUserById(model.ToUserId);
                if (findPartner == null)
                    return Error("Cannot find partner");

                var request = new RelationshipRequest()
                {
                    FromUserId = currentUser,
                    ToUserId = model.ToUserId,
                    Type = model.Type
                };

                _relationshipRequestService.CreateRequest(request);

                return Created(model.ToUserId);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// Create new relationship, return partner Id
        /// </summary>
        /// <returns></returns>
        [Route("request/confirm/{Id}")]
        [HttpPost]
        [SwaggerResponse(200, "", typeof(string))]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult ConfirmNewRelationship([FromUri] string Id)
        {
            try
            {
                var findRequest = _relationshipRequestService.GetRequestById(Id);
                if (findRequest == null)
                {
                    return Error("Request not found");
                }

                var newRelationshipFrom = new Relationship()
                {
                    FromUserId = findRequest.FromUserId,
                    ToUserId = findRequest.ToUserId,
                    Type = findRequest.Type
                };

                var newRelationshipTo = new Relationship()
                {
                    ToUserId = findRequest.FromUserId,
                    FromUserId = findRequest.ToUserId,
                    Type = findRequest.Type
                };

                _relationshipService.CreateRelationship(newRelationshipFrom);
                _relationshipService.CreateRelationship(newRelationshipTo);
                _relationshipRequestService.RemoveRequest(findRequest);

                return Success("Succesfully create new relationship");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// Get all pending request
        /// </summary>
        /// <returns></returns>
        [Route("reuqest/pending")]
        [HttpGet]
        [SwaggerResponse(200, "")]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult GetAllPendingRequest()
        {
            try
            {
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);

                var res = _relationshipRequestService.GetAllPendingRequestOfUser(currentUserId);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// Get request by Id
        /// </summary>
        /// <returns></returns>
        [Route("reuqest/{id}")]
        [HttpGet]
        [SwaggerResponse(200, "")]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult GetPendingRequestById([FromUri] string id)
        {
            try
            {
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);

                var res = _relationshipRequestService.GetRequestById(id);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// Get all sent request
        /// </summary>
        /// <returns></returns>
        [Route("reuqest/sent")]
        [HttpGet]
        [SwaggerResponse(200, "")]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult GetAllSentRequest()
        {
            try
            {
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);

                var res = _relationshipRequestService.GetAllSentRequestIfUser(currentUserId);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// Delete a relationship
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpDelete]
        [SwaggerResponse(200, "")]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult DeleteRelationshipById([FromUri] string id)
        {
            try
            {
                var findRela = _relationshipService.GetRelationshipById(id);
                if (findRela == null)
                    return Error("Relationship not found");

                _relationshipService.DeleteRelationship(findRela);

                return Deleted();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }


        /// <summary>
        /// Delete relationship by partner Id
        /// </summary>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        [Route("{partnerId}")]
        [HttpDelete]
        [SwaggerResponse(200, "")]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult DeleteRelationshipByPartnerId([FromUri] string partnerId)
        {
            try
            {
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);

                var findRela = _relationshipService.GetRelationshipByPartnerId(currentUserId, partnerId);
                if (findRela == null)
                    return Error("Relationship not found");

                _relationshipService.DeleteRelationship(findRela);

                return Deleted();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// Get all of user relationship
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        [SwaggerResponse(200, "")]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult GetAllRelationship()
        {
            try
            {
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);

                var res = _relationshipService.GetAllRelationships(currentUserId);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// Get all of user relationship by Type
        /// </summary>
        /// <returns></returns>
        [Route("type/{type}")]
        [HttpGet]
        [SwaggerResponse(200, "")]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult GetAllRelationshipByType(Relationshiptype type)
        {
            try
            {
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);

                var res = _relationshipService.GetAllRelationshipsByType(currentUserId, type);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}