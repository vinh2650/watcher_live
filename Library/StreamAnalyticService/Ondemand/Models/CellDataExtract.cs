using System;

namespace StreamAnalyticService.Ondemand.Models
{
    /// <summary>
    /// Cell data extract from excel
    /// </summary>
    public class CellDataExtract
    {
        public string JobId { get; set; }
        public string OperatorId { get; set; }

        /// <summary>
        /// Cell Id
        /// </summary>
        public string CellId { get; set; }

        /// <summary>
        /// Cell Name
        /// </summary>
        public string CellName { get; set; }

        /// <summary>
        /// InDateUtc, mapping with column StartTime in excel file
        /// </summary>
        public DateTime InDateUtc { get; set; }

        /// <summary>
        /// Mapping with column L.ChMeas.MIMO.PRB.CL.Rank1 (None) or column D in excel file
        /// </summary>
        public decimal Rank1 { get; set; }

        /// <summary>
        /// Mapping with column L.ChMeas.MIMO.PRB.CL.Rank2 (None) or column E in excel file
        /// </summary>
        public decimal Rank2 { get; set; }

        /// <summary>
        /// Mapping with column L.ChMeas.MIMO.PRB.CL.Rank1 (None) or column D in excel file
        /// </summary>
        public decimal Rank3 { get; set; }

        /// <summary>
        /// Mapping with column L.ChMeas.MIMO.PRB.CL.Rank2 (None) or column E in excel file
        /// </summary>
        public decimal Rank4 { get; set; }

        /// <summary>
        /// Interference avg, for 4g
        /// </summary>
        public decimal InterferenceAvg { get; set; }
        /// <summary>
        /// rtwp for 3g
        /// </summary>
        public decimal Rtwp { get; set; }

        public int CellType { get; set; }

        public string EnodeBId { get; set; }

        public double SecondWillBeAdd { get; set; }

        public decimal EpochMillisecond { get; set; }

        public DateTime? DateWithoutTimeUtc { get; set; }

        public string NeName { get; set; }

        public string SiteId { get; set; }

        public string BtsName { get; set; }



        public bool IsFifthData { get; set; }

        #region PrbData

        public double PrbSum => (Prb0 ?? 0)
                                  + (Prb1 ?? 0)
                                  + (Prb2 ?? 0)
                                  + (Prb3 ?? 0)
                                  + (Prb4 ?? 0)
                                  + (Prb5 ?? 0)
                                  + (Prb6 ?? 0)
                                  + (Prb7 ?? 0)
                                  + (Prb8 ?? 0)
                                  + (Prb9 ?? 0);

        public double PrbCount => (Prb0 == null ? 0 : 1) 
                                         + (Prb1 == null ? 0 : 1)
                                         + (Prb2 == null ? 0 : 1)
                                         + (Prb3 == null ? 0 : 1)
                                         + (Prb4 == null ? 0 : 1)
                                         + (Prb5 == null ? 0 : 1)
                                         + (Prb6 == null ? 0 : 1)
                                         + (Prb7 == null ? 0 : 1)
                                         + (Prb8 == null ? 0 : 1)
                                         + (Prb9 == null ? 0 : 1);


        public double? Prb0 { get; set; }
        public double? Prb1 { get; set; }
        public double? Prb2 { get; set; }
        public double? Prb3 { get; set; }
        public double? Prb4 { get; set; }
        public double? Prb5 { get; set; }
        public double? Prb6 { get; set; }
        public double? Prb7 { get; set; }
        public double? Prb8 { get; set; }
        public double? Prb9 { get; set; }


        #endregion


    }
    public enum CellTypeExtract
    {
        /// <summary>
        /// 2G
        /// </summary>
        TwoGen = 2,

        /// <summary>
        /// 3G
        /// </summary>
        ThereGen = 3,
        /// <summary>
        /// 4G
        /// </summary>
        FourthGen = 4
    }
}
