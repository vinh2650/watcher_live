using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StreamAnalyticService.Ondemand.Models;

namespace StreamAnalyticService.Ondemand.Interface
{
    /// <summary>
    /// Service for extract data from cell (xlsx file)
    /// </summary>
    public interface ICellDataProcessService
    {
        /// <summary>
        /// extract cell data from excel file
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        List<CellDataExtract> ExtractData(Stream stream,string operatorId);
      


        /// <summary>
        /// Send data to stream
        /// </summary>
        /// <returns></returns>
        Task SendCellDataToStream(List<CellDataExtract> data);

        void SendCellDataToStreamNotAsync(List<CellDataExtract> data);

        
        /// <summary>
        /// Function that only by hangfire, run every 15 minute for scan excel file and sending to eventhub
        /// </summary>
        void ProcessXlsxFileByTask();
    }

}
