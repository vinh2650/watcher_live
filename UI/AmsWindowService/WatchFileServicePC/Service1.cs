using System.ServiceProcess;

namespace WatchFileService
{
    public partial class WatchFileService : ServiceBase
    {
        private readonly FileWatcherForUploading _fileWatcher;
        public WatchFileService()
        {
            InitializeComponent();
            _fileWatcher = new FileWatcherForUploading();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            _fileWatcher.StartWatcher();
        }
        protected override void OnStop()
        {
            //todo maybe process something here when try to stopping service e.g finish uploading first
            _fileWatcher.StopWatcher();
        }
    }
}
