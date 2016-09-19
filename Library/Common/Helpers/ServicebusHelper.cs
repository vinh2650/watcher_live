using System.Configuration;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Common.Helpers
{
    public static class ServicebusHelper
    {
        private static readonly string SyncDataQueuePath = ConfigurationManager.AppSettings["AMSSyncDataQueue"];
        private static readonly string SyncDataQueueConnection = ConfigurationManager.AppSettings["AMSSyncDataConnection"];

        private static QueueClient _queueClient;

        public static QueueClient QueueClient
        {
            get
            {
                if (_queueClient == null)
                {
                    var namespaceManager =
                     NamespaceManager.CreateFromConnectionString(SyncDataQueueConnection);
                    if (!namespaceManager.QueueExists(SyncDataQueuePath))
                    {
                        namespaceManager.CreateQueue(SyncDataQueuePath);
                    }
                    // Initialize the connection to Service Bus queue.
                    _queueClient = QueueClient.CreateFromConnectionString(SyncDataQueueConnection, SyncDataQueuePath);
                }
                return _queueClient;
            }


        }

    }
}
