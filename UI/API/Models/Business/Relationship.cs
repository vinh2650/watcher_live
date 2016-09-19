using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Core.Domain.Business;

namespace API.Models.Business
{
    /// <summary>
    /// Model of Reletionship
    /// </summary>
    public class RelationshipModel
    {
        /// <summary>
        /// To user id
        /// </summary>
        [Required]
        public string ToUserId { get; set; }

        /// <summary>
        /// Type of the relationship
        /// 0 - Family
        /// 1 - Friend
        /// </summary>
        [Required]
        public Relationshiptype Type { get; set; }
    }
}