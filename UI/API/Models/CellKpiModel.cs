using System;


namespace API.Models
{
    public class CellKpiModel
    {
        /// <summary>
        /// InDateUtc
        /// </summary>
        public DateTime? CreateDateTimeUtc { get; set; }

        /// <summary>
        /// Cell Id, 
        /// </summary>
        public string CellId { get; set; }

        public string AvgRank { get; set; }


        public string Alarm { get; set; }
    }
}
