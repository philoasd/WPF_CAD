using NLog;

namespace LogLib
{
    public static class Log
    {
        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 保存流程日志
        /// </summary>
        /// <param name="message"></param>
        public static void LogInfo(string message)
        {
            _Logger.Info(message);
        }

        /// <summary>
        /// 保存错误日志
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(string message)
        {
            _Logger.Error(message);
        }

        /// <summary>
        /// 保存调试日志
        /// </summary>
        /// <param name="message"></param>
        public static void LogDebug(string message)
        {
            _Logger.Debug(message);
        }

        /// <summary>
        /// 保存警告日志
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(string message)
        {
            _Logger.Warn(message);
        }
    }

}
