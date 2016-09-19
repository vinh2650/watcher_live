using System;
using System.Configuration;
using System.Globalization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace Common.Helpers
{
    public static class AzureHelper
    {

        #region table client

        private static CloudTableClient _cloudTableClient;

        public static CloudTableClient CloudTableClient
        {
            get
            {
                if (_cloudTableClient == null)
                {
                    var blobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];
                    // Create blob client and return reference to the container
                    var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
                    _cloudTableClient = blobStorageAccount.CreateCloudTableClient();
                }

                return _cloudTableClient;
            }


        }

        #region  group member table

        private static string GroupMemberReference = ConfigurationManager.AppSettings["GROUPMEMBERNAME"];
        private static CloudTable _groupMemberCloudTable;

        public static CloudTable GroupMemberCloudTable
        {
            get
            {
                if (_groupMemberCloudTable == null)
                {
                    _groupMemberCloudTable = CloudTableClient.GetTableReference(GroupMemberReference);

                    try
                    {
                        _groupMemberCloudTable.CreateIfNotExists();
                    }
                    catch (StorageException ex)
                    {

                        throw;
                    }


                }
                return _groupMemberCloudTable;
            }
        }

        #endregion


        #region  group member table

        private static string MemberReference = ConfigurationManager.AppSettings["MEMBERNAME"];
        private static CloudTable _memberCloudTable;

        public static CloudTable MemberCloudTable
        {
            get
            {
                if (_memberCloudTable == null)
                {
                    _memberCloudTable = CloudTableClient.GetTableReference(MemberReference);
                    try
                    {
                        _memberCloudTable.CreateIfNotExists();
                    }
                    catch (StorageException ex)
                    {

                        throw;
                    }


                }
                return _memberCloudTable;
            }
        }

        #endregion

        #endregion

        #region blob client

        private static CloudBlobClient _cloudBlobClient;

        public static CloudBlobClient CloudBlobClient
        {
            get
            {
                if (_cloudBlobClient == null)
                {
                    var blobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];
                    // Create blob client and return reference to the container
                    var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
                    _cloudBlobClient = blobStorageAccount.CreateCloudBlobClient();
                }

                return _cloudBlobClient;
            }


        }

        private static CloudBlobClient _cloudBlobClientTest;
        public static CloudBlobClient CloudBlobClientTest
        {
            get
            {
                if (_cloudBlobClient == null)
                {
                    var blobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionStringTest"];
                    // Create blob client and return reference to the container
                    var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
                    _cloudBlobClient = blobStorageAccount.CreateCloudBlobClient();
                }

                return _cloudBlobClient;
            }


        }

        private static string AlarmContainer = "alarms";
        private static CloudBlobContainer _alarmCloudBlobContainer;

        public static CloudBlobContainer AlarmCloudBlobContainer
        {
            get
            {
                if (_alarmCloudBlobContainer == null)
                {
                    _alarmCloudBlobContainer = CloudBlobClient.GetContainerReference(AlarmContainer);
                    _alarmCloudBlobContainer.CreateIfNotExists();
                    _alarmCloudBlobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess =
                                BlobContainerPublicAccessType.Blob
                        });
                }
                return _alarmCloudBlobContainer;
            }
        }

       
        #endregion


        #region queue Client
        private static CloudQueueClient _cloudQueueClient;

        public static CloudQueueClient CloudQueueClient
        {
            get
            {
                if (_cloudQueueClient == null)
                {
                    var blobStorageConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];
                    // Create blob client and return reference to the container
                    var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
                    _cloudQueueClient = blobStorageAccount.CreateCloudQueueClient();
                    _cloudQueueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

                }

                return _cloudQueueClient;
            }
        }

        private static CloudQueue _emailQueue { get; set; }

        public static CloudQueue EmailQueue
        {
            get
            {
                if (_emailQueue == null)
                {
                    var emailQueue = CloudQueueClient.GetQueueReference("queue");
                    emailQueue.CreateIfNotExists();
                    _emailQueue = emailQueue;
                }

                return _emailQueue;
            }
        }

        #endregion

        #region container
        /// <summary>
        /// avatar container, need to public
        /// </summary>
        private static CloudBlobContainer _avatarBlobContainer;

        private static CloudBlobContainer _csvBlobContainer;

        /// <summary>
        /// csv container, public
        /// </summary>
        public static CloudBlobContainer CsvBlobContainer
        {
            get
            {
                if (_csvBlobContainer == null)
                {
                    var csvFolderName = ConfigurationManager.AppSettings["CsvFolderName"];
                    // Retrieve a reference to a container. 
                    _csvBlobContainer = CloudBlobClient.GetContainerReference(csvFolderName);
                    // Create the container if it doesn't already exist.
                    _csvBlobContainer.CreateIfNotExists();
                    _csvBlobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess =
                                BlobContainerPublicAccessType.Blob
                        });
                }

                return _csvBlobContainer;
            }


        }

        /// <summary>
        /// avatar container, public
        /// </summary>
        public static CloudBlobContainer AvatarBlobContainer
        {
            get
            {
                if (_avatarBlobContainer == null)
                {
                    var avatarFolderName = ConfigurationManager.AppSettings["AvatarFolderName"];
                    // Retrieve a reference to a container. 
                    _avatarBlobContainer = CloudBlobClient.GetContainerReference(avatarFolderName);
                    // Create the container if it doesn't already exist.
                    _avatarBlobContainer.CreateIfNotExists();
                    _avatarBlobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess =
                                BlobContainerPublicAccessType.Blob
                        });
                }

                return _avatarBlobContainer;
            }


        }

        /// <summary>
        /// document container, secured
        /// </summary>
        private static CloudBlobContainer _documentBlobContainer;

        /// <summary>
        /// document container secured
        /// </summary>
        public static CloudBlobContainer DocumentBlobContainer
        {
            get
            {
                if (_documentBlobContainer == null)
                {
                    var documentFolderName = ConfigurationManager.AppSettings["DocumentFolderName"];
                    // Retrieve a reference to a container. 
                    _documentBlobContainer = CloudBlobClient.GetContainerReference(documentFolderName);
                    // Create the container if it doesn't already exist.
                    _documentBlobContainer.CreateIfNotExists();
                    // Setup public folder
                    _documentBlobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess =
                                BlobContainerPublicAccessType.Blob
                        });

                }

                return _documentBlobContainer;
            }
        }

        /// <summary>
        /// attachment container, secured
        /// </summary>
        private static CloudBlobContainer _attachmentBlobContainer;

        /// <summary>
        /// attachment container secured
        /// </summary>
        public static CloudBlobContainer AttachmentBlobContainer
        {
            get
            {
                if (_documentBlobContainer == null)
                {
                    var documentFolderName = ConfigurationManager.AppSettings["AttachFolderName"];
                    // Retrieve a reference to a container. 
                    _documentBlobContainer = CloudBlobClient.GetContainerReference(documentFolderName);
                    // Create the container if it doesn't already exist.
                    _documentBlobContainer.CreateIfNotExists();
                    // Setup public folder
                    _documentBlobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess =
                                BlobContainerPublicAccessType.Blob
                        });

                }

                return _documentBlobContainer;
            }
        }


        /// <summary>
        /// Generate download url for blob with permission
        /// </summary>
        /// <param name="blob">blob need to generate url</param>
        /// <param name="permission">permission like read, write, list</param>
        /// <param name="sasMinutesValid">time for expired link</param>
        /// <returns></returns>
        public static string GetDownloadLink(this CloudBlockBlob blob, SharedAccessBlobPermissions permission,
            int sasMinutesValid)
        {
            var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = permission,
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(sasMinutesValid),
            });
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", blob.Uri, sasToken);
        }



        #endregion
    }
}
