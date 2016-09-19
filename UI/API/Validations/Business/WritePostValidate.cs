using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API.Models.Business;
using FluentValidation;

namespace API.Validations.Business
{
    /// <summary>
    /// Validate Write Notification Validate
    /// </summary>
    public class WriteNotificationValidate : AbstractValidator<WriteNotificationModel>
    {
        /// <summary>
        ///  Validate Write Notification Validate
        /// </summary>
        public WriteNotificationValidate()
        {
            
        }
    }
}