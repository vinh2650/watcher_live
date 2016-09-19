using KellermanSoftware.NetSFtpLibrary;
using System.IO;
using System.Configuration;

namespace WatchFileService
{
    /// <summary>
    /// Represent class for watching file generate by private PC and uploading to server via SFTP
    /// </summary>
    public class FileWatcherForUploading
    {
        #region fields

        private readonly SFTP _sftp;
        private readonly FileSystemWatcher _fileSystemWatcher;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public FileWatcherForUploading()
        {
            //init file watcher     
            _fileSystemWatcher = new FileSystemWatcher(ConfigurationManager.AppSettings["location"]);
          
            //init sftp object
            _sftp = new SFTP
            {
                HostAddress = ConfigurationManager.AppSettings["HostAddress"],
                UserName = ConfigurationManager.AppSettings["UserName"],
                Password = ConfigurationManager.AppSettings["Password"],
                CurrentDirectory = ConfigurationManager.AppSettings["CurrentDirectory"]
            };
           
        }

        #region method

        /// <summary>
        /// Start watcher
        /// </summary>
        public void StartWatcher()
        {
            //bind OnCreated function
            _fileSystemWatcher.Created += (OnNewFileCreated);
            _fileSystemWatcher.EnableRaisingEvents = true;

            //maintain connection with server from the time service start
            _sftp.Connect();
        }



        public void StopWatcher()
        {
            //todo maybe check any file is uploading or not. If yes, must wait it finish
            //disconnect sftp
            _sftp.Disconnect();
        }

        #endregion


        #region utils

        /// <summary>
        /// Function for handler the event when 1 file is created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewFileCreated(object sender, FileSystemEventArgs e)
        {
            var filePath = e.FullPath;
            var fileName = e.Name;
            if (File.Exists(filePath))
            {
                //_sftp.Connect();
                //upload file to server via SFTP 
                _sftp.UploadFile(filePath, fileName);
                //_sftp.Disconnect();
            }

        }

        #endregion

    }
}
