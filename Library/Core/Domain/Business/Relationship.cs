using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Authentication;

namespace Core.Domain.Business
{
    public class Relationship : BaseEntity
    {
        /// <summary>
        /// UserId
        /// </summary>
        public string FromUserId { get; set; }

        /// <summary>
        /// User friend Id
        /// </summary>
        public string ToUserId { get; set; }

        /// <summary>
        /// Relation ship type
        /// </summary>
        public Relationshiptype Type { get; set; }

        public User User { get; set; }
    }

    public enum Relationshiptype
    {
        Friend = 1,
        Family = 0
    }
}
