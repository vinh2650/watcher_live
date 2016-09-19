using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using OfficeOpenXml;
using StreamAnalyticService.Ondemand.Interface;
using StreamAnalyticService.Ondemand.Models;

namespace StreamAnalyticService.Ondemand.Implement
{
    /// <summary>
    /// Implement cell data process 
    /// </summary>
    public class CellDataProcessService:ICellDataProcessService
    {
       
        private static readonly string CellDataSenderHub = ConfigurationManager.AppSettings["EventHub.Kpi.Path"];
        private static readonly string CellDatasenderConnectionString = ConfigurationManager.AppSettings["EventHub.Kpi.ConnectionString"];
        private static readonly string FolderWillScan = ConfigurationManager.AppSettings["Hangfire.ProcessXlsx.FolderWillScan"];
        
        static readonly string eventHubName = ConfigurationManager.AppSettings["EventHub.NetworkAlarmInput.Path"];
        static readonly string connectionString = ConfigurationManager.AppSettings["EventHub.NetworkAlarmInput.ConnectionString"];

        public List<CellDataExtract> ExtractData(Stream stream,string operatorId)
        {
            var result = new List<CellDataExtract>();

            try
            {

                using (var pck = new ExcelPackage(stream))
                {
                    var workSheet = pck.Workbook.Worksheets.First();

                    int numRows = workSheet.Dimension.End.Row;
                    int numCols = workSheet.Dimension.End.Column;

                    if (numRows < 2 || numCols < 4) return result;

                    int indexCreatedDate = 0;
                    int indexCellId = 0;
                    int indexRank1 = 0;
                    int indexRank2 = 0;
                    int indexRank3 = 0;
                    int indexRank4 = 0;
                    int indexFourGenDetect = 0;
                    int indexNeName = 0;
                    string fourGenDetect = "L.UL.Interference.Avg (dBm)";

                    #region Prb Index

                    int indexPrb0 = 0;
                    int indexPrb1 = 0;
                    int indexPrb2 = 0;
                    int indexPrb3 = 0;
                    int indexPrb4 = 0;
                    int indexPrb5 = 0;
                    int indexPrb6 = 0;
                    int indexPrb7 = 0;
                    int indexPrb8 = 0;
                    int indexPrb9 = 0;

                    #endregion


                    for (var i = 1; i <= numCols; i++)
                    {
                        var columnHeader = workSheet.Cells[1, i].Text;
                        if (!string.IsNullOrEmpty(columnHeader))
                        {
                            if (columnHeader.Equals("Start Time", StringComparison.Ordinal))
                                indexCreatedDate = i;
                            else if (columnHeader.Equals("Cell", StringComparison.Ordinal))
                                indexCellId = i;
                            else if (columnHeader.Equals("L.ChMeas.MIMO.PRB.OL.Rank1 (None)", StringComparison.Ordinal))
                                indexRank1 = i;
                            else if (columnHeader.Equals("L.ChMeas.MIMO.PRB.OL.Rank2 (None)", StringComparison.Ordinal))
                                indexRank2 = i;
                            else if (columnHeader.Equals("L.ChMeas.MIMO.PRB.OL.Rank3 (None)", StringComparison.Ordinal))
                                indexRank3 = i;
                            else if (columnHeader.Equals("L.ChMeas.MIMO.PRB.OL.Rank4 (None)", StringComparison.Ordinal))
                                indexRank4 = i;
                            else if (columnHeader.Equals("NE Name", StringComparison.Ordinal))
                                indexNeName = i;
                            
                            else if (columnHeader.Equals(fourGenDetect, StringComparison.Ordinal))
                                indexFourGenDetect = i;

                            #region prb
                            else if (columnHeader.Equals("L.UL.Interference.Avg.PRB0(dBm)", StringComparison.Ordinal))
                                indexPrb0 = i;
                            else if (columnHeader.Equals("L.UL.Interference.Avg.PRB1(dBm)", StringComparison.Ordinal))
                                indexPrb1 = i;
                            else if (columnHeader.Equals("L.UL.Interference.Avg.PRB2(dBm)", StringComparison.Ordinal))
                                indexPrb2 = i;
                            else if (columnHeader.Equals("L.UL.Interference.Avg.PRB3(dBm)", StringComparison.Ordinal))
                                indexPrb3 = i;
                            else if (columnHeader.Equals("L.UL.Interference.Avg.PRB4(dBm)", StringComparison.Ordinal))
                                indexPrb4 = i;
                            else if (columnHeader.Equals("L.UL.Interference.Avg.PRB5(dBm)", StringComparison.Ordinal))
                                indexPrb5 = i;
                            else if (columnHeader.Equals("L.UL.Interference.Avg.PRB6(dBm)", StringComparison.Ordinal))
                                indexPrb6 = i;
                            else if (columnHeader.Equals("L.UL.Interference.Avg.PRB7(dBm)", StringComparison.Ordinal))
                                indexPrb7 = i;
                            else if (columnHeader.Equals("L.UL.Interference.Avg.PRB8(dBm)", StringComparison.Ordinal))
                                indexPrb8 = i;
                            else if (columnHeader.Equals("L.UL.Interference.Avg.PRB9(dBm)", StringComparison.Ordinal))
                                indexPrb9 = i;
                            #endregion
                        }
                    }
                    if (indexCreatedDate < 1 || indexCellId < 1 || indexRank1 < 1 || indexRank2 < 1 || indexRank3 < 1 || indexRank4 < 1)
                        return result;
                    for (var i = 2; i <= numRows; i++)
                    {
                        DateTime createdDateUtc;
                        decimal rank1, rank2, rank3, rank4;

                        var cellEnodeBInfo = workSheet.Cells[i, indexCellId].Text;
                        if (string.IsNullOrEmpty(cellEnodeBInfo)
                            || !DateTime.TryParse(workSheet.Cells[i, indexCreatedDate].Value.ToString(), out createdDateUtc)
                            || !decimal.TryParse(workSheet.Cells[i, indexRank1].Value.ToString(), out rank1)
                            || !decimal.TryParse(workSheet.Cells[i, indexRank2].Value.ToString(), out rank2)
                            || !decimal.TryParse(workSheet.Cells[i, indexRank3].Value.ToString(), out rank3)
                            || !decimal.TryParse(workSheet.Cells[i, indexRank4].Value.ToString(), out rank4))
                            continue;

                        if ((rank1 + rank2 + rank3 + rank4) == 0)
                        {
                            continue;
                        }
                        var neName = workSheet.Cells[i, indexNeName].Value.ToString();

                        var cellName = GetCellInfo(cellEnodeBInfo, "Cell Name");
                        var cellId = GetCellInfo(cellEnodeBInfo, "Local Cell ID");
                        var enodeBId = GetCellInfo(cellEnodeBInfo, "eNodeB ID");
                        
                        var cellKpi = new CellDataExtract()
                        {
                            OperatorId = operatorId,
                            InDateUtc = createdDateUtc,
                            CellName = cellName,
                            //CellId = cellId + testId,
                            CellId = cellId ,
                            Rank1 = rank1,
                            Rank2 = rank2,
                            Rank3 = rank3,
                            Rank4 = rank4,
                            EnodeBId = enodeBId ,
                            NeName= neName,
                            EpochMillisecond =(decimal) (createdDateUtc - new DateTime(1970, 1, 1)).TotalMilliseconds,
                            DateWithoutTimeUtc = new DateTime(createdDateUtc.Year,createdDateUtc.Month,createdDateUtc.Day,0,0,0)


                        };
                        //this kpi is for 4g cell
                        if (indexFourGenDetect > 0)
                        {
                            cellKpi.CellType = (int)CellTypeExtract.FourthGen;
                            decimal avgDb = 0;
                            if (decimal.TryParse(workSheet.Cells[i, indexFourGenDetect].Value.ToString(), out avgDb))
                            {
                                cellKpi.InterferenceAvg = avgDb;
                            }
                            else
                            {
                                continue;
                                
                            }
                            
                        }

                        #region prb parse


                        if (createdDateUtc.Minute == 0)
                        {
                            cellKpi.IsFifthData = true;

                            double prb0 = 0;
                            if (double.TryParse(workSheet.Cells[i, indexPrb0].Value.ToString(), out prb0))
                            {
                                cellKpi.Prb0 = prb0;
                            }
                            else
                            {
                                cellKpi.Prb0 = null;
                            }
                            double prb1 = 0;
                            if (double.TryParse(workSheet.Cells[i, indexPrb1].Value.ToString(), out prb1))
                            {
                                cellKpi.Prb1 = prb1;
                            }
                            else
                            {
                                cellKpi.Prb1 = null;
                            }
                            double prb2 = 0;
                            if (double.TryParse(workSheet.Cells[i, indexPrb2].Value.ToString(), out prb2))
                            {
                                cellKpi.Prb2 = prb2;
                            }
                            else
                            {
                                cellKpi.Prb2 = null;
                            }
                            double prb3 = 0;
                            if (double.TryParse(workSheet.Cells[i, indexPrb3].Value.ToString(), out prb3))
                            {
                                cellKpi.Prb3 = prb3;
                            }
                            else
                            {
                                cellKpi.Prb3 = null;
                            }
                            double prb4 = 0;
                            if (double.TryParse(workSheet.Cells[i, indexPrb4].Value.ToString(), out prb4))
                            {
                                cellKpi.Prb4 = prb4;
                            }
                            else
                            {
                                cellKpi.Prb4 = null;
                            }
                            double prb5 = 0;
                            if (double.TryParse(workSheet.Cells[i, indexPrb5].Value.ToString(), out prb5))
                            {
                                cellKpi.Prb5 = prb5;
                            }
                            else
                            {
                                cellKpi.Prb5 = null;
                            }
                            double prb6 = 0;
                            if (double.TryParse(workSheet.Cells[i, indexPrb6].Value.ToString(), out prb6))
                            {
                                cellKpi.Prb6 = prb6;
                            }
                            else
                            {
                                cellKpi.Prb6 = null;
                            }
                            double prb7 = 0;
                            if (double.TryParse(workSheet.Cells[i, indexPrb7].Value.ToString(), out prb7))
                            {
                                cellKpi.Prb7 = prb7;
                            }
                            else
                            {
                                cellKpi.Prb7 = null;
                            }
                            double prb8 = 0;
                            if (double.TryParse(workSheet.Cells[i, indexPrb8].Value.ToString(), out prb8))
                            {
                                cellKpi.Prb8 = prb8;
                            }
                            else
                            {
                                cellKpi.Prb8 = null;
                            }
                            double prb9 = 0;
                            if (double.TryParse(workSheet.Cells[i, indexPrb9].Value.ToString(), out prb9))
                            {
                                cellKpi.Prb9 = prb9;
                            }
                            else
                            {
                                cellKpi.Prb9 = null;
                            }

                        }
                        else
                        {

                        }

                        #endregion


                        result.Add(cellKpi);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        public async Task SendCellDataToStream(List<CellDataExtract> data)
        {
            if (data == null) return;
            if (data.Count == 0) return;
            var eventHubClient = EventHubClient.CreateFromConnectionString(CellDatasenderConnectionString, CellDataSenderHub);

            var batchList = new List<EventData>();
            long batchSize = 0;
            var listEventData = data.Select(p => new EventData(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(p)))
            {
                PartitionKey = "1"
            }).ToList();
            foreach (var eventData in listEventData)
            {
                if ((batchSize + eventData.SerializedSizeInBytes) > 200000)
                {
                    // Send current batch
                    await eventHubClient.SendBatchAsync(batchList);
               
                    // Initialize a new batch
                    batchList = new List<EventData> { eventData };
                    batchSize = eventData.SerializedSizeInBytes;
                }
                else
                {
                    // Add the EventData to the current batch
                    batchList.Add(eventData);
                    batchSize += eventData.SerializedSizeInBytes;
                }
            }
            // The final batch is sent outside of the loop
            await eventHubClient.SendBatchAsync(batchList);
            
        }
        

        public void SendCellDataToStreamNotAsync(List<CellDataExtract> data)
        {
            if (data == null) return;
            if (data.Count == 0) return;
            var eventHubClient = EventHubClient.CreateFromConnectionString(CellDatasenderConnectionString, CellDataSenderHub);
            try
            {
                var batchList = new List<EventData>();
                long batchSize = 0;
                var listEventData = data.Select(p => new EventData(
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(p)))
                {
                    PartitionKey = "1"
                }).ToList();
                foreach (var eventData in listEventData)
                {
                    if ((batchSize + eventData.SerializedSizeInBytes) > 200000)
                    {
                        // Send current batch
                        eventHubClient.SendBatch(batchList);

                        // Initialize a new batch
                        batchList = new List<EventData> { eventData };
                        batchSize = eventData.SerializedSizeInBytes;
                    }
                    else
                    {
                        // Add the EventData to the current batch
                        batchList.Add(eventData);
                        batchSize += eventData.SerializedSizeInBytes;
                    }
                }
                // The final batch is sent outside of the loop
                eventHubClient.SendBatch(batchList);

                Trace.WriteLine("complete sending "+ data.Count+ " at "+ DateTime.UtcNow.ToString());
            }
            catch (Exception ex)
            {
                Trace.WriteLine("error sending " + data.Count + " cause " + ex.Message);
            }
          
        }
        
   
      


        public void ProcessXlsxFileByTask()
        {

            //var processDataFolderPath = "Z:\\kpis";
            var processDataFolderPath = FolderWillScan;
            //extract data
            var date = DateTime.UtcNow;
           
            try
            {
                 var listData = ExtractData(processDataFolderPath, date.ToString("ddMMyyyy-HHmm"), "amsfeedgenerator");
                Trace.WriteLine("extract data: " + listData.Count);

                SendCellDataToStreamNotAsync(listData);
                Trace.WriteLine("send data: " + listData.Count);
            }
            catch (Exception ex)
            {

                Trace.WriteLine("error : " + ex.Message);
            }

        }


        #region utils
        private List<string> GetFilesNameEndWithDateTime(string folderPath, string suffixName)
        {
            try
            {
                var files = Directory.GetFiles(folderPath);
                var result = files.Where(x => x.EndsWith(suffixName, StringComparison.OrdinalIgnoreCase)).ToList();
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private string GetCellInfo(string dataStr,string fieldName)
        {
            var data = dataStr.Split(',');
            var firstOrDefault = data.FirstOrDefault(x => x.Contains(fieldName));
            if (firstOrDefault != null)
            {
                var res = firstOrDefault.Split('=')[1];
                return res;
            }
            return "";
        }
        /// <summary>
        /// use only for feed data by ams generator
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="dateTime"></param>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        private List<CellDataExtract> ExtractData(string folderPath, string dateTime, string operatorId)
        {
            var result = new List<CellDataExtract>();
            var fileNameSuffix = string.Format("{0}.xlsx", dateTime);
            var fileNameFinds = GetFilesNameEndWithDateTime(folderPath, fileNameSuffix);
            foreach (var fileName in fileNameFinds)
            {
                if (File.Exists(fileName))
                {

                    using (var stream = new FileStream(fileName, FileMode.Open))
                    {
                        var data = ExtractData(stream, operatorId);
                        result.AddRange(data);
                    }
                }
            }
            return result;
        }

        #endregion
    }
}
