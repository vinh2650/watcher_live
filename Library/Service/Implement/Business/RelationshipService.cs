using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Core.Domain.Authentication;
using Core.Domain.Business;
using Service.Interface.Authentication;
using Service.Interface.Business;

namespace Service.Implement.Business
{
    public class RelationshipService : IRelationshipService
    {
        private readonly DbContext _context;
        private readonly DbSet<Relationship> _relationshipsDb;
        private readonly IUserService _userService;

        public RelationshipService(DbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
            _relationshipsDb = context.Set<Relationship>();
        }

        public void CreateRelationship(Relationship relationship)
        {
            _relationshipsDb.Add(relationship);
            _context.SaveChanges();
        }

        public void DeleteRelationship(Relationship relationship)
        {
            _relationshipsDb.Remove(relationship);
            _context.SaveChanges();
        }

        public List<Relationship> GetAllRelationships(string userId)
        {
            try
            {
                var query = "SELECT * FROM Relationship WHERE FromUserId = @p0";
                var res = _relationshipsDb.SqlQuery(query, userId).ToList();
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Relationship> GetAllRelationshipsByType(string userId, Relationshiptype type)
        {
            try
            {
                var query = "SELECT * FROM Relationship WHERE FromUserId = @p0 AND Type = @p1";
                var res = _context.Database.SqlQuery<Relationship>(query, userId, type).ToList();
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<User> GetAllRelationshipPartners(string userId)
        {
            try
            {
                var res = _context.Database.SqlQuery<User>("EXEC Relationship_GetAllRelarionshipPartner @p0, @p1", userId, null).ToList();
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<User> GetRelationshipPartnersByType(string userId, string type)
        {
            try
            {
                var res = _context.Database.SqlQuery<User>("EXEC Relationship_GetAllRelarionshipPartner @p0, @p1", userId, type).ToList();
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Relationship GetRelationshipById(string relationshipId)
        {
            try
            {
                var query = "SELECT * FROM Relationship WHERE Id = @p0";
                var res = _relationshipsDb.SqlQuery(query, relationshipId).FirstOrDefault();
                return res;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public Relationship GetRelationshipByPartnerId(string fromUserId, string toUserId)
        {
            try
            {
                var query = "SELECT * FROM Relationship WHERE FromUserId = @p0 AND ToUserId = @p1";
                var res = _relationshipsDb.SqlQuery(query, fromUserId, toUserId).FirstOrDefault();
                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool CheckToUser(string fromUserId, string toUserId)
        {
            try
            {
                if (fromUserId == toUserId)
                    return false;

                var query = "SELECT * FROM Relationship WHERE FromUserId = @p0 AND ToUserId = @p1";
                var res = _relationshipsDb.SqlQuery(query, fromUserId, toUserId).FirstOrDefault();
                
                if(res!=null)
                    return false;

                return true;
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
