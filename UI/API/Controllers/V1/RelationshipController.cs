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
        /// create new relationship request
        /// </summary>
        /// <param name="model">create relationship request</param>
        /// <returns></returns>
        [Route("request/new")]
        [HttpPost]
        [SwaggerResponse(200, "")]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult CreateNewRelationship([FromBody] RelationshipModel model)
        {
            try
            {
                //Get current suer id
                var currentUser = User.GetValueOfClaim(ClaimName.UseridKey);

                //check targe user is valid 
                //user cannot make relation with him/herself
                //also can not make relation with same user twice
                if (_relationshipService.CheckToUser(currentUser, model.ToUserId))
                {
                    //Get to target user data
                    var findPartner = _userService.GetUserById(model.ToUserId);
                    if (findPartner == null)
                        return Error("Cannot find partner");

                    //prepair relaionship request
                    var request = new RelationshipRequest()
                    {
                        FromUserId = currentUser,
                        ToUserId = model.ToUserId,
                        Type = model.Type
                    };

                    //save new relation ship to sql db
                    _relationshipRequestService.CreateRequest(request);

                    return Created(model.ToUserId);
                }
                return Error("Taget user a not valid");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// confirm request to create a new relationship
        /// only when target user ( user receive request ) cofirm request, relation will be created
        /// </summary>
        /// <param name="requestId">id of request</param>
        /// <returns></returns>
        [Route("request/confirm/{requestId}")]
        [HttpPost]
        [SwaggerResponse(200, "", typeof(string))]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult ConfirmNewRelationship([FromUri] string requestId)
        {
            try
            {
                //find relation ship request by Id
                var findRequest = _relationshipRequestService.GetRequestById(requestId);
                if (findRequest == null)
                    return Error("Request not found");

                //create relationship
                //one for current user
                var newRelationshipFrom = new Relationship()
                {
                    FromUserId = findRequest.FromUserId,
                    ToUserId = findRequest.ToUserId,
                    Type = findRequest.Type
                };
                //one for target user
                var newRelationshipTo = new Relationship()
                {
                    ToUserId = findRequest.FromUserId,
                    FromUserId = findRequest.ToUserId,
                    Type = findRequest.Type
                };

                //save relation to sql data
                _relationshipService.CreateRelationship(newRelationshipFrom);
                _relationshipService.CreateRelationship(newRelationshipTo);
                //remvoe relationship request
                _relationshipRequestService.RemoveRequest(findRequest);

                return Success("Succesfully create new relationship");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// get all pending request that have been send to current user
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
                //get current userId
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);
                //get all pending request
                var res = _relationshipRequestService.GetAllPendingRequestOfUser(currentUserId);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// get request by id
        /// </summary>
        /// <param name="id">id of request</param>
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
                //todo install some code to secure
                //todo current user can only request belong to him/her
                //get request by id
                var res = _relationshipRequestService.GetRequestById(id);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// get all all sent request, sent by current user
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
                //get current userId
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);
                //get all sent request
                var res = _relationshipRequestService.GetAllSentRequestIfUser(currentUserId);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// remove a request
        /// </summary>
        /// <param name="requestId">request id</param>
        /// <returns></returns>
        [Route("{requestId}")]
        [HttpDelete]
        [SwaggerResponse(200, "")]
        [SwaggerResponse(401, "Unauthorize")]
        [SwaggerResponse(500, "Internal Error")]
        [SwaggerResponse(404, "Not Found")]
        public IHttpActionResult DeleteRelationshipById([FromUri] string requestId)
        {
            try
            {
                //find request by Id
                var findRela = _relationshipService.GetRelationshipById(requestId);
                if (findRela == null)
                    return Error("Relationship not found");
                //remove request
                _relationshipService.DeleteRelationship(findRela);

                return Deleted();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }


        /// <summary>
        /// remove relationship by partner Id
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
                //get current user id
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);
                //find relation between current user nad parner
                var findRela = _relationshipService.GetRelationshipByPartnerId(currentUserId, partnerId);
                if (findRela == null)
                    return Error("Relationship not found");
                //remove the relationship
                _relationshipService.DeleteRelationship(findRela);

                return Deleted();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// get all relationship of current user
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
                //get currentuser id
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);
                //get all relation of currentuser
                var res = _relationshipService.GetAllRelationships(currentUserId);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// get all relation of input type
        /// </summary>
        /// <param name="type"></param>
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
                //get current user id
                var currentUserId = User.GetValueOfClaim(ClaimName.UseridKey);
                //get all relationship by type
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