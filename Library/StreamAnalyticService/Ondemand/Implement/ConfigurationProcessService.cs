using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Xml;
using Common.Helpers;
using Core.Domain.Business.StreamAnalyticEntity;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using StreamAnalyticService.Ondemand.Interface;
using StreamAnalyticService.Ondemand.Models;

namespace StreamAnalyticService.Ondemand.Implement
{
    public class ConfigurationProcessService:IConfigurationProcessService
    {
        private static readonly string senderConfigurationEventHub = ConfigurationManager.AppSettings["senderConfiguration.Path"];
        private static readonly string senderConfigurationConnectionString = ConfigurationManager.AppSettings["senderConfiguration.ConnectionString"];
        private const string Cell = "Cell";
        private const string CellId = "CellId";
        private const string Txrxmode = "TxRxMode";

        private readonly DbContext _context;
        public ConfigurationProcessService(DbContext dbContext)
        {
            _context = dbContext;
        }

        public List<CellConfigurationExtract> ExtractCellConfigurationObject(string folderPath, string dateTime)
        {
            try
            {
                var files = Directory.GetFiles(folderPath);

                var lstFileName = files.Where(x => x.EndsWith(dateTime, StringComparison.OrdinalIgnoreCase)).ToList();

                var result = new List<CellConfigurationExtract>();

                foreach (string xmlFile in lstFileName)
                {
                    if (!File.Exists(xmlFile)) continue;
                    using (XmlReader reader = XmlReader.Create(xmlFile))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals(Cell)) //read node until <Cell>
                            {
                                CellConfigurationExtract cellConfig = new CellConfigurationExtract();

                                //Read following node <CellId>
                                reader.ReadToFollowing(CellId);

                                //The next is context of node <CellId>
                                reader.Read();

                                cellConfig.CellId = reader.Value;

                                //Read following node <TxTxMode>
                                reader.ReadToFollowing(Txrxmode);

                                //The next is context of node <TxRxMode>
                                reader.Read();

                                cellConfig.TxRxMode = reader.Value;

                                result.Add(cellConfig);

                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendCellConfigurationToStream(List<CellConfigurationExtract> data)
        {
            if (data==null) return;
            if (data.Count == 0) return;
            var eventHubClient = EventHubClient.CreateFromConnectionString(senderConfigurationConnectionString, senderConfigurationEventHub);
            foreach (var cellConfig in data)
            {
                eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cellConfig))));
            }
        }

