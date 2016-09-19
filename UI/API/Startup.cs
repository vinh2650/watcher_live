using System;
using System.Collections.Generic;
using System.Configuration;
using Hangfire;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using API.App_Start;
using Autofac;
using Autofac.Integration.WebApi;
using Hangfire.Dashboard;
using Hangfire.Redis;
using Microsoft.ServiceBus.Messaging;
using GlobalConfiguration = Hangfire.GlobalConfiguration;

[assembly: OwinStartup(typeof(API.Startup))]

namespace API
{
    public partial class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        public static IContainer Container { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            var httpConfig = new HttpConfiguration();
            //register DI here
            var builder = AutofacConfig.Configuration(app);
            Container = builder.Build();

            var resolver = new AutofacWebApiDependencyResolver(Container);
            httpConfig.DependencyResolver = resolver;

            ConfigureAuth(app, resolver);
            app.UseAutofacMiddleware(Container);
            app.UseAutofacWebApi(httpConfig);
            app.UseWebApi(httpConfig);
            WebApiConfig.Register(httpConfig);

            var handleSa = bool.Parse(ConfigurationManager.AppSettings["HandleSa"]);
            if (handleSa)
            {
                ConfigHangfire(app, Container);
                HandleServiceBus();
            }
            ConfigHangfire(app, Container);
        }
        private static string eventHubName = "testingamsnetworkalarm";
        private static string connectionString =
           "Endpoint=sb://amsiotsystem.servicebus.windows.net/;SharedAccessKeyName=amstesting;SharedAccessKey=FTM1gesuHRype5fYi3eEM0OMSVo42di2m+MveUE+e2g=";


        string eventHubNameTest = "test";
        string connectionStringTest =
           "Endpoint=sb://amsiotsystem.servicebus.windows.net/;SharedAccessKeyName=amssend;SharedAccessKey=Ko7uePemrkKpf4A5Wflcl7BYfDv5xPASM8pSK7jIVDw=";

        private static readonly string TaskOneTestConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString.TaskOneTest"];
        private static readonly string TaskTwoTestConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString.TaskTwoTest"];
        private static readonly string KpiLogTestConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString.KpiLogTest"];

        private static readonly string TaskOneTestPath = ConfigurationManager.AppSettings["TaskOneTest.Path"];
        private static readonly string TaskTwoTestPath = ConfigurationManager.AppSettings["TaskTwoTest.Path"];
        private static readonly string KpiLogTestPath = ConfigurationManager.AppSettings["KpiLogTest.Path"];

