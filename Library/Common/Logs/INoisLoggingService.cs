using System;
using System.Threading.Tasks;

namespace Common.Logs
{
    /// <summary>
    /// logging service
    /// </summary>
    public interface INoisLoggingService
    {
        /// <summary>
        /// log error
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        void LogError(string error,Exception ex);

        /// <summary>
        /// log info
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        void LogInfo(string error, Exception ex);

        /// <summary>
        /// log fatal
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        void LogFatal(string error, Exception ex);

        /// <summary>
        /// log warning
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        void LogWarning(string error, Exception ex);
        /// <summary>
        /// log debug
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        void LogDebug(string error, Exception ex);

        /// <summary>
        /// log error async
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        Task LogErrorAsync(string error, Exception ex);

        /// <summary>
        /// log error async
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        Task LogInfoAsync(string error, Exception ex);

        /// <summary>
        /// log fatal async
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        Task LogFatalAsync(string error, Exception ex);

        /// <summary>
        /// log warning async
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        Task LogWarningAsync(string error, Exception ex);

        /// <summary>
        /// log debug async
        /// </summary>
        /// <param name="error"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        Task LogDebugAsync(string error, Exception ex);
    }
}
