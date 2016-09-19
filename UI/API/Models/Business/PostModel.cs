using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using API.Validations.Business;
using Core.Domain.Business;
using FluentValidation.Attributes;

namespace API.Models.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class GetMessageResult
    {
        /// <summary>
        /// 
        /// </summary>
        public GetMessageResult()
        {
            Messages = new List<GetNotificationResults>();
        }
        /// <summary>
        /// 
        /// </summary>
        public long Records { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<GetNotificationResults> Messages { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<OtherMessagesData> OtherMessagesData { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class OtherMessagesData
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Total { get; set; }
    }
    /// <summary>
    /// ViewModel GetPostResults
    /// </summary>
    public class GetNotificationResults
    {
        /// <summary>
        /// 
        /// </summary>
        public GetNotificationResults()
        {
            To = new List<AssignedModel>();
            AttachmentList = new List<string>();
        }
        /// <summary>
        /// Id Post result
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        ///name author
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        ///  subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// list assigned
        /// </summary>
        public List<AssignedModel> To { get; set; }
        /// <summary>
        /// message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// create time
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// total
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// attachmentList
        /// </summary>
        public List<string> AttachmentList { get; set; }
        /// <summary>
        /// user
        /// </summary>
        public string UserId { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class AssignedModel
    {
        /// <summary>
        /// userId
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// email
        /// </summary>
        public string Email { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class GetReplyResult
    {
        /// <summary>
        /// init contructor
        /// </summary>
        public GetReplyResult()
        {
            Replies = new List<GetCommentResult>();
        }
        /// <summary>
        /// record
        /// </summary>
        public long Records { get; set; }
        /// <summary>
        /// list replie
        /// </summary>
        public List<GetCommentResult> Replies { get; set; }
    }
    /// <summary>
    /// class GetCommentResult
    /// </summary>
    public class GetCommentResult
    {
        /// <summary>
        /// contructor
        /// </summary>
        public GetCommentResult()
        {
            AttachmentList = new List<string>();
        }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Author
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// AttachmentList
        /// </summary>
        public List<string> AttachmentList { get; set; }

        /// <summary>
        /// id of user
        /// </summary>
        public string UserId { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class NotificationModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Content
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// CreatedDate
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// User
        /// </summary>
        public UserNotificationModel User { get; set; }
    }
    /// <summary>
    /// class UserPostModel
    /// </summary>
    public class UserNotificationModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// FirstName
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// LastName
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Avatar
        /// </summary>
        public string Avatar { get; set; }
    }

    /// <summary>
    /// Write Post view model
    /// </summary>
    [Validator(typeof(WriteNotificationValidate))]
    public class WriteNotificationModel
    {
        /// <summary>
        /// Object name, bts name or cell name
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Content
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// AttachmentList
        /// </summary>
        public List<string> AttachmentList { get; set; }
        /// <summary>
        /// AssignedList
        /// </summary>
        public List<string> AssignedList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public WriteNotificationModel()
        {
            AttachmentList = new List<string>();
            AssignedList = new List<string>();
        }

    }

    /// <summary>
    /// Write post from email view model
    /// </summary>
    public class WriteNotificationFromEmailModel
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public WriteNotificationFromEmailModel()
        {
            AttachmentList = new List<string>();
        }

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// AttachmentList
        /// </summary>
        public List<string> AttachmentList { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// class WriteCommentModel
    /// </summary>
    public class WriteCommentModel
    {
        /// <summary>
        /// contructor WriteCommentModel()
        /// </summary>
        public WriteCommentModel()
        {
            AttachmentList = new List<string>();
        }
        /// <summary>
        /// PostId
        /// </summary>
        public string NotificationId { get; set; }
        /// <summary>
        /// Content
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// AttachmentList
        /// </summary>
        public List<string> AttachmentList { get; set; }
    }
    /// <summary>
    /// class WriteCommentFromEmailModel
    /// </summary>
    public class WriteCommentFromEmailModel : WriteCommentModel
    {
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// get message of cell
    /// </summary>
    public class GetMessageOfCell
    {
        /// <summary>
        /// contructor
        /// </summary>
        public GetMessageOfCell()
        {
            Messages = new List<MessageForGetMessageOfCell>();
        }

        /// <summary>
        /// total message
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// cell Name
        /// </summary>
        //public string CellId { get; set; }
        public string CellName { get; set; }

        /// <summary>
        /// messages
        /// </summary>
        public List<MessageForGetMessageOfCell> Messages { get; set; }
    }

    /// <summary>
    /// message for get message of cell
    /// </summary>
    public class MessageForGetMessageOfCell
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// OwnerId 
        /// </summary>
        public string OwnerId { get; set; }
        /// <summary>
        /// Content
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// AttachmentList
        /// </summary>
        public string AttachmentList { get; set; }
        /// <summary>
        /// UpdatedDateUtc
        /// </summary>
        public DateTime? UpdatedDateUtc { get; set; }
    }

    /// <summary>
    /// Get Notification By Cell Name Request Model
    /// </summary>
    public class GetNotificationByCellnameRequest
    {
        /// <summary>
        /// CellName
        /// </summary>
        public string CellName { get; set; }

        /// <summary>
        /// Skip Post
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Take Post
        /// </summary>
        public int Take { get; set; }
    }

}