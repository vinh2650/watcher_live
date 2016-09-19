using System;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.ServiceProcess;
using Autofac;
using Common.Logs;
using Data;
using Hangfire;
using Hangfire.Redis;
using Service.CachingLayer;
using Service.Implement.Authentication;
using Service.Interface.Authentication;


namespace ScanAndReplyEmailService
{
    public partial class AmsScanAndReplyEmailService : ServiceBase
    {
        private BackgroundJobServer _server;

        private static readonly string RedisPassword = ConfigurationManager.AppSettings["RedisPassword"];
        private static readonly string RedisHost = ConfigurationManager.AppSettings["RedisUrl"];
        private static readonly string RedisPort = ConfigurationManager.AppSettings["RedisPort"];

        private static readonly string RedisConnectionString = $"{RedisPassword}@{RedisHost}:{RedisPort}";

        public AmsScanAndReplyEmailService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {

               
                var builder = new ContainerBuilder();

                builder.Register(c => new NoisContext())
                    .As<DbContext>().InstancePerDependency();
                builder.RegisterType<MemoryCacheManager>()
                  .As<ICacheManager>().InstancePerDependency();

                builder.RegisterType<DefaultLoggingService>().As<INoisLoggingService>().InstancePerDependency();
                builder.RegisterType<UserService>().As<IUserService>().InstancePerDependency();

                builder.RegisterType<ProcessEmail>().As<IProcessEmail>().InstancePerDependency();
                

                var container = builder.Build();
                var options = new RedisStorageOptions
                {
                    Prefix = "hangfire:ams:reply:email",
                    InvisibilityTimeout = TimeSpan.FromHours(24),
                };
                GlobalConfiguration.Configuration
                   .UseRedisStorage(RedisConnectionString, 0, options)
                   .UseAutofacActivator(container);
                
               
                var optionsbackground = new BackgroundJobServerOptions
                {
                    SchedulePollingInterval = TimeSpan.FromSeconds(1),
                };
                _server = new BackgroundJobServer(optionsbackground);

                RecurringJob.AddOrUpdate<IProcessEmail>("ProcessEmailPerMinute",p => p.ProcessMailQueue(), "*/1 * * * *");
                
            }
            catch (Exception ex)
            {
                Trace.TraceError("error has occur " + ex.InnerException);
            }
        }

        protected override void OnStop()
        {
            _server.Dispose();
        }
    }
}
