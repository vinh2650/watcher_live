using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Authentication;
using Service.Interface.SearchModel;

namespace Service.Interface.Search
{
    public interface IUserSearchService
    {
        /// <summary>
        /// Index user to es
        /// </summary>
        /// <param name="user"></param>
        void IndexUser(User user);

        /// <summary>
        /// Search user by name keyword
        /// </summary>
        /// <param name="keywords">list of name keyword</param>
        /// <returns></returns>
        esResultModel<esUserData> SearchUserByKeyword(List<string> keywords);
    }
}
