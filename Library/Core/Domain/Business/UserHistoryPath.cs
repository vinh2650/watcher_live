using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Authentication;

namespace Core.Domain.Business
{
    public class UserHistoryPath : BaseEntity
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// History Path
        /// </summary>
        public string HistoryPath { get; set; }

        public User User { get; set; }
    }
}
