using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Helpers;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace BulkCsvToSqlRole
{
    public class BulkCsvSqlRequest
    {
        public string OperatorId { get; set; }


        public string JobId { get; set; }

        public string FileName { get; set; }

        /// <summary>
        /// 1 is BTS and 2 is Cell
        /// </summary>
        public int UploadType { get; set; }
    }


    public class WorkerRole : RoleEntryPoint
    {
        readonly string _queueName = ConfigurationManager.AppSettings["AmsSyncdataQueue"];

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient _client;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.


         
            _client.OnMessage((receivedMessage) =>
            {
                try
                {
                    // Process the message

                    // View the message as an OnlineOrder.
                    var messageBodyStr = receivedMessage.GetBody<string>();
                    var syncDataRequest = JsonConvert.DeserializeObject<BulkCsvSqlRequest>(messageBodyStr);

                    Trace.WriteLine("type "+ syncDataRequest.UploadType);
                    Trace.WriteLine("file " + syncDataRequest.FileName);
                    //download csv
                    //  create a block blob
                    CloudBlockBlob blockBlob = AzureHelper.CsvBlobContainer.GetBlockBlobReference(syncDataRequest.FileName);
                    if (blockBlob.Exists())
                    {
                        //blob storage uses forward slashes, windows uses backward slashes; do a replace
                        //  so localPath will be right

                        string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output",
                            blockBlob.Name.Replace(@"/", @"\"));
                        //if the directory path matching the "folders" in the blob name don't exist, create them
                        string dirPath = Path.GetDirectoryName(localPath);
                        if (!Directory.Exists(localPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }
                        blockBlob.DownloadToFile(localPath, FileMode.Create);

                        if (syncDataRequest.UploadType == 2)
                        {
                            Trace.TraceInformation("Start process Cell ");
                            var sw = new Stopwatch();
                            sw.Start();
                            SaveCellToSql(localPath,syncDataRequest.OperatorId, syncDataRequest.JobId);
                            sw.Stop();
                            Trace.TraceInformation("Complete save Cell to SQL with total millisecond: " +
                                                   sw.ElapsedMilliseconds);
                        }
                        else if (syncDataRequest.UploadType == 1)
                        {
                            Trace.TraceInformation("Start process BTS ");
                            var sw = new Stopwatch();
                            sw.Start();
                            SaveBtsToSql(localPath, syncDataRequest.OperatorId, syncDataRequest.JobId);
                            sw.Stop();
                            Trace.TraceInformation("Complete save BTS to SQL with total millisecond: " +
                                                   sw.ElapsedMilliseconds);
                        }

                    }
                    else
                    {
                        Trace.WriteLine("not exist file");
                    }
                    
                   // receivedMessage.Complete();

                }
                catch(Exception ex)
                {
                    // Handle any message processing specific exceptions here
                    Trace.TraceInformation("There is error during process message " + ex.Message);
                }
            });

            CompletedEvent.WaitOne();
        }

        private static void SaveCellToSql(string localPath, string operatorId,string jobId)
        {
            try
            {
                var sites = File.ReadAllLines(localPath)
               .Skip(1)
               .Select(s => s.ToString())
               .ToList();

                var constr =
                    ConfigurationManager.AppSettings["SqlConnection"];

                var options = new ParallelOptions() { MaxDegreeOfParallelism = 2 };
                var lockobj = new object();
                var cmdArray = new List<string>();

                var sb = new StringBuilder();

                #region CreateQuery

                sb.Append("INSERT INTO [BtsCell] ( [Id], " +
                          " [OperatorId], " +
                          " [JobId], " +
                          " [TempId], " +
                          " [BtsName], " +
                           " [CellName], " +
                          " [Technology], " +
                          " [FrequencyTechnologyBand], " +
                          " [Sector], " +
                          " [Azimuth] " +
                          " ) VALUES ");

                var count = 1;
                Parallel.ForEach(sites, options, row =>
                {
                    lock (lockobj)
                    {
                        var cell = row.Split(',');

                        var id = cell[0].Trim('"');
                        var btsName = cell[1].Trim('"');
                        var cellName = cell[2].Trim('"');

                        var tempid = cell[5].Trim('"');
                        var tech = cell[6].Trim('"');
                        var band = cell[7].Trim('"');
                        var sector = cell[8].Trim('"');
                        var azimuth = cell[9].Trim('"');
                      
                        if (count % 162 == 0)
                        {
                            sb.Append(
                                $"('{id}','{operatorId}','{jobId}','{tempid}','{btsName}','{cellName}','{tech}','{band}','{sector}','{azimuth}')");

                            cmdArray.Add(sb.ToString());
                            count = 1;

                            sb = new StringBuilder();
                            sb.Append("INSERT INTO [BtsCell] ( [Id], " +
                                      " [OperatorId], " +
                                       " [JobId], " +
                                      " [TempId], " +
                                      " [BtsName], " +
                                      " [CellName], " +
                                      " [Technology], " +
                                      " [FrequencyTechnologyBand], " +
                                      " [Sector], " +
                                      " [Azimuth] " +
                                      " ) VALUES ");
                        }
                        else
                        {
                            sb.Append(
                                $"('{id}','{operatorId}','{jobId}','{tempid}','{btsName}','{cellName}','{tech}','{band}','{sector}','{azimuth}'),");
                        }
                        count++;
                    }
                });

                cmdArray.Add(sb.ToString().TrimEnd(','));

                #endregion

                #region executeQuery

                Parallel.ForEach(cmdArray, cmdstr =>
                {
                    using (SqlConnection connection = new SqlConnection(constr))
                    {
                        SqlCommand cmd = new SqlCommand(cmdstr, connection);
                        if (connection.State == ConnectionState.Closed)
                            connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception ex)
            {

                Trace.TraceError("Error has occur during process: " + ex.InnerException);
                Trace.TraceError("And : " + ex.Message);
            }

            #endregion
        }
        private static void SaveBtsToSql(string localPath,string operatorId, string jobId)
        {
            try
            {
                var listBts = File.ReadAllLines(localPath)
               .Skip(1)
               .Select(s => s.ToString())
               .ToList();


                var constr =
                    ConfigurationManager.AppSettings["SqlConnection"];

                var sb = new StringBuilder();
                sb.Append("INSERT INTO [Bts] ([Id],[TempId],[OperatorId]," + " [JobId], " +
                " [BtsName], " +
                " [NetworkName], " +
                " [CarrierName], " +
                " [Latitude], " +
                " [Longitude] " +
                " ) VALUES ");



                for (var i = 1; i <= listBts.Count(); i++)
                {
                    var row = listBts[i - 1];
                    var cell = row.Split(',');
                    var id = cell[0].Trim('"');
                    var btsname = cell[1].Trim('"');
                    var latitude = cell[2];
                    var longitude = cell[3];

                   
                    var tempid = cell[6].Trim('"');
                    var networkname = cell[7].Trim('"');
                    var carriername = cell[8].Trim('"');

                    if (i % 999 == 0)
                    {
                        sb.Append(
                            $"('{id}','{tempid}','{operatorId}','{jobId}','{btsname}','{networkname}','{carriername}',{latitude},{longitude})");
                    }
                    else
                    {
                        sb.Append(
                            $"('{id}','{tempid}','{operatorId}','{jobId}','{btsname}','{networkname}','{carriername}',{latitude},{longitude}),");
                    }
                    if (i % 999 == 0)
                    {
                        //process then reset
                        using (SqlConnection connection = new SqlConnection(constr))
                        {
                            try
                            {
                                connection.Open();
                                SqlCommand cmd = new SqlCommand(sb.ToString(), connection);
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                throw;
                            }
                        }
                        sb = new StringBuilder();
                        if (i < listBts.Count())
                            sb.Append("INSERT INTO [Bts] ( [Id], [TempId],[OperatorId]," + " [JobId], " +
                            " [BtsName], " +
                            " [NetworkName], " +
                            " [CarrierName], " +
                            " [Latitude], " +
                            " [Longitude] " +
                            " ) VALUES ");
                    }

                }
                using (SqlConnection connection = new SqlConnection(constr))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(sb.ToString().TrimEnd(','), connection);
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {

                Trace.TraceError("Error has occur during process: " + ex.InnerException);
                Trace.TraceError("And : " + ex.Message);
            }

        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(_queueName))
            {
                namespaceManager.CreateQueue(_queueName);
            }

            // Initialize the connection to Service Bus Queue
            _client = QueueClient.CreateFromConnectionString(connectionString, _queueName,ReceiveMode.ReceiveAndDelete);
            
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            _client.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}
