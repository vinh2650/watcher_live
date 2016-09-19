using System;

namespace Core
{
    /// <summary>
    /// Base Entity for project
    /// </summary>
    public abstract class BaseEntity
    {       
        /// <summary>
        /// Id of entity with format as GUID
        /// </summary>
        public string Id { get; set; }

    
        /// <summary>
        /// Created Date in UTC of entity
        /// </summary>
        public DateTime CreatedDateUtc { get; set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
            CreatedDateUtc = DateTime.UtcNow;
        }     
    }
}
