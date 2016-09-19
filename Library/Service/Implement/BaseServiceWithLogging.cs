using System;
using Common.Logs;

namespace Service.Implement
{
    /// <summary>
    /// Base service that implement logging
    /// </summary>
    public class BaseServiceWithLogging
    {
        private readonly INoisLoggingService _noisLoggingService;

        public BaseServiceWithLogging(INoisLoggingService noisLoggingService)
        {
            _noisLoggingService = noisLoggingService;
        }
        public BaseServiceWithLogging() { }

        public void LogError(string error,Exception ex )
        {
            _noisLoggingService.LogError(error, ex);

        }

        public void LogWarning(string error, Exception ex)
        {
            _noisLoggingService.LogWarning(error, ex);
        }

        public void LogFatal(string error, Exception ex)
        {
            _noisLoggingService.LogFatal(error, ex);
        }

        public void LogDebug(string error, Exception ex)
        {
            _noisLoggingService.LogDebug(error, ex);
        }

        public void LogInformation(string error, Exception ex)
        {
            _noisLoggingService.LogInfo(error, ex);
        }
    }
}
