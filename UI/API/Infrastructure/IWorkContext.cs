using System.Collections.Generic;
using Core.Domain.Authentication;

namespace API.Infrastructure
{
    public interface IWorkContext
    {
        /// <summary>
        /// Current user, use for getting quickly in code
        /// </summary>
        User CurrentUser { get; }

        /// <summary>
        /// current user name
        /// </summary>
        string CurrentUsername { get; }

        /// <summary>
        /// the roles of current user.
        /// </summary>
        List<Role> CurrentRoles { get; }
    }
}
