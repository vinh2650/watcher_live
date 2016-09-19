using System.Collections.Generic;

namespace ReplyEmailsService.Models
{
    public class ReplyNotificationModel
    {
        public ReplyNotificationModel()
        {
            AttachmentList = new List<string>();
        }

        public string Email { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public List<string> AttachmentList { get; set; }
        public string NotificationId { get; set; }
    }
}
