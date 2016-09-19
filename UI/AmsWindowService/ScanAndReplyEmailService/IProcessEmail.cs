using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using ScanAndReplyEmailService.Models;
using ScanAndReplyEmailService.Services;
using System.Web.Script.Serialization;
using Core.Domain.Authentication;
using Core.Domain.Business;
using CsQuery;
using MimeKit;
using Service.Interface.Authentication;
using Service.Interface.Business;

namespace ScanAndReplyEmailService
{
    public interface IProcessEmail
    {

        /// <summary>
        /// 
        /// </summary>
        void ProcessMailQueue();
    }
}
