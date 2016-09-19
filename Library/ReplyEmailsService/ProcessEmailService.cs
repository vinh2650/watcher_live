using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using CsQuery;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using ReplyEmailsService.Models;
using ReplyEmailsService.Services;

namespace ReplyEmailsService
{
    public class ProcessEmailService
    {
        private const string UnusedCharacters = "RE: ";

        public void ProcessMailQueue(TextWriter log)
        {
            var server = ConfigurationManager.AppSettings["MailHostName"];
            var port = Convert.ToInt16(ConfigurationManager.AppSettings["MailHostNamePortNo"]);
            var userName = ConfigurationManager.AppSettings["MailUserName"];
            //var name = ConfigurationManager.AppSettings["MailName"];
            var password = ConfigurationManager.AppSettings["MailUserPassword"];
            var ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
            //var domain = ConfigurationManager.AppSettings["BaseUrl"];

            using (var client = new ImapClient())
            {
                if (log != null)
                    log.WriteLine("Connecting...");

                client.Connect(server, port, ssl);

                if (log != null)
                {
                    log.WriteLine(" Done!");
                    log.WriteLine("Logging in...");
                }

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(userName, password);
                if (!client.IsAuthenticated)
                {
                    if (log != null)
                        log.WriteLine("Authenticate fail!");
                    return;
                }

                // The Inbox folder is always available on all IMAP servers...

                if (log != null)
                    log.WriteLine("Reading unseen emails...");

                var folder = client.Inbox;
                folder.Open(FolderAccess.ReadWrite);
                var inbox = folder.Search(SearchQuery.NotSeen);

                if (log != null)
                {
                    log.WriteLine("Total: " + inbox.Count + " email(s)");
                    log.WriteLine("Sending posts...");
                }


                for (int i = 0; i < inbox.Count; i++)
                {
                    var messageId = inbox[i];
                    var email = folder.GetMessage(messageId);
                    var subject = email.Subject;

                    // remove unused characters
                    if (subject.IndexOf(UnusedCharacters, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        subject = subject.Remove(subject.IndexOf(UnusedCharacters, StringComparison.OrdinalIgnoreCase),
                            UnusedCharacters.Length);
                    }

                    //var uuid = email.MessageId;
                    var body = email.HtmlBody;
                    if (string.IsNullOrEmpty(body))
                        continue;

                    var fromAddress = (email.From == null ? null : email.From.FirstOrDefault()) as MailboxAddress;
                    if (fromAddress == null)
                    {
                        if (log != null)
                            log.WriteLine("{0} Can't find out sender email address", messageId.Id);
                        continue;
                    }


                    var attachmentService = new AttachmentService();
                    var attachments = email.Attachments.ToList();
                    var attachmentList = new List<string>();
                    if (attachments.Any())
                    {
                        foreach (var attachment in attachments)
                        {
                            var url = attachmentService.SaveAttachment(attachment as MimePart);
                            if (!string.IsNullOrEmpty(url))
                                attachmentList.Add(url);
                        }
                    }


                    // Only procceed email that from IMS
                    if (subject.IndexOf("(AMS)", StringComparison.Ordinal) > -1)
                    {
                        // Change unseen to seen message
                        folder.AddFlags(messageId, MessageFlags.Seen, true);

                        // Extract body
                        var contentMessage = ExtractMessage(body, email, attachmentService);

                        // Extract Post id
                        var postId = ExtractNotificationId(body);

                        // Send comment
                        if (!string.IsNullOrEmpty(postId) && contentMessage.Length > 0)
                        {
                            try
                            {
                                SendNotification(fromAddress.Address, contentMessage, postId, attachmentList, log);
                            }
                            catch (Exception ex)
                            {
                                if (log != null)
                                    log.WriteLine("post fail: " + ex);
                            }
                        }
                    }

                    // Only procceed email that from IMS
                    if (subject.IndexOf("Site", StringComparison.OrdinalIgnoreCase) > -1
                        || subject.IndexOf("Cell", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        // Change unseen to seen message
                        folder.AddFlags(messageId, MessageFlags.Seen, true);

                        try
                        {
                            SendMessage(fromAddress.Address, subject, body, attachmentList, log);
                        }
                        catch (Exception ex)
                        {
                            if (log != null)
                                log.WriteLine("post message fail: " + ex);
                        }
                    }
                }

                client.Disconnect(true);
            }

            if (log != null)
                log.WriteLine("Everything was done! Press a key to exit...");
        }

        private void SendNotification(string email, string content, string postId, List<string> attachmentList,
            TextWriter log)
        {
            var domain = ConfigurationManager.AppSettings["BaseUrl"];

            try
            {
                var webAddr = domain + "/Api/Post/ReplyFromEmail";

                var httpWebRequest = (HttpWebRequest) WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    if (log != null)
                    {
                        log.WriteLine("PARAMETER:");
                        log.WriteLine("Content: " + content);
                        log.WriteLine("Email: " + content);
                        log.WriteLine("PostId: " + content);
                        log.WriteLine("AttachmentList: ");
                        {
                            foreach (var attechment in attachmentList)
                            {
                                log.WriteLine("-----: " + attechment);
                            }
                        }
                    }

                    var model = new ReplyNotificationModel();
                    model.Content = content;
                    model.Email = email;
                    model.Subject = "";
                    model.AttachmentList = attachmentList;
                    model.NotificationId = postId;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(model);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                httpResponse.Dispose();
            }
            catch (Exception ex)
            {
                if (log != null)
                    log.WriteLine("Error: {0}{1}", ex.Message,
                        ex.InnerException != null ? "\n" + ex.InnerException.Message : null);
                throw;
            }
        }

        private string ExtractMessage(string body, MimeMessage email, AttachmentService attachmentService)
        {
            if (string.IsNullOrWhiteSpace(body)) return body;

            const string srcPrefix = "cid:";

            
            CQ dom = "<div></div>";
            dom.Html(body);
            var bodyElem = dom.Find("body").FirstOrDefault();
            if (bodyElem != null)
            {
                CQ newDom = "<div></div>";
                newDom.Html(bodyElem.InnerHTML);
                dom = newDom;
            }

            // remove the gmail extra
            dom.Find(".gmail_extra").Remove();

            // remove the yahoo extra
            dom.Find(".qtdSeparateBR").Remove();
            dom.Find(".yahoo_quoted").Remove();

            // download image and replace the src attribute of all img elements
            var allImages = dom.Find("img").ToList();
            foreach (var img in allImages)
            {
                var src = img.GetAttribute("src");
                if (!string.IsNullOrEmpty(src) && src.StartsWith(srcPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    var cidStr = src.Substring(srcPrefix.Length);
                    var cidImage = email.BodyParts.FirstOrDefault(
                        x => string.Equals(cidStr, x.ContentId, StringComparison.OrdinalIgnoreCase)) as MimePart;
                    if (cidImage != null)
                    {
                        string newSrc = attachmentService.SaveAttachment(cidImage);
                        img.SetAttribute("src", newSrc);
                    }
                }
            }

            return dom.Html();
        }

        private string ExtractNotificationId(string body)
        {
            CQ dom = "<div></div>";
            dom.Html(body);
            var alinks = dom.Find("a");

            string titlePrefix = "post-id-";
            foreach (var alink in alinks)
            {
                var title = alink.GetAttribute("title");
                if (!string.IsNullOrWhiteSpace(title) &&
                    title.StartsWith(titlePrefix, StringComparison.OrdinalIgnoreCase))
                    return title.Substring(titlePrefix.Length);
            }
            return null;
        }

        private void SendMessage(string email, string subject, string content, List<string> attachments, TextWriter log)
        {
            var domain = ConfigurationManager.AppSettings["BaseUrl"];

            try
            {
                var webAddr = domain + "/api/post/WritePostFromEmail";

                var httpWebRequest = (HttpWebRequest) WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    if (log != null)
                    {
                        log.WriteLine("PARAMETER:");
                        log.WriteLine("Content: " + content);
                        log.WriteLine("Email: " + content);
                        log.WriteLine("PostId: " + content);
                        log.WriteLine("AttachmentList: ");
                        {
                            foreach (var attechment in attachments)
                            {
                                log.WriteLine("-----: " + attechment);
                            }
                        }
                    }

                    var model = new ReplyNotificationModel();
                    model.Content = content;
                    model.Email = email;
                    model.Subject = subject;
                    model.AttachmentList = attachments;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    var json = serializer.Serialize(model);

                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                httpResponse.Dispose();
            }
            catch (Exception ex)
            {
                if (log != null)
                    log.WriteLine("Error: {0}{1}", ex.Message,
                        ex.InnerException != null ? "\n" + ex.InnerException.Message : null);
                throw;
            }
        }
    }
}
