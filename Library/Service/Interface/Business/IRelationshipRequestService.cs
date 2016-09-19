using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Business;

namespace Service.Interface.Business
{
    public interface IRelationshipRequestService
    {
        /// <summary>
        /// Create request
        /// </summary>
        void CreateRequest(RelationshipRequest request);

        /// <summary>
        /// Get request by Id
        /// </summary>
        RelationshipRequest GetRequestById(string requestId);

        /// <summary>
        /// Get all pending request of user
        /// </summary>
        List<RelationshipRequest> GetAllPendingRequestOfUser(string userId);

        /// <summary>
        /// Get all sent request of user
        /// </summary>
        List<RelationshipRequest> GetAllSentRequestIfUser(string userId);

        /// <summary>
        /// Remove request by Id
        /// </summary>
        void RemoveRequest(RelationshipRequest request);
    }
}
