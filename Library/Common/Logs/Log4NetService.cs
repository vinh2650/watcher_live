using System;
using System.Threading.Tasks;
using Common.Helpers;
using log4net;

namespace Common.Logs
{
    /// <summary>
    /// Implement logging service by log4net
    /// </summary>
    public class Log4NetService : INoisLoggingService
    {
        private readonly ILog _log4NetLog;

        public Log4NetService()
        {
            _log4NetLog = Log4NetHelper.Logger;
        }

        public void LogError(string error, Exception ex)
        {
            _log4NetLog.Error(error, ex);
        }

        public void LogInfo(string error, Exception ex)
        {
            try
            {
                _log4NetLog.Info(error, ex);
            }
            catch (Exception ee)
            {

                throw;
            }
            ;
        }

        public void LogFatal(string error, Exception ex)
        {
            _log4NetLog.Fatal(error, ex);
        }

        public void LogWarning(string error, Exception ex)
        {
            _log4NetLog.Warn(error, ex);
        }

        public void LogDebug(string error, Exception ex)
        {
            _log4NetLog.Debug(error, ex);
        }

        public Task LogErrorAsync(string error, Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task LogInfoAsync(string error, Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task LogFatalAsync(string error, Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task LogWarningAsync(string error, Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task LogDebugAsync(string error, Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
