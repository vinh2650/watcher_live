using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models.Business
{
    /// <summary>
    /// get role result
    /// </summary>
    public class RoleGetResult
    {
        /// <summary>
        /// id of role
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// system name of role
        /// </summary>
        public string Name { get; set; }
    }
}