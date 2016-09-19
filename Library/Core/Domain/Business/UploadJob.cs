using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Authentication;

namespace Core.Domain.Business
{
    /// <summary>
    /// Represent class for manage different job per 1 operator of user
    /// </summary>
    public class UploadJob:BaseEntity
    {
        public string JobName { get; set; }

        /// <summary>
        /// user
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// user id
        /// </summary>
        public string UserId { get; set; }


        public Operator Operator { get; set; }

        public string OperatorId { get; set; }

        public JobStatus JobStatus { get; set; }

        /// <summary>
        /// color for display on map
        /// </summary>
        public string Band { get; set; }
    }

    public enum JobStatus
    {
        NotUpload = 0,
        Processing = 1,
        Ready = 2
    }
}