        public void ConvertFileAndUploadToBlob(string folderPath, string dateTime,TextWriter log)
        {
            var files = Directory.GetFiles(folderPath);

            #region prepare header

            var str = new StringBuilder();
            //LocalCellId

            str.Append("LocalCellId,");


            //CellName

            str.Append("CellName,");


            //<CsgInd>0</CsgInd><!--False-->

            str.Append("CsgInd,");


            //<UlCyclicPrefix>0</UlCyclicPrefix><!--Normal-->

            str.Append("UlCyclicPrefix,");


            //<DlCyclicPrefix>0</DlCyclicPrefix><!--Normal-->

            str.Append("DlCyclicPrefix,");


            //FreqBand

            str.Append("FreqBand,");


            //<UlEarfcnCfgInd>0</UlEarfcnCfgInd><!--Notconfigure-->

            str.Append("UlEarfcnCfgInd,");


            //DlEarfcn

            str.Append("DlEarfcn,");


            //<UlBandWidth>5</UlBandWidth><!--20M-->

            str.Append("UlBandWidth,");


            //<DlBandWidth>5</DlBandWidth><!--20M-->

            str.Append("DlBandWidth,");


            //CellId

            str.Append("CellId,");


            //PhyCellId

            str.Append("PhyCellId,");


            //AdditionalSpectrumEmission

            str.Append("AdditionalSpectrumEmission,");


            //<CellActiveState>1</CellActiveState><!--Active-->

            str.Append("CellActiveState,");


            //<CellAdminState>0</CellAdminState><!--Unblock-->

            str.Append("CellAdminState,");


            //<FddTddInd>0</FddTddInd><!--FDD-->

            str.Append("FddTddInd,");


            //<CellSpecificOffset>15</CellSpecificOffset><!--0dB-->

            str.Append("CellSpecificOffset,");


            //<QoffsetFreq>15</QoffsetFreq><!--0dB-->

            str.Append("QoffsetFreq,");


            //RootSequenceIdx

            str.Append("RootSequenceIdx,");


            //<HighSpeedFlag>0</HighSpeedFlag><!--Lowspeedcellflag-->

            str.Append("HighSpeedFlag,");


            //PreambleFmt

            str.Append("PreambleFmt,");


            //CellRadius

            str.Append("CellRadius,");


            //<CustomizedBandWidthCfgInd>0</CustomizedBandWidthCfgInd><!--Notconfigure-->

            str.Append("CustomizedBandWidthCfgInd,");


            //<EmergencyAreaIdCfgInd>0</EmergencyAreaIdCfgInd><!--Notconfigure-->

            str.Append("EmergencyAreaIdCfgInd,");


            //<UePowerMaxCfgInd>0</UePowerMaxCfgInd><!--Notconfigure-->

            str.Append("UePowerMaxCfgInd,");


            //<MultiRruCellFlag>0</MultiRruCellFlag><!--False-->

            str.Append("MultiRruCellFlag,");


            //<CPRICompression>0</CPRICompression><!--NoCompression-->

            str.Append("CpriCompression,");


            //<AirCellFlag>0</AirCellFlag><!--False-->

            str.Append("AirCellFlag,");


            //<CrsPortNum>1</CrsPortNum><!--1port-->

            str.Append("CrsPortNum,");



            //<TxRxMode>0</TxRxMode><!--1T1R-->

            str.Append("TxRxMode,");


            //UserLabel

            str.Append("UserLabel,");


            //<WorkMode>0</WorkMode><!--Uplinkanddownlink-->

            str.Append("WorkMode,");


            //<EuCellStandbyMode>0</EuCellStandbyMode><!--Active-->

            str.Append("EuCellStandbyMode,");



            //CellSlaveBand

            str.Append("CellSlaveBand,");


            //CnOpSharingGroupId

            str.Append("CnOpSharingGroupId,");


            //<IntraFreqRanSharingInd>1</IntraFreqRanSharingInd><!--True-->

            str.Append("IntraFreqRanSharingInd,");



            //<IntraFreqAnrInd>1</IntraFreqAnrInd><!--ALLOWED-->

            str.Append("IntraFreqAnrInd,");


            //<CellScaleInd>0</CellScaleInd><!--MACRO-->

            str.Append("CellScaleInd,");


            //FreqPriorityForAnr

            str.Append("FreqPriorityForAnr,");


            //CellRadiusStartLocation

            str.Append("CellRadiusStartLocation,");


            //objId

            str.Append("ObjId,");


            str.Append("\r\n");
            log.WriteLine("complete header");
            #endregion
            
            var lstFileName = files.Where(x => x.EndsWith(dateTime, StringComparison.OrdinalIgnoreCase)).ToList();
            var configurations = new List<CellConfigurationExtract>();
             log.WriteLine("total file "+ lstFileName.Count);
            foreach (var file in lstFileName)
            {
                //convert xml to object
             
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(file);
                if (xmlDoc.DocumentElement != null)
                {
                    XmlNodeList nodeList = xmlDoc.DocumentElement.GetElementsByTagName("Cell");
                    foreach (XmlNode node in nodeList)
                    {
                        var attributes = node.FirstChild;
                        var configurationModel = new CellConfigurationExtract();
                        try
                        {
                            var xmlElement = attributes["LocalCellId"];
                            if (xmlElement != null)
                                configurationModel.LocalCellId = xmlElement.InnerText;
                            var element = attributes["CellName"];
                            if (element != null)
                                configurationModel.CellName = element.InnerText;
                            var xmlElement1 = attributes["CsgInd"];
                            if (xmlElement1 != null)
                                configurationModel.CsgInd = xmlElement1.InnerText;
                            var element1 = attributes["UlCyclicPrefix"];
                            if (element1 != null)
                                configurationModel.UlCyclicPrefix = element1.InnerText;
                            var xmlElement2 = attributes["DlCyclicPrefix"];
                            if (xmlElement2 != null)
                                configurationModel.DlCyclicPrefix = xmlElement2.InnerText;

                            configurationModel.FreqBand = attributes["FreqBand"]?.InnerText ?? "";
                            configurationModel.UlEarfcnCfgInd = attributes["UlEarfcnCfgInd"]?.InnerText ?? "";
                            configurationModel.DlEarfcn = attributes["DlEarfcn"]?.InnerText ?? "";
                            configurationModel.UlBandWidth = attributes["UlBandWidth"]?.InnerText ?? "";
                            configurationModel.DlBandWidth = attributes["DlBandWidth"]?.InnerText ?? "";
                            configurationModel.CellId = attributes["CellId"]?.InnerText ?? "";
                            configurationModel.PhyCellId = attributes["PhyCellId"]?.InnerText ?? "";
                            configurationModel.AdditionalSpectrumEmission = attributes["AdditionalSpectrumEmission"]?.InnerText ?? "";
                            configurationModel.CellActiveState = attributes["CellActiveState"]?.InnerText ?? "";
                            configurationModel.CellAdminState = attributes["CellAdminState"]?.InnerText ?? "";
                            configurationModel.FddTddInd = attributes["FddTddInd"]?.InnerText ?? "";
                            configurationModel.CellSpecificOffset = attributes["CellSpecificOffset"]?.InnerText ?? "";
                            configurationModel.QoffsetFreq = attributes["QoffsetFreq"]?.InnerText ?? "";
                            configurationModel.RootSequenceIdx = attributes["RootSequenceIdx"]?.InnerText ?? "";
                            configurationModel.HighSpeedFlag = attributes["HighSpeedFlag"]?.InnerText ?? "";
                            configurationModel.PreambleFmt = attributes["PreambleFmt"]?.InnerText ?? "";
                            configurationModel.CellRadius = attributes["CellRadius"]?.InnerText ?? "";
                            configurationModel.CustomizedBandWidthCfgInd = attributes["CustomizedBandWidthCfgInd"]?.InnerText ?? "";
                            configurationModel.EmergencyAreaIdCfgInd = attributes["EmergencyAreaIdCfgInd"]?.InnerText ?? "";
                            configurationModel.UePowerMaxCfgInd = attributes["UePowerMaxCfgInd"]?.InnerText ?? "";
                            configurationModel.MultiRruCellFlag = attributes["MultiRruCellFlag"]?.InnerText ?? "";
                            configurationModel.CpriCompression = attributes["CPRICompression"]?.InnerText ?? "";
                            configurationModel.AirCellFlag = attributes["AirCellFlag"]?.InnerText ?? "";
                            configurationModel.CrsPortNum = attributes["CrsPortNum"]?.InnerText ?? "";
                            configurationModel.TxRxMode = attributes["TxRxMode"]?.InnerText ?? "";
                            configurationModel.UserLabel = attributes["UserLabel"]?.InnerText ?? "";
                            configurationModel.WorkMode = attributes["WorkMode"]?.InnerText ?? "";
                            configurationModel.EuCellStandbyMode = attributes["EuCellStandbyMode"]?.InnerText ?? "";
                            configurationModel.CellSlaveBand = attributes["CellSlaveBand"]?.InnerText ?? "";
                            configurationModel.CnOpSharingGroupId = attributes["CnOpSharingGroupId"]?.InnerText ?? "";
                            configurationModel.IntraFreqRanSharingInd = attributes["IntraFreqRanSharingInd"]?.InnerText ?? "";
                            configurationModel.IntraFreqAnrInd = attributes["IntraFreqAnrInd"]?.InnerText ?? "";
                            configurationModel.CellScaleInd = attributes["CellScaleInd"]?.InnerText ?? "";
                            configurationModel.FreqPriorityForAnr = attributes["FreqPriorityForAnr"]?.InnerText ?? "";
                            configurationModel.CellRadiusStartLocation = attributes["CellRadiusStartLocation"]?.InnerText ?? "";
                            configurationModel.ObjId = attributes["objId"]?.InnerText ?? "";
                        }
                        catch (Exception ex)
                        {
                            
                            log.WriteLine("null obj here");
                        }
                        if(string.IsNullOrEmpty(configurationModel.CellName)) continue;
                        
                        if (configurations.FirstOrDefault(p => p.CellName == configurationModel.CellName) == null)
                        {
                            configurations.Add(configurationModel);
                        }
                    }
                }
            }
            log.WriteLine("complete parse");
            #region bind data

            foreach (var item in configurations)
            {
                //LocalCellId

                str.Append("\"" + item.LocalCellId + "\",");


                //CellName

                str.Append("\"" + item.CellName + "\",");


                //<CsgInd>0</CsgInd><!--False-->

                str.Append("\"" + item.CsgInd + "\",");


                //<UlCyclicPrefix>0</UlCyclicPrefix><!--Normal-->

                str.Append("\"" + item.UlCyclicPrefix + "\",");


                //<DlCyclicPrefix>0</DlCyclicPrefix><!--Normal-->

                str.Append("\"" + item.DlCyclicPrefix + "\",");


                //FreqBand

                str.Append("\"" + item.FreqBand + "\",");


                //<UlEarfcnCfgInd>0</UlEarfcnCfgInd><!--Notconfigure-->

                str.Append("\"" + item.UlEarfcnCfgInd + "\",");


                //DlEarfcn

                str.Append("\"" + item.DlEarfcn + "\",");


                //<UlBandWidth>5</UlBandWidth><!--20M-->

                str.Append("\"" + item.UlBandWidth + "\",");


                //<DlBandWidth>5</DlBandWidth><!--20M-->

                str.Append("\"" + item.DlBandWidth + "\",");


                //CellId

                str.Append("\"" + item.CellId + "\",");


                //PhyCellId

                str.Append("\"" + item.PhyCellId + "\",");


                //AdditionalSpectrumEmission

                str.Append("\"" + item.AdditionalSpectrumEmission + "\",");


                //<CellActiveState>1</CellActiveState><!--Active-->

                str.Append("\"" + item.CellActiveState + "\",");


                //<CellAdminState>0</CellAdminState><!--Unblock-->

                str.Append("\"" + item.CellAdminState + "\",");


                //<FddTddInd>0</FddTddInd><!--FDD-->

                str.Append("\"" + item.FddTddInd + "\",");


                //<CellSpecificOffset>15</CellSpecificOffset><!--0dB-->

                str.Append("\"" + item.CellSpecificOffset + "\",");


                //<QoffsetFreq>15</QoffsetFreq><!--0dB-->

                str.Append("\"" + item.QoffsetFreq + "\",");


                //RootSequenceIdx

                str.Append("\"" + item.RootSequenceIdx + "\",");


                //<HighSpeedFlag>0</HighSpeedFlag><!--Lowspeedcellflag-->

                str.Append("\"" + item.HighSpeedFlag + "\",");


                //PreambleFmt

                str.Append("\"" + item.PreambleFmt + "\",");


                //CellRadius

                str.Append("\"" + item.CellRadius + "\",");


                //<CustomizedBandWidthCfgInd>0</CustomizedBandWidthCfgInd><!--Notconfigure-->

                str.Append("\"" + item.CustomizedBandWidthCfgInd + "\",");


                //<EmergencyAreaIdCfgInd>0</EmergencyAreaIdCfgInd><!--Notconfigure-->

                str.Append("\"" + item.EmergencyAreaIdCfgInd + "\",");


                //<UePowerMaxCfgInd>0</UePowerMaxCfgInd><!--Notconfigure-->

                str.Append("\"" + item.UePowerMaxCfgInd + "\",");


                //<MultiRruCellFlag>0</MultiRruCellFlag><!--False-->

                str.Append("\"" + item.MultiRruCellFlag + "\",");


                //<CPRICompression>0</CPRICompression><!--NoCompression-->

                str.Append("\"" + item.CpriCompression + "\",");


                //<AirCellFlag>0</AirCellFlag><!--False-->

                str.Append("\"" + item.AirCellFlag + "\",");


                //<CrsPortNum>1</CrsPortNum><!--1port-->

                str.Append("\"" + item.CrsPortNum + "\",");



                //<TxRxMode>0</TxRxMode><!--1T1R-->

                str.Append("\"" + item.TxRxMode + "\",");


                //UserLabel

                str.Append("\"" + item.UserLabel + "\",");


                //<WorkMode>0</WorkMode><!--Uplinkanddownlink-->

                str.Append("\"" + item.WorkMode + "\",");


                //<EuCellStandbyMode>0</EuCellStandbyMode><!--Active-->

                str.Append("\"" + item.EuCellStandbyMode + "\",");



                //CellSlaveBand

                str.Append("\"" + item.CellSlaveBand + "\",");


                //CnOpSharingGroupId

                str.Append("\"" + item.CnOpSharingGroupId + "\",");


                //<IntraFreqRanSharingInd>1</IntraFreqRanSharingInd><!--True-->

                str.Append("\"" + item.IntraFreqRanSharingInd + "\",");



                //<IntraFreqAnrInd>1</IntraFreqAnrInd><!--ALLOWED-->

                str.Append("\"" + item.IntraFreqAnrInd + "\",");


                //<CellScaleInd>0</CellScaleInd><!--MACRO-->

                str.Append("\"" + item.CellScaleInd + "\",");


                //FreqPriorityForAnr

                str.Append("\"" + item.FreqPriorityForAnr + "\",");


                //CellRadiusStartLocation

                str.Append("\"" + item.CellRadiusStartLocation + "\",");
                //objId
                str.Append("\"" + item.ObjId + "\",");
                str.Append("\r\n");
            }
            log.WriteLine("complete binding");
            #endregion

            FileStream fs = new FileStream(folderPath+"configuration.csv", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(Encoding.UTF8.GetBytes(str.ToString()));
            bw.Close();
            log.WriteLine("complete saving");

            // Retrieve reference to a blob
            var blobContainer = AzureHelper.AlarmCloudBlobContainer;
       

            var blob = blobContainer.GetBlockBlobReference("configuration.csv");

            // Set the blob content type
            blob.Properties.ContentType = "text/csv";

            // Upload file into blob storage, basically copying it from local disk into Azure
            using (var fs2 = File.OpenRead(folderPath + "configuration.csv"))
            {
                blob.UploadFromStream(fs2);
            }

        }


        public void ConvertFileAndUploadToBlob(Stream stream)
        {

            #region prepare header

            var str = new StringBuilder();
            //LocalCellId

            str.Append("LocalCellId,");


            //CellName

            str.Append("CellName,");


            //<CsgInd>0</CsgInd><!--False-->

            str.Append("CsgInd,");


            //<UlCyclicPrefix>0</UlCyclicPrefix><!--Normal-->

            str.Append("UlCyclicPrefix,");


            //<DlCyclicPrefix>0</DlCyclicPrefix><!--Normal-->

            str.Append("DlCyclicPrefix,");


            //FreqBand

            str.Append("FreqBand,");


            //<UlEarfcnCfgInd>0</UlEarfcnCfgInd><!--Notconfigure-->

            str.Append("UlEarfcnCfgInd,");


            //DlEarfcn

            str.Append("DlEarfcn,");


            //<UlBandWidth>5</UlBandWidth><!--20M-->

            str.Append("UlBandWidth,");


            //<DlBandWidth>5</DlBandWidth><!--20M-->

            str.Append("DlBandWidth,");


            //CellId

            str.Append("CellId,");


            //PhyCellId

            str.Append("PhyCellId,");


            //AdditionalSpectrumEmission

            str.Append("AdditionalSpectrumEmission,");


            //<CellActiveState>1</CellActiveState><!--Active-->

            str.Append("CellActiveState,");


            //<CellAdminState>0</CellAdminState><!--Unblock-->

            str.Append("CellAdminState,");


            //<FddTddInd>0</FddTddInd><!--FDD-->

            str.Append("FddTddInd,");


            //<CellSpecificOffset>15</CellSpecificOffset><!--0dB-->

            str.Append("CellSpecificOffset,");


            //<QoffsetFreq>15</QoffsetFreq><!--0dB-->

            str.Append("QoffsetFreq,");


            //RootSequenceIdx

            str.Append("RootSequenceIdx,");


            //<HighSpeedFlag>0</HighSpeedFlag><!--Lowspeedcellflag-->

            str.Append("HighSpeedFlag,");


            //PreambleFmt

            str.Append("PreambleFmt,");


            //CellRadius

            str.Append("CellRadius,");


            //<CustomizedBandWidthCfgInd>0</CustomizedBandWidthCfgInd><!--Notconfigure-->

            str.Append("CustomizedBandWidthCfgInd,");


            //<EmergencyAreaIdCfgInd>0</EmergencyAreaIdCfgInd><!--Notconfigure-->

            str.Append("EmergencyAreaIdCfgInd,");


            //<UePowerMaxCfgInd>0</UePowerMaxCfgInd><!--Notconfigure-->

            str.Append("UePowerMaxCfgInd,");


            //<MultiRruCellFlag>0</MultiRruCellFlag><!--False-->

            str.Append("MultiRruCellFlag,");


            //<CPRICompression>0</CPRICompression><!--NoCompression-->

            str.Append("CpriCompression,");


            //<AirCellFlag>0</AirCellFlag><!--False-->

            str.Append("AirCellFlag,");


            //<CrsPortNum>1</CrsPortNum><!--1port-->

            str.Append("CrsPortNum,");



            //<TxRxMode>0</TxRxMode><!--1T1R-->

            str.Append("TxRxMode,");


            //UserLabel

            str.Append("UserLabel,");


            //<WorkMode>0</WorkMode><!--Uplinkanddownlink-->

            str.Append("WorkMode,");


            //<EuCellStandbyMode>0</EuCellStandbyMode><!--Active-->

            str.Append("EuCellStandbyMode,");



            //CellSlaveBand

            str.Append("CellSlaveBand,");


            //CnOpSharingGroupId

            str.Append("CnOpSharingGroupId,");


            //<IntraFreqRanSharingInd>1</IntraFreqRanSharingInd><!--True-->

            str.Append("IntraFreqRanSharingInd,");



            //<IntraFreqAnrInd>1</IntraFreqAnrInd><!--ALLOWED-->

            str.Append("IntraFreqAnrInd,");


            //<CellScaleInd>0</CellScaleInd><!--MACRO-->

            str.Append("CellScaleInd,");


            //FreqPriorityForAnr

            str.Append("FreqPriorityForAnr,");


            //CellRadiusStartLocation

            str.Append("CellRadiusStartLocation,");


            //objId

            str.Append("ObjId,");


            str.Append("\r\n");

            #endregion

            //convert xml to object

            List<CellConfigurationExtract> configurations = new List<CellConfigurationExtract>();
            
            XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(stream);
                if (xmlDoc.DocumentElement != null)
                {
                    XmlNodeList nodeList = xmlDoc.DocumentElement.GetElementsByTagName("Cell");
                    foreach (XmlNode node in nodeList)
                    {
                    
                    var attributes = node.FirstChild;
                       
                        try
                        {
                        var configurationModel = new CellConfigurationExtract();

                        var xmlElement = attributes["LocalCellId"];
                            if (xmlElement != null)
                                configurationModel.LocalCellId = xmlElement.InnerText;
                            var element = attributes["CellName"];
                            if (element != null)
                                configurationModel.CellName = element.InnerText;
                            //var xmlElement1 = attributes["CsgInd"];
                            //if (xmlElement1 != null)
                            //    configurationModel.CsgInd = xmlElement1.InnerText;
                            //var element1 = attributes["UlCyclicPrefix"];
                            //if (element1 != null)
                            //    configurationModel.UlCyclicPrefix = element1.InnerText;
                            //var xmlElement2 = attributes["DlCyclicPrefix"];
                            //if (xmlElement2 != null)
                            //    configurationModel.DlCyclicPrefix = xmlElement2.InnerText;

                            //configurationModel.FreqBand = attributes["FreqBand"]?.InnerText ?? "";
                            //configurationModel.UlEarfcnCfgInd = attributes["UlEarfcnCfgInd"]?.InnerText ?? "";
                            //configurationModel.DlEarfcn = attributes["DlEarfcn"]?.InnerText ?? "";
                            //configurationModel.UlBandWidth = attributes["UlBandWidth"]?.InnerText ?? "";
                            //configurationModel.DlBandWidth = attributes["DlBandWidth"]?.InnerText ?? "";
                            //configurationModel.CellId = attributes["CellId"]?.InnerText ?? "";
                            //configurationModel.PhyCellId = attributes["PhyCellId"]?.InnerText ?? "";
                            //configurationModel.AdditionalSpectrumEmission = attributes["AdditionalSpectrumEmission"]?.InnerText ?? "";
                            //configurationModel.CellActiveState = attributes["CellActiveState"]?.InnerText ?? "";
                            //configurationModel.CellAdminState = attributes["CellAdminState"]?.InnerText ?? "";
                            //configurationModel.FddTddInd = attributes["FddTddInd"]?.InnerText ?? "";
                            //configurationModel.CellSpecificOffset = attributes["CellSpecificOffset"]?.InnerText ?? "";
                            //configurationModel.QoffsetFreq = attributes["QoffsetFreq"]?.InnerText ?? "";
                            //configurationModel.RootSequenceIdx = attributes["RootSequenceIdx"]?.InnerText ?? "";
                            //configurationModel.HighSpeedFlag = attributes["HighSpeedFlag"]?.InnerText ?? "";
                            //configurationModel.PreambleFmt = attributes["PreambleFmt"]?.InnerText ?? "";
                            //configurationModel.CellRadius = attributes["CellRadius"]?.InnerText ?? "";
                            //configurationModel.CustomizedBandWidthCfgInd = attributes["CustomizedBandWidthCfgInd"]?.InnerText ?? "";
                            //configurationModel.EmergencyAreaIdCfgInd = attributes["EmergencyAreaIdCfgInd"]?.InnerText ?? "";
                            //configurationModel.UePowerMaxCfgInd = attributes["UePowerMaxCfgInd"]?.InnerText ?? "";
                            //configurationModel.MultiRruCellFlag = attributes["MultiRruCellFlag"]?.InnerText ?? "";
                            //configurationModel.CpriCompression = attributes["CPRICompression"]?.InnerText ?? "";
                            //configurationModel.AirCellFlag = attributes["AirCellFlag"]?.InnerText ?? "";
                            //configurationModel.CrsPortNum = attributes["CrsPortNum"]?.InnerText ?? "";
                            configurationModel.TxRxMode = attributes["TxRxMode"]?.InnerText ?? "";
                            //configurationModel.UserLabel = attributes["UserLabel"]?.InnerText ?? "";
                            //configurationModel.WorkMode = attributes["WorkMode"]?.InnerText ?? "";
                            //configurationModel.EuCellStandbyMode = attributes["EuCellStandbyMode"]?.InnerText ?? "";
                            //configurationModel.CellSlaveBand = attributes["CellSlaveBand"]?.InnerText ?? "";
                            //configurationModel.CnOpSharingGroupId = attributes["CnOpSharingGroupId"]?.InnerText ?? "";
                            //configurationModel.IntraFreqRanSharingInd = attributes["IntraFreqRanSharingInd"]?.InnerText ?? "";
                            //configurationModel.IntraFreqAnrInd = attributes["IntraFreqAnrInd"]?.InnerText ?? "";
                            //configurationModel.CellScaleInd = attributes["CellScaleInd"]?.InnerText ?? "";
                            //configurationModel.FreqPriorityForAnr = attributes["FreqPriorityForAnr"]?.InnerText ?? "";
                            //configurationModel.CellRadiusStartLocation = attributes["CellRadiusStartLocation"]?.InnerText ?? "";
                            //configurationModel.ObjId = attributes["objId"]?.InnerText ?? "";


                            configurations.Add(configurationModel);
                        }
                        catch (Exception ex)
                        {

                        }
                        
                    }
                }


            foreach (var configurationModel in configurations)
            {
                var item = configurationModel;
                #region bind data

                //LocalCellId

                str.Append("\"" + item.LocalCellId + "\",");


                //CellName

                str.Append("\"" + item.CellName + "\",");


                //<CsgInd>0</CsgInd><!--False-->

                str.Append("\"" + item.CsgInd + "\",");


                //<UlCyclicPrefix>0</UlCyclicPrefix><!--Normal-->

                str.Append("\"" + item.UlCyclicPrefix + "\",");


                //<DlCyclicPrefix>0</DlCyclicPrefix><!--Normal-->

                str.Append("\"" + item.DlCyclicPrefix + "\",");


                //FreqBand

                str.Append("\"" + item.FreqBand + "\",");


                //<UlEarfcnCfgInd>0</UlEarfcnCfgInd><!--Notconfigure-->

                str.Append("\"" + item.UlEarfcnCfgInd + "\",");


                //DlEarfcn

                str.Append("\"" + item.DlEarfcn + "\",");


                //<UlBandWidth>5</UlBandWidth><!--20M-->

                str.Append("\"" + item.UlBandWidth + "\",");


                //<DlBandWidth>5</DlBandWidth><!--20M-->

                str.Append("\"" + item.DlBandWidth + "\",");


                //CellId

                str.Append("\"" + item.CellId + "\",");


                //PhyCellId

                str.Append("\"" + item.PhyCellId + "\",");


                //AdditionalSpectrumEmission

                str.Append("\"" + item.AdditionalSpectrumEmission + "\",");


                //<CellActiveState>1</CellActiveState><!--Active-->

                str.Append("\"" + item.CellActiveState + "\",");


                //<CellAdminState>0</CellAdminState><!--Unblock-->

                str.Append("\"" + item.CellAdminState + "\",");


                //<FddTddInd>0</FddTddInd><!--FDD-->

                str.Append("\"" + item.FddTddInd + "\",");


                //<CellSpecificOffset>15</CellSpecificOffset><!--0dB-->

                str.Append("\"" + item.CellSpecificOffset + "\",");


                //<QoffsetFreq>15</QoffsetFreq><!--0dB-->

                str.Append("\"" + item.QoffsetFreq + "\",");


                //RootSequenceIdx

                str.Append("\"" + item.RootSequenceIdx + "\",");


                //<HighSpeedFlag>0</HighSpeedFlag><!--Lowspeedcellflag-->

                str.Append("\"" + item.HighSpeedFlag + "\",");


                //PreambleFmt

                str.Append("\"" + item.PreambleFmt + "\",");


                //CellRadius

                str.Append("\"" + item.CellRadius + "\",");


                //<CustomizedBandWidthCfgInd>0</CustomizedBandWidthCfgInd><!--Notconfigure-->

                str.Append("\"" + item.CustomizedBandWidthCfgInd + "\",");


                //<EmergencyAreaIdCfgInd>0</EmergencyAreaIdCfgInd><!--Notconfigure-->

                str.Append("\"" + item.EmergencyAreaIdCfgInd + "\",");


                //<UePowerMaxCfgInd>0</UePowerMaxCfgInd><!--Notconfigure-->

                str.Append("\"" + item.UePowerMaxCfgInd + "\",");


                //<MultiRruCellFlag>0</MultiRruCellFlag><!--False-->

                str.Append("\"" + item.MultiRruCellFlag + "\",");


                //<CPRICompression>0</CPRICompression><!--NoCompression-->

                str.Append("\"" + item.CpriCompression + "\",");


                //<AirCellFlag>0</AirCellFlag><!--False-->

                str.Append("\"" + item.AirCellFlag + "\",");


                //<CrsPortNum>1</CrsPortNum><!--1port-->

                str.Append("\"" + item.CrsPortNum + "\",");



                //<TxRxMode>0</TxRxMode><!--1T1R-->

                str.Append("\"" + item.TxRxMode + "\",");


                //UserLabel

                str.Append("\"" + item.UserLabel + "\",");


                //<WorkMode>0</WorkMode><!--Uplinkanddownlink-->

                str.Append("\"" + item.WorkMode + "\",");


                //<EuCellStandbyMode>0</EuCellStandbyMode><!--Active-->

                str.Append("\"" + item.EuCellStandbyMode + "\",");



                //CellSlaveBand

                str.Append("\"" + item.CellSlaveBand + "\",");


                //CnOpSharingGroupId

                str.Append("\"" + item.CnOpSharingGroupId + "\",");


                //<IntraFreqRanSharingInd>1</IntraFreqRanSharingInd><!--True-->

                str.Append("\"" + item.IntraFreqRanSharingInd + "\",");



                //<IntraFreqAnrInd>1</IntraFreqAnrInd><!--ALLOWED-->

                str.Append("\"" + item.IntraFreqAnrInd + "\",");


                //<CellScaleInd>0</CellScaleInd><!--MACRO-->

                str.Append("\"" + item.CellScaleInd + "\",");


                //FreqPriorityForAnr

                str.Append("\"" + item.FreqPriorityForAnr + "\",");


                //CellRadiusStartLocation

                str.Append("\"" + item.CellRadiusStartLocation + "\",");
                //objId
                str.Append("\"" + item.ObjId + "\",");
                str.Append("\r\n");

                #endregion
            }

            var path = "/App_Data/CellConfiguration/";
           

            if (HostingEnvironment.IsHosted)
            {
                //hosted
                path = HostingEnvironment.MapPath(path);
            }
            else
            {
                //not hosted. For example, run in unit tests
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                path = Path.Combine(baseDirectory, path);
            }

            FileStream fs = new FileStream(path + "configurationtest.csv", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(Encoding.UTF8.GetBytes(str.ToString()));
            bw.Close();

            // Retrieve reference to a blob
            var blobContainer = AzureHelper.AlarmCloudBlobContainer;


            var blob = blobContainer.GetBlockBlobReference("configuration.csv");

            // Set the blob content type
            blob.Properties.ContentType = "text/csv";

            // Upload file into blob storage, basically copying it from local disk into Azure
            using (var fs2 = File.OpenRead(path + "configurationtest.csv"))
            {
                blob.UploadFromStream(fs2);
            }

        }

      

        private void PrepareDataForConfiguration(StringBuilder str ,string testId,List<CellConfigurationExtract> configurations)
        {
           foreach (var configurationModel in configurations)
            {
                var item = configurationModel;

                #region bind data

                str.Append("\"" + testId + "\",");
                //LocalCellId

                str.Append("\"" + item.LocalCellId + "\",");


                //CellName

                str.Append("\"" + item.CellName +  "\",");


                //<TxRxMode>0</TxRxMode><!--1T1R-->

                str.Append("\"" + item.TxRxMode + "\",");


                str.Append("\r\n");

                #endregion
            }
        }

        private void PrepareHeaderForConfiguration(StringBuilder str)
        {
            #region prepare header

            str.Append("TestId,");
            //LocalCellId

            str.Append("LocalCellId,");
            //CellName

            str.Append("CellName,");


            str.Append("TxRxMode,");


            str.Append("\r\n");

            #endregion
        }

        private  List<CellConfigurationExtract> ExtractConfigurationFromXmls(Stream stream)
        {
            var configurations = new List<CellConfigurationExtract>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);
            if (xmlDoc.DocumentElement != null)
            {
                XmlNodeList nodeList = xmlDoc.DocumentElement.GetElementsByTagName("Cell");
                foreach (XmlNode node in nodeList)
                {
                    var attributes = node.FirstChild;

                    try
                    {
                        var configurationModel = new CellConfigurationExtract();

                        var xmlElement = attributes["LocalCellId"];
                        if (xmlElement != null)
                            configurationModel.LocalCellId = xmlElement.InnerText;
                        var element = attributes["CellName"];
                        if (element != null)
                            configurationModel.CellName = element.InnerText;

                        configurationModel.TxRxMode = attributes["TxRxMode"]?.InnerText ?? "";


                        configurations.Add(configurationModel);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return configurations;
        }

       
        public void ConvertFileAndUploadToBlobForTesting(List<CellConfigurationExtract> allConfiguration, string testId)
        {
            var finalStringBuilder = new StringBuilder();
            
            //prepare header
            PrepareHeaderForConfiguration(finalStringBuilder);
            //prepare data
            PrepareDataForConfiguration(finalStringBuilder,testId, allConfiguration);

            var path = "/App_Data/CellConfiguration/";


            if (HostingEnvironment.IsHosted)
            {
                //hosted
                path = HostingEnvironment.MapPath(path);
            }
            else
            {
                //not hosted. For example, run in unit tests
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                path = Path.Combine(baseDirectory, path);
            }

            FileStream fs = new FileStream(path + testId + "_configuration.csv", FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(Encoding.UTF8.GetBytes(finalStringBuilder.ToString()));
            bw.Close();
            for (var i = 0; i < 3; i++)
            {
                // Retrieve reference to a blob
                var blobContainer = AzureHelper.AlarmCloudBlobContainer;

                var datetime = DateTime.UtcNow.AddMinutes(i);
                var blobName = string.Format("{0}/{1}/configuration.csv", datetime.ToString("dd-MM-yyyy"), datetime.ToString("HH-mm"));
                var blob = blobContainer.GetBlockBlobReference(blobName);
                // Set the blob content type
                blob.Properties.ContentType = "text/csv";

                // Upload file into blob storage, basically copying it from local disk into Azure
                using (var fs2 = File.OpenRead(path + testId + "_configuration.csv"))
                {
                    blob.UploadFromStream(fs2);
                }
            }

        }

        public List<CellConfigurationExtract> ExtractConfigurationForTesting(List<string> configurationFilePaths, string testId)
        {
            var allConfiguration = new List<CellConfigurationExtract>();
            foreach (var filePath in configurationFilePaths)
            {
                var stream = File.OpenRead(filePath);
                allConfiguration.AddRange(ExtractConfigurationFromXmls(stream));
            }
            return allConfiguration;
        }

     
        private void SaveToDatabaseForConfigurationOfPwrSwitchAndAtten(List<CellConfigurationForPwrSwitchAndAtten> configurations)
        {
            if (configurations == null || !configurations.Any()) return;
            string constr = ConfigurationManager.ConnectionStrings["AmsContext"].ConnectionString;
            var sb = new StringBuilder();
            sb.Append("INSERT INTO [CellConfigurationForPwrSwitchAndAtten] ( [Id], " +
                      " [CellName], " +
                      " [CnSrnSnRn], " +
                      
                      " [ATTEN], " +
                      " [PWRSWITCH], " +
                      " [CreatedDateUtc] " +
                      " ) VALUES ");
            for (var i = 1; i <= configurations.Count(); i++)
            {
                var configuration = configurations[i-1];
                if (i % 1000 == 0)
                {
                    sb.Append(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}')", configuration.Id,
                   configuration.CellName,configuration.CnSrnSnRn,configuration.ATTEN,configuration.PWRSWITCH,configuration.CreatedDateUtc.ToString("yyyy-MM-dd'T'HH:mm:ss.FFF'Z'")));
                }
                else
                {
                    sb.Append(string.Format("('{0}','{1}','{2}','{3}','{4}','{5}'),", configuration.Id,
                   configuration.CellName, configuration.CnSrnSnRn, configuration.ATTEN, configuration.PWRSWITCH, configuration.CreatedDateUtc.ToString("yyyy-MM-dd'T'HH:mm:ss.FFF'Z'")));
                }


                if (i % 1000 == 0)
                {
                    //process then reset
                    using (SqlConnection connection = new SqlConnection(constr))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand(sb.ToString(), connection);
                        var sw1 = new Stopwatch();
                        sw1.Start();
                        cmd.ExecuteNonQuery();
                        sw1.Stop();
                        Trace.TraceError("time for insert 1k records:"+ sw1.ElapsedMilliseconds);
                        
                    }
                    sb = new StringBuilder();
                    if (i < configurations.Count())
                        sb.Append("INSERT INTO [CellConfigurationForPwrSwitchAndAtten] ( [Id], " +
                      " [CellName], " +
                      " [CnSrnSnRn], " +
                      " [PWRSWITCH], " +
                      " [ATTEN], " +
                      " [CreatedDateUtc] " +
                      " ) VALUES ");
                }

            }


            using (SqlConnection connection = new SqlConnection(constr))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand(sb.ToString().TrimEnd(','), connection);
                
                var sw2 = new Stopwatch();
                sw2.Start();
                cmd.ExecuteNonQuery();
                sw2.Stop();
                Trace.TraceError("time for insert last 1k records:" + sw2.ElapsedMilliseconds);
            }
        }

        private void SaveToDatabaseForConfigurationOfSecondQuery(DateTime today,DateTime yesterday)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("PrepareConfigurationDataForSecondQuery @startdate, @enddate", 
                    new SqlParameter("@startdate", today.ToString("yyyy-MM-dd HH:mm:ss.FFF")),
                    new SqlParameter("@enddate", yesterday.ToString("yyyy-MM-dd HH:mm:ss.FFF")));

            }
            catch (Exception ex)
            {
                
                throw ex;
            }

            }
      
        public void ExportConfigurationForSecondQueryAndUploadToBlob(DateTime today)
        {
            var guid = Guid.NewGuid().ToString();
            var fileNamePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, guid + "-secondconfiguration.csv");

            string constr = ConfigurationManager.ConnectionStrings["AmsContext"].ConnectionString;

            SqlConnection spContentConn = new SqlConnection(constr);
            SqlCommand sqlcmd = new SqlCommand();
            StreamWriter CsvfileWriter = new StreamWriter(fileNamePath);
            sqlcmd.Connection = spContentConn;
            sqlcmd.CommandTimeout = 0;
            sqlcmd.CommandType = CommandType.Text;
            string sqlselectQuery = "SELECT CellName,AdditionInfoForAtten,AdditionInfoForPwr,AllAttenDifferent,AllAttenSame,AllPwrDifferent,AllPwrSame FROM CellConfigurationSecondQuery Where CreatedDateUtc = '" + today.ToString("yyyy-MM-dd HH:mm:ss.FFF")+"'";
            sqlcmd.CommandText = sqlselectQuery;
            spContentConn.Open();
            using (spContentConn)
            {
                using (SqlDataReader sdr = sqlcmd.ExecuteReader())
                using (CsvfileWriter) 
                {
                    //This Block of code for getting the Table Headers
                    DataTable Tablecolumns = new DataTable();

                    for (int i = 0; i < sdr.FieldCount; i++)
                    {
                        Tablecolumns.Columns.Add(sdr.GetName(i));
                    }
                    Tablecolumns.Columns.Add("CreatedDateUtc");
                    CsvfileWriter.WriteLine(string.Join(",", Tablecolumns.Columns.Cast<DataColumn>().Select(csvfile => csvfile.ColumnName)));
                    while (sdr.Read())
                    {
                        CsvfileWriter.WriteLine(sdr[0].ToString() + "," + "\"" + sdr[1].ToString() + "\"" + ","
                            + "\"" + sdr[2].ToString() + "\"" + "," + sdr[3].ToString()
                       + "," + sdr[4].ToString() + "," + sdr[5].ToString() + "," + sdr[6].ToString() + "," + today.ToString("yyyy-MM-dd'T'HH:mm:ss.FFF'Z'") + ",");
                    }
                }
            }
            spContentConn.Close();
            // Retrieve reference to a blob
            var blobContainer = AzureHelper.AlarmCloudBlobContainer;
            var blobName = "secondconfiguration.csv";
            var blob = blobContainer.GetBlockBlobReference(blobName);

            // Set the blob content type
            blob.Properties.ContentType = "text/csv";

            // Upload file into blob storage, basically copying it from local disk into Azure
            using (var fs2 = File.OpenRead(fileNamePath))
            {
                blob.UploadFromStream(fs2);
            }
        }

       
    }



   
}
