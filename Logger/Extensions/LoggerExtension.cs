using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VSSystem.Logger.Extensions
{
    public static class LoggerExtension
    {
        #region Base Log

        public static Task LogInfoAsync(this object sender, ALogger logger, string name, string contents, string component)
        {
            return logger?.LogInfoAsync(name, contents, component) ?? Task.CompletedTask;
        }
        public static Task LogDebugAsync(this object sender, ALogger logger, string name, string contents, string component)
        {
            return logger?.LogDebugAsync(name, contents, component) ?? Task.CompletedTask;
        }
        public static Task LogWarningAsync(this object sender, ALogger logger, string name, string contents, string component)
        {
            return logger?.LogWarningAsync(name, contents, component) ?? Task.CompletedTask;
        }
        public static Task LogErrorAsync(this object sender, ALogger logger, string name, string contents, string component)
        {
            return logger?.LogErrorAsync(name, contents, component) ?? Task.CompletedTask;
        }
        public static Task LogErrorAsync(this object sender, ALogger logger, string name, Exception ex, string component)
        {
            return logger?.LogErrorAsync(name, ex, component) ?? Task.CompletedTask;
        }
        public static Task LogCsvAsync(this object sender, ALogger logger, string name, string[] headers, string[] values, string component)
        {
            return logger?.LogCsvAsync(name, headers, values, component);
        }

        public static void LogInfo(this object sender, ALogger logger, string name, string contents, string component)
        {
            logger?.LogInfo(name, contents, component);
        }
        public static void LogDebug(this object sender, ALogger logger, string name, string contents, string component)
        {
            logger?.LogDebug(name, contents, component);
        }
        public static void LogWarning(this object sender, ALogger logger, string name, string contents, string component)
        {
            logger?.LogWarning(name, contents, component);
        }
        public static void LogError(this object sender, ALogger logger, string name, string contents, string component)
        {
            logger?.LogError(name, contents, component);
        }
        public static void LogError(this object sender, ALogger logger, string name, Exception ex, string component)
        {
            logger?.LogError(name, ex, component);
        }
        public static void LogCsv(this object sender, ALogger logger, string name, string[] headers, string[] values, string component)
        {
            logger?.LogCsv(name, headers, values, component);
        }
        #endregion
    }
}