        private void HandleServiceBus()
        {
           
            var receiverClient1 = QueueClient.CreateFromConnectionString(TaskOneTestConnectionString, TaskOneTestPath, ReceiveMode.ReceiveAndDelete);
            var receiverClient2 = QueueClient.CreateFromConnectionString(TaskTwoTestConnectionString, TaskTwoTestPath, ReceiveMode.ReceiveAndDelete);
          
            // Configure the callback options.
            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = true;

            options.AutoRenewTimeout = TimeSpan.FromMinutes(1);
            //Callback to handle received messages.
            receiverClient1.OnMessage(message =>
            {
                
                //// Process the message
                //var messageBodyStr = message.GetBody<string>();
                //var cellObj = JsonConvert.DeserializeObject<ReturnCell>(messageBodyStr);
                //var testId = cellObj.TestId;
                //var context = new NoisContext();
                //IOndemandService testingRequestService = new TestingSaRequestService(context,new MemoryCacheManager());
                //var testRequest = testingRequestService.GetTestRequest(testId);
                //var receiveTime = DateTime.UtcNow;
                //if (testRequest != null)
                //{
                //    try
                //    {
                //        var startTime = testRequest.StartTimeUtc;
                //        var originalTime = testRequest.OriginalStartTimeUtc;
                //        var eventTime = DateTime.Parse(cellObj.EventTime).ToUniversalTime();
                //        var secondDiff = eventTime.Subtract(startTime).TotalSeconds;
                //        var milisecondDiff = eventTime.Subtract(startTime).TotalMilliseconds;
                //        var minuteWillBeAdd = (int)secondDiff * 15;
                //        cellObj.EventTime = originalTime.AddMinutes(minuteWillBeAdd).ToString();
                //        receiveTime = originalTime.AddMinutes(minuteWillBeAdd);

                //        //create log event
                //        var newTestingLogEvent = new TestingLogEvent()
                //        {
                //            TestId = testId,
                //            CellName = cellObj.CellName,
                //            CreatedDateUtc = receiveTime,
                //            LocalCellId = cellObj.CellId.Replace(testId, ""),
                //            AverageRank = Double.Parse(cellObj.AvgRank),
                //            ExecuteCode = "Go to code 1",
                //            TestExecuteCode = TestExecuteCode.AddMimo
                //        };
                //        testingRequestService.CreateLogEventTesting(newTestingLogEvent);
                //    }
                //    catch (Exception ex)
                //    {
                //        Trace.TraceError("ERROR 1 " + ex.InnerException);
                //    }
                   
                //}
            }, options);
            // Callback to handle received messages.
            receiverClient2.OnMessage(message =>
            {
              
                // var messageBodyStr = message.GetBody<string>();
                //var cellObj = JsonConvert.DeserializeObject<ReturnCell>(messageBodyStr);
                //var testId = cellObj.TestId;
                //var context = new NoisContext();
                //IOndemandService testingRequestService = new TestingSaRequestService(context, new MemoryCacheManager());
                //var testRequest = testingRequestService.GetTestRequest(testId);
                //var receiveTime = DateTime.UtcNow;
                //if (testRequest != null)
                //{

                //    var startTime = testRequest.StartTimeUtc;
                //    var originalTime = testRequest.OriginalStartTimeUtc;
                //    var eventTime = DateTime.Parse(cellObj.EventTime).ToUniversalTime();
                //    var secondDiff = eventTime.Subtract(startTime).TotalSeconds;
                //    var milisecondDiff = eventTime.Subtract(startTime).TotalMilliseconds;
                //    var minuteWillBeAdd = (int)secondDiff * 15;
                //    cellObj.EventTime = originalTime.AddMinutes(minuteWillBeAdd).ToString();
                //    receiveTime = originalTime.AddMinutes(minuteWillBeAdd);

                //    //create log event
                //    var newTestingLogEvent = new TestingLogEvent()
                //    {
                //        TestId = testId,
                //        CellName = cellObj.CellName,
                //        CreatedDateUtc = receiveTime,
                //        LocalCellId = cellObj.CellId.Replace(testId, ""),
                //        AverageRank = Double.Parse(cellObj.AvgRank),
                //        ExecuteCode = "Go to code 2",
                //        TestExecuteCode = TestExecuteCode.CeaseMimo
                //    };
                //    testingRequestService.CreateLogEventTesting(newTestingLogEvent);
                //}

               
                
            }, options);

        }

        #region hangfire for process IoT

        /// <summary>
        /// Configure Hangfire for running background task
        /// </summary>
        /// <param name="app"></param>
        private static void ConfigHangfire(IAppBuilder app,IContainer container)
        {

            var options = new RedisStorageOptions
            {
                Prefix = "hangfire:ams:",
                InvisibilityTimeout = TimeSpan.FromHours(24),
            };
            GlobalConfiguration.Configuration
             
               .UseRedisStorage("Nois2016@insightuses.cloudapp.net:6379", 0, options)
               .UseAutofacActivator(container);

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] {new MyRestrictiveAuthorizationFilter()}
            });
           
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class MyRestrictiveAuthorizationFilter : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            // In case you need an OWIN context, use the next line,
            // `OwinContext` class is the part of the `Microsoft.Owin` package.
            var context = new OwinContext(owinEnvironment);

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return true;
            
        }
    }
}
