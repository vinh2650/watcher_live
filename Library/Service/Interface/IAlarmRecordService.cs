using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Business;
using Core.Domain.Business.StreamAnalyticEntity;
using Core.Domain.StoreProcedure;

namespace Service.Interface
{
    /// <summary>
    /// Alram record service
    /// </summary>
    public interface IAlarmRecordService
    {

        /// <summary>
        /// export SQL to CSV and upload to blob
        /// </summary>
        void ExportToCsvAndUploadToBlob(string filePathForSave);

        void BuldSampleData();


        List<AlarmRecord> GetAllCurrentRecord();

        void InsertNewAlarm(AlarmRecord alarmRecord);

        void RemoveAlarm(AlarmRecord alarmRecord);

        AlarmRecord GetByCellName(string cellName);

        /// <summary>
        /// get all alarm record of 1 operator
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        List<SpEntity.AllAlarm> GetAllAlarmRecords(string operatorId,int? skip, int? take);

        /// <summary>
        /// get alarm histories by cell name
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="cellName"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        List<SpEntity.AlarmOrHistoryModel> GetAlarmHistoriesByCellName(string operatorId,string cellName, int? skip, int? take);


        /// <summary>
        /// get alarm histories by cell name
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="cellName"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        List<SpEntity.AlarmOrHistoryModel> GetAlarmActiveByCellName(string operatorId, string cellName, int? skip, int? take);

        /// <summary>
        /// get alarm on chart
        /// </summary>
        /// <returns></returns>
        List<SpEntity.AlarmOnChart> GetAlarmOnChart(string operatorId);

        /// <summary>
        /// get all alarm by cell name, include history alarm
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="cellName"></param>
        /// <param name="maxRow"></param>
        /// <returns></returns>
        List<SpEntity.AlarmDataByCellName> GetAllAlarmDataByCellName(string operatorId,string cellName, int maxRow);

        /// <summary>
        /// get all alarm and history records
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        List<SpEntity.AlarmOrHistoryModel> GetAllAlarmsAndHistories(string operatorId,int? skip, int? take);

        /// <summary>
        /// Extract list alarm from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="historyAlarmRecords"></param>
        /// <returns></returns>
        List<AlarmRecord> ExtractFromStream(Stream stream,out List<HistoryAlarmRecord> historyAlarmRecords);


        /// <summary>
        /// Get alarm count for list cellname group by datetime
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="cellNames"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        List<SpEntity.AlarmCounts> GetAlarmCountByTimeRange(string operatorId, List<string> cellNames,DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get all Alarm data by Keywords
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="keywords"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        List<SpEntity.AlarmDataByCellName> GetAllAlarmDataByKeywords(string operatorId, string keywords, int skip, int take);
    }
}
