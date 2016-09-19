using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
    public class ProcessEmail : IProcessEmail
    {
        private const string UnusedCharacters = "RE: ";
        private readonly IUserService _userService;

        public ProcessEmail(IUserService userService)
        {
            _userService = userService;
        }


        //public void ProcessMailQueue()
        //{
        //    var server = ConfigurationManager.AppSettings["MailHostName"];
        //    var port = Convert.ToInt16(ConfigurationManager.AppSettings["MailHostNamePortNo"]);
        //    var userName = ConfigurationManager.AppSettings["MailUserName"];
        //    var password = ConfigurationManager.AppSettings["MailUserPassword"];
        //    var ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);

        //    using (var client = new ImapClient())
        //    {
        //        client.Connect(server, port, ssl);

        //        // Note: since we don't have an OAuth2 token, disable
        //        // the XOAUTH2 authentication mechanism.
        //        client.AuthenticationMechanisms.Remove("XOAUTH2");

        //        client.Authenticate(userName, password);
        //        if (!client.IsAuthenticated)
        //        {
        //            return;
        //        }

        //        // The Inbox folder is always available on all IMAP servers...

        //        var folder = client.Inbox;
        //        folder.Open(FolderAccess.ReadWrite);
        //        var inbox = folder.Search(SearchQuery.NotSeen);

        //        for (int i = 0; i < inbox.Count; i++)
        //        {
        //            var messageId = inbox[i];
        //            var email = folder.GetMessage(messageId);
        //            var subject = email.Subject;

        //            // remove unused characters
        //            if (subject.IndexOf(UnusedCharacters, StringComparison.OrdinalIgnoreCase) > -1)
        //            {
        //                subject = subject.Remove(subject.IndexOf(UnusedCharacters, StringComparison.OrdinalIgnoreCase),
        //                    UnusedCharacters.Length);
        //            }
        //            //todo validate this email too
        //            var from = email.From.Mailboxes.FirstOrDefault().Address;
        //            //var uuid = email.MessageId;
        //            var body = email.HtmlBody;
        //            if (string.IsNullOrEmpty(body))
        //                continue;

        //            var fromAddress = (email.From == null ? null : email.From.FirstOrDefault()) as MailboxAddress;
        //            if (fromAddress == null)
        //            {
        //                continue;
        //            }


        //            var attachmentService = new AttachmentService();
        //            var attachments = email.Attachments.ToList();
        //            var attachmentList = new List<string>();
        //            if (attachments.Any())
        //            {
        //                foreach (var attachment in attachments)
        //                {
        //                    var url = attachmentService.SaveAttachment(attachment as MimePart);
        //                    if (!string.IsNullOrEmpty(url))
        //                        attachmentList.Add(url);
        //                }
        //            }


        //            // Only procceed email that from AMS
        //            if (subject.IndexOf("(AMS)", StringComparison.Ordinal) > -1)
        //            {
        //                // Change unseen to seen message
        //                folder.AddFlags(messageId, MessageFlags.Seen, true);

        //                // Extract body
        //                var contentMessage = ExtractMessage(body, email, attachmentService);

        //                // Extract Post id
        //                var postId = ExtractNotificationId(body);

        //                // Send comment
        //                if (!string.IsNullOrEmpty(postId) && contentMessage.Length > 0)
        //                {
        //                    try
        //                    {

        //                        AppendReplyForNotification(fromAddress.Address, contentMessage, postId, attachmentList);

        //                    }
        //                    catch (Exception ex)
        //                    {

        //                    }
        //                }
        //            }

        //            // Process Email for BTS and Cell then we create notification for BTS or Cell
        //            //check with regex

        //            if (Regex.IsMatch(subject, @"^\d+\_[a-zA-Z0-9]{1,}"))
        //            {
        //                // Change unseen to seen message
        //                folder.AddFlags(messageId, MessageFlags.Seen, true);

        //                try
        //                {
        //                   // CreateNewPost(fromAddress.Address, subject, body, attachmentList);
        //                }
        //                catch (Exception ex)
        //                {

        //                }
        //            }

                    
        //        }

        //        client.Disconnect(true);
        //    }
        //}

        //private void AppendReplyForNotification(string email, string content, string notificationId, List<string> attachmentList)
        //{
        //    try
        //    {

        //        var successMessage = "";
        //        User user;
        //        var outMessage = "";

        //        //check notification by id
        //        var notification = _postService.GetNotificationById(notificationId);
        //        if (notification == null)
        //        {
        //            //todo send email inform that post is not exist
        //            Trace.TraceError("Post is not exist");
        //            return;
        //        }
        //        //create reply for notification
        //        var postComment = _postService.ReplyNotificationFromEmail(
        //            notification.Id,
        //            content,
        //            attachmentList,
        //            email,
        //            out outMessage,
        //            null,
        //            notification);

        //        //send mail to receivers
        //        var fromEmail = ConfigurationManager.AppSettings["FromEmail"];
        //        var fromName = ConfigurationManager.AppSettings["FromName"];
        //        var domain = ConfigurationManager.AppSettings["BaseUrl"];
        //        //get user linking
        //        var usersLinking = _postService.GetUserLinkingsByNotificationId(notificationId);
        //        var receivers = new List<string>();

        //        //get owner reveiver
        //        var owner = _userService.GetUserById(notification.OwnerId);
        //        if (owner != null && owner.Email != email)
        //        {
        //            receivers.Add(owner.Email);
        //        }

        //        //get list reivers link with post
        //        if (usersLinking.Count > 0)
        //        {
        //            receivers.AddRange(usersLinking.Where(m => m.Email != email).Select(m => m.Email).ToList());
        //        }
        //        ;

        //        var templateFormat = "<a href='{0}#/app/home?lat={1}&lng={2}&id={3}&type={4}&postId={5}'>{6}</a>";

        //        if (receivers.Count > 0)
        //        {
        //            foreach (var receiver in receivers)
        //            {
        //                var userByEmail = _userService.GetUserByEmail(receiver);
        //                //check email has exist or not 
        //                var subject = !string.IsNullOrEmpty(notification.Subject)
        //                    ? string.Format(templateFormat, domain, "latLng.Latitude", "latLng.Longitude",
        //                        notification.LinkId, (int) notification.Type, notification.Id, notification.Subject)
        //                    : "";
        //                var footerNotify = userByEmail != null
        //                    ? ""
        //                    : "(Your email has not been registered in the AMS System. Your reply won't appear in AMS System. Please contact Administrator to register your email.)";

        //                var templateForWritePost = _emailService.GetWriteNotificationTemplate("WritePost.html",
        //                    content,
        //                    ":: Reply above this line to post a reply ::",

        //                    subject, footerNotify);

        //                _emailService.SendEmail(
        //                    fromEmail,
        //                    fromName,
        //                    new List<string>() {receiver},
        //                    "",
        //                    "",
        //                    notification.Subject, templateForWritePost, attachmentList);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}

        //private void CreateNewNotification(string email, string subject, string content, List<string> attachments)
        //{
        //    // validate subject
        //    if (string.IsNullOrEmpty(subject))
        //    {
        //        Trace.TraceError("there's wrong with subject");
        //        return;
        //    }
        //    if (!Regex.IsMatch(subject, @"^\d+\_[a-zA-Z0-9]{1,}"))
        //    {
        //        Trace.TraceError("Subject is not in correct format");
        //        return;
        //    }
        //    // validate attachments source
        //    if (!CheckValidationAttachments(attachments))
        //    {

        //        Trace.TraceError("Attachments is corrupt or not exist. Cannot write post!");
        //        return;
        //    }

        //    var user = _userService.GetUserByEmail(email);
        //    if (user == null)
        //    {
        //        //todo 
        //        _emailService.SendEmailRemindRegisterAccountAsync(email);
        //        Trace.TraceError("This email not exist in AMS. Please register for email");
        //        return;
        //    }

        //    // get BTS/Site
        //    var name = subject.Split(' ')[1];
            


        //    //create post
        //    var post = new Notification()
        //    {
        //        Subject = subject,
        //        Content = content,
        //        OwnerId = user.Id,
        //        AttachmentList = attachments.Count > 0 ? string.Join(",", attachments) : "",
        //        UpdatedDateUtc = DateTime.UtcNow
        //    };

            

        //    //try
        //    //{
        //    //    if (model.Subject.IndexOf(PostMapType_Site, StringComparison.OrdinalIgnoreCase) > -1)
        //    //    {
        //    //        //var bts = _btsConfigureService.GetBtsByName(name, user.CurrentLibraryId);
        //    //        var bts = _btsSearchService.GetByLibraryIdAndBtsName(user.CurrentLibraryId, name);
        //    //        if (bts == null)
        //    //            return Error("This site is not exist");
        //    //        //postMap.LinkId = (string)bts["Id"];
        //    //        postMap.LinkId = bts.id;
        //    //        postMap.Type = PostMapType.Bts;

        //    //    }
        //    //    else
        //    //    {
        //    //        //var site = _siteConfigureService.GetSiteByName(name, user.CurrentLibraryId);
        //    //        var site = _cellSearchService.GetByCellNameAndLibraryId(user.CurrentLibraryId, name);
        //    //        if (site == null)
        //    //            return Error("This cell is not exist");
        //    //        postMap.LinkId = (string)site["id"];
        //    //        postMap.Type = PostMapType.Cell;
        //    //    }

        //    //    _postService.WritePost(post, postMap, null);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return Error(ex);
        //    //}


        //}

        //private string ExtractMessage(string body, MimeMessage email, AttachmentService attachmentService)
        //{
        //    if (string.IsNullOrWhiteSpace(body)) return body;

        //    const string srcPrefix = "cid:";


        //    CQ dom = "<div></div>";
        //    dom.Html(body);
        //    var bodyElem = dom.Find("body").FirstOrDefault();
        //    if (bodyElem != null)
        //    {
        //        CQ newDom = "<div></div>";
        //        newDom.Html(bodyElem.InnerHTML);
        //        dom = newDom;
        //    }

        //    // remove the gmail extra
        //    dom.Find(".gmail_extra").Remove();

        //    // remove the yahoo extra
        //    dom.Find(".qtdSeparateBR").Remove();
        //    dom.Find(".yahoo_quoted").Remove();

        //    // download image and replace the src attribute of all img elements
        //    var allImages = dom.Find("img").ToList();
        //    foreach (var img in allImages)
        //    {
        //        var src = img.GetAttribute("src");
        //        if (!string.IsNullOrEmpty(src) && src.StartsWith(srcPrefix, StringComparison.OrdinalIgnoreCase))
        //        {
        //            var cidStr = src.Substring(srcPrefix.Length);
        //            var cidImage = email.BodyParts.FirstOrDefault(
        //                x => string.Equals(cidStr, x.ContentId, StringComparison.OrdinalIgnoreCase)) as MimePart;
        //            if (cidImage != null)
        //            {
        //                string newSrc = attachmentService.SaveAttachment(cidImage);
        //                img.SetAttribute("src", newSrc);
        //            }
        //        }
        //    }

        //    return dom.Html();
        //}

        //private string ExtractNotificationId(string body)
        //{
        //    CQ dom = "<div></div>";
        //    dom.Html(body);
        //    var alinks = dom.Find("a");

        //    string titlePrefix = "post-id-";
        //    foreach (var alink in alinks)
        //    {
        //        var title = alink.GetAttribute("title");
        //        if (!string.IsNullOrWhiteSpace(title) &&
        //            title.StartsWith(titlePrefix, StringComparison.OrdinalIgnoreCase))
        //            return title.Substring(titlePrefix.Length);
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// validate attachment before send attach
        ///// </summary>
        ///// <param name="attachments"></param>
        ///// <returns></returns>
        //private bool CheckValidationAttachments(List<string> attachments)
        //{
        //    attachments.RemoveAll(string.IsNullOrEmpty);
        //    if (attachments.Count > 0)
        //    {
        //        Uri myUri;
        //        foreach (var url in attachments)
        //        {
        //            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out myUri);
        //            if (!myUri.IsAbsoluteUri)
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}
        public void ProcessMailQueue()
        {
            throw new NotImplementedException();
        }
    }
}
