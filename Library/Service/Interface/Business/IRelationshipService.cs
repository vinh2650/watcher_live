using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Authentication;
using Core.Domain.Business;

namespace Service.Interface.Business
{
    public interface IRelationshipService
    {

        /// <summary>
        /// Create new relationship
        /// </summary>
        void CreateRelationship(Relationship relationship);

        /// <summary>
        /// Delete a relationship
        /// </summary>
        /// <param name="relationship"></param>
        void DeleteRelationship(Relationship relationship);

        /// <summary>
        /// Get a relationship by Id
        /// </summary>
        /// <param name="relationshipId"></param>
        Relationship GetRelationshipById(string relationshipId);

        /// <summary>
        /// Ger relationship by partner Id
        /// </summary>
        /// <param name="fromUserId"></param>
        /// <param name="toUserId"></param>
        /// <returns></returns>
        Relationship GetRelationshipByPartnerId(string fromUserId, string toUserId);

        /// <summary>
        /// Get all relationship
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<Relationship> GetAllRelationships(string userId);

        /// <summary>
        /// Get all relationship
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<Relationship> GetAllRelationshipsByType(string userId, Relationshiptype type);

        /// <summary>
        /// Get list of relationships partners by type
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<User> GetRelationshipPartnersByType(string userId, string type);

        /// <summary>
        /// Get all relationships partners
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<User> GetAllRelationshipPartners(string userId);
    }
}
