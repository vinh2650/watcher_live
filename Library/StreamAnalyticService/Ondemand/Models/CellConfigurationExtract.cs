namespace StreamAnalyticService.Ondemand.Models
{

    /// <summary>
    /// Configuration for cell that extract from XML files
    /// </summary>
    public class CellConfigurationExtract
    {

        public string OperatorId { get; set; }


        /// <summary>
        /// LocalCellId
        /// </summary>
        public string LocalCellId { get; set; }

        /// <summary>
        /// CellName
        /// </summary>
        public string CellName { get; set; }

        /// <summary>
        /// <CsgInd>0</CsgInd> <!--False-->
        /// </summary>
        public string CsgInd { get; set; }

        /// <summary>
        /// <UlCyclicPrefix>0</UlCyclicPrefix><!--Normal-->
        /// </summary>
        public string UlCyclicPrefix { get; set; }

        /// <summary>
        /// <DlCyclicPrefix>0</DlCyclicPrefix><!--Normal-->
        /// </summary>
        public string DlCyclicPrefix { get; set; }

        /// <summary>
        /// FreqBand
        /// </summary>
        public string FreqBand { get; set; }

        /// <summary>
        /// <UlEarfcnCfgInd>0</UlEarfcnCfgInd><!--Not configure-->
        /// </summary>
        public string UlEarfcnCfgInd { get; set; }

        /// <summary>
        /// DlEarfcn
        /// </summary>
        public string DlEarfcn { get; set; }

        /// <summary>
        /// <UlBandWidth>5</UlBandWidth><!--20M-->
        /// </summary>
        public string UlBandWidth { get; set; }

        /// <summary>
        /// <DlBandWidth>5</DlBandWidth><!--20M-->
        /// </summary>
        public string DlBandWidth { get; set; }

        /// <summary>
        /// CellId
        /// </summary>
        public string CellId { get; set; }

        /// <summary>
        /// PhyCellId
        /// </summary>
        public string PhyCellId { get; set; }

        /// <summary>
        /// AdditionalSpectrumEmission
        /// </summary>
        public string AdditionalSpectrumEmission { get; set; }

        /// <summary>
        /// <CellActiveState>1</CellActiveState><!--Active-->
        /// </summary>
        public string CellActiveState { get; set; }

        /// <summary>
        /// <CellAdminState>0</CellAdminState><!--Unblock-->
        /// </summary>
        public string CellAdminState { get; set; }

        /// <summary>
        /// <FddTddInd>0</FddTddInd><!--FDD-->
        /// </summary>
        public string FddTddInd { get; set; }

        /// <summary>
        /// <CellSpecificOffset>15</CellSpecificOffset><!--0dB-->
        /// </summary>
        public string CellSpecificOffset { get; set; }

        /// <summary>
        /// <QoffsetFreq>15</QoffsetFreq><!--0dB-->
        /// </summary>
        public string QoffsetFreq { get; set; }

        /// <summary>
        /// RootSequenceIdx
        /// </summary>
        public string RootSequenceIdx { get; set; }

        /// <summary>
        /// <HighSpeedFlag>0</HighSpeedFlag><!--Low speed cell flag-->
        /// </summary>
        public string HighSpeedFlag { get; set; }

        /// <summary>
        /// PreambleFmt
        /// </summary>
        public string PreambleFmt { get; set; }

        /// <summary>
        /// CellRadius
        /// </summary>
        public string CellRadius { get; set; }

        /// <summary>
        /// <CustomizedBandWidthCfgInd>0</CustomizedBandWidthCfgInd><!--Not configure-->
        /// </summary>
        public string CustomizedBandWidthCfgInd { get; set; }

        /// <summary>
        /// <EmergencyAreaIdCfgInd>0</EmergencyAreaIdCfgInd><!--Not configure-->
        /// </summary>
        public string EmergencyAreaIdCfgInd { get; set; }

        /// <summary>
        /// <UePowerMaxCfgInd>0</UePowerMaxCfgInd><!--Not configure-->
        /// </summary>
        public string UePowerMaxCfgInd { get; set; }

        /// <summary>
        /// <MultiRruCellFlag>0</MultiRruCellFlag><!--False-->
        /// </summary>
        public string MultiRruCellFlag { get; set; }

        /// <summary>
        /// <CPRICompression>0</CPRICompression><!--No Compression-->
        /// </summary>
        public string CpriCompression { get; set; }

        /// <summary>
        /// <AirCellFlag>0</AirCellFlag><!--False-->
        /// </summary>
        public string AirCellFlag { get; set; }

        /// <summary>
        /// <CrsPortNum>1</CrsPortNum><!--1 port-->
        /// </summary>
        public string CrsPortNum { get; set; }


        /// <summary>
        /// <TxRxMode>0</TxRxMode><!--1T1R-->
        /// </summary>
        public string TxRxMode { get; set; }

        /// <summary>
        /// UserLabel
        /// </summary>
        public string UserLabel { get; set; }

        /// <summary>
        /// <WorkMode>0</WorkMode><!--Uplink and downlink-->
        /// </summary>
        public string WorkMode { get; set; }

        /// <summary>
        /// <EuCellStandbyMode>0</EuCellStandbyMode><!--Active-->
        /// </summary>
        public string EuCellStandbyMode { get; set; }


        /// <summary>
        /// CellSlaveBand
        /// </summary>
        public string CellSlaveBand { get; set; }

        /// <summary>
        /// CnOpSharingGroupId
        /// </summary>
        public string CnOpSharingGroupId { get; set; }

        /// <summary>
        /// <IntraFreqRanSharingInd>1</IntraFreqRanSharingInd><!--True-->
        /// </summary>
        public string IntraFreqRanSharingInd { get; set; }


        /// <summary>
        /// <IntraFreqAnrInd>1</IntraFreqAnrInd><!--ALLOWED-->
        /// </summary>
        public string IntraFreqAnrInd { get; set; }

        /// <summary>
        /// <CellScaleInd>0</CellScaleInd><!--MACRO-->
        /// </summary>
        public string CellScaleInd { get; set; }

        /// <summary>
        /// FreqPriorityForAnr
        /// </summary>
        public string FreqPriorityForAnr { get; set; }

        /// <summary>
        /// CellRadiusStartLocation
        /// </summary>
        public string CellRadiusStartLocation { get; set; }

        /// <summary>
        /// objId
        /// </summary>
        public string ObjId { get; set; }
    }



}
