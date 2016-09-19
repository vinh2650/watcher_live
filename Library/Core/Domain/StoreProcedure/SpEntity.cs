
using System;
using System.Collections.Generic;

namespace Core.Domain.StoreProcedure
{ /// <summary>
  /// get bts filter model
  /// </summary>
    public class GetBtsFilter
    {
        /// <summary>
        /// column name
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// value
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// get bts sort model
    /// </summary>
    public class GetBtsSort
    {
        /// <summary>
        /// column name
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// sort type
        /// </summary>
        public SortType SortType { get; set; }
    }

    /// <summary>
    /// sort type
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// descrease
        /// </summary>
        Desc = 1,
        /// <summary>
        /// ascrease
        /// </summary>
        Asc = 2
    }

    /// <summary>
    /// custom filter
    /// </summary>
    public class CustomFilter
    {
        /// <summary>
        /// constructor
        /// </summary>
        public CustomFilter()
        {
            CustomSubFilters = new List<CustomSubFilter>();
        }

        /// <summary>
        /// technology
        /// </summary>
        public string Technology { get; set; }

        /// <summary>
        /// custom filter
        /// </summary>
        public List<CustomSubFilter> CustomSubFilters { get; set; }
    }

    /// <summary>
    /// sub filter
    /// </summary>
    public class CustomSubFilter
    {
        /// <summary>
        /// band
        /// </summary>
        public string Band { get; set; }
    }
    /// <summary>
    /// store procedure entities
    /// </summary>
    public static class SpEntity
    {
        /// <summary>
        /// organization sp entity
        /// </summary>
        

      

        /// <summary>
        /// Store comments in post
        /// </summary>
        public class Reply
        {
            public string Id { get; set; }
            public string NotificationId { get; set; }
            public string OwnerId { get; set; }
            public string Content { get; set; }
            public DateTime? CreatedDateUtc { get; set; }
            public DateTime? UpdatedDateUtc { get; set; }
            public string Subject { get; set; }
            public string AttachmentList { get; set; }
            public long? RowCounts { get; set; }
        }

        /// <summary>
        /// entity get message from SP
        /// </summary>
        public class Messages
        {
            public string Id { get; set; }
            public string OwnerId { get; set; }
            public string Content { get; set; }
            public DateTime? CreatedDateUtc { get; set; }
            public string Subject { get; set; }
            public string AttachmentList { get; set; }
            public string Author { get; set; }
            public long? RowCounts { get; set; }
            public int Total { get; set; }
        }

        /// <summary>
        /// entity get other message from SP
        /// </summary>
        public class OtherMessages
        {
            public string Id { get; set; }
            public string OwnerId { get; set; }
            public string Content { get; set; }
            public DateTime? CreatedDateUtc { get; set; }
            public string Subject { get; set; }
            public string AttachmentList { get; set; }
            public string Author { get; set; }
            public long? RowCounts { get; set; }
            public int Total { get; set; }
        }

        /// <summary>
        /// get total message for cells
        /// </summary>
        public class TotalMessageForCells
        {
            public string Id { get; set; }

            public int Total { get; set; }
        }

        /// <summary>
        /// get total message for bts
        /// </summary>
        public class TotalMessageForBts
        {
            public string Id { get; set; }

            public int Total { get; set; }
        }

        public class MessagesForCell
        {
            /// <summary>
            /// Id
            /// </summary>
            public string Id { get; set ; }

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
            
            /// <summary>
            /// Created Date in UTC of entity
            /// </summary>
            public DateTime? CreatedDateUtc { get; set; }

            public long? RowCounts { get; set; }
        }

        public class CellsDataByCellName
        {
            /// <summary>
            /// 
            /// </summary>
            public string CellId { get; set; }

            /// <summary>
            /// Created Date in UTC of entity
            /// </summary>
            public DateTime? CreatedDateUtc { get; set; }

            /// <summary>
            /// Cell Name
            /// </summary>
            public string CellName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long? InterferenceDbm { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long? RtwpDbm { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public long? RowCounts { get; set; }
        }

      
        
        
    }

    
}
