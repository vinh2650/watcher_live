using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Core.Domain.Business;
using Service.Interface.Business;

namespace Service.Implement.Business
{
    public class RelationshipRequestService : IRelationshipRequestService
    {
        private readonly DbContext _context;
        private readonly DbSet<RelationshipRequest> _relationshipRequestsDb;

        public RelationshipRequestService(DbContext context)
        {
            _context = context;
            _relationshipRequestsDb = _context.Set<RelationshipRequest>();
        }

        public void CreateRequest(RelationshipRequest request)
        {
            try
            {
                _relationshipRequestsDb.Add(request);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public RelationshipRequest GetRequestById(string requestId)
        {
            try
            {
                var query = "SELECT * FROM RelationshipRequest WHERE Id = @p0";
                var res = _relationshipRequestsDb.SqlQuery(query, requestId).FirstOrDefault();
                return res;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public List<RelationshipRequest> GetAllPendingRequestOfUser(string userId)
        {
            var query = "SELECT * FROM RelationshipRequest WHERE ToUserId = @p0";
            var res = _relationshipRequestsDb.SqlQuery(query, userId).ToList();
            return res;
        }

        public List<RelationshipRequest> GetAllSentRequestIfUser(string userId)
        {
            var query = "SELECT * FROM RelationshipRequest WHERE FromUserId = @p0";
            var res = _relationshipRequestsDb.SqlQuery(query, userId).ToList();
            return res;
        }

        public void RemoveRequest(RelationshipRequest request)
        {
            try
            {
                _relationshipRequestsDb.Remove(request);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                
                throw;
            }
        }


    }
}
