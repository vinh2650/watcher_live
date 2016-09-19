using System;
using System.Collections.Generic;
using System.IO;
using Core.Domain.Business.StreamAnalyticEntity;
using StreamAnalyticService.Ondemand.Models;

namespace StreamAnalyticService.Ondemand.Interface
{
    /// <summary>
    /// Service for extract, process configuration file (xml)
    /// </summary>
    public interface IConfigurationProcessService
    {
        /// <summary>
        ///  Parse all matched xml file into list cell configuration
        /// </summary>
        /// <returns></returns>
        List<CellConfigurationExtract> ExtractCellConfigurationObject(string folderPath, string dateTime);


        void SendCellConfigurationToStream(List<CellConfigurationExtract> data);


        void ConvertFileAndUploadToBlob(string folderPath, string dateTime, TextWriter log);


        /// <summary>
        /// ConvertFileAndUploadToBlob
        /// </summary>
        /// <param name="stream"></param>
        void ConvertFileAndUploadToBlob(Stream stream);



        /// <summary>
        /// ConvertFileAndUploadToBlob
        /// </summary>
        /// <param name="allConfiguration"></param>
        /// <param name="testId"></param>
        void ConvertFileAndUploadToBlobForTesting(List<CellConfigurationExtract> allConfiguration, string testId);


        List<CellConfigurationExtract> ExtractConfigurationForTesting(List<string> configurationFilePaths, string testId);


        #region second query
     
        /// <summary>
        /// 
        /// </summary>
        void ExportConfigurationForSecondQueryAndUploadToBlob(DateTime today);


        #endregion

        #region second configuration
        
        #endregion
    }
}
