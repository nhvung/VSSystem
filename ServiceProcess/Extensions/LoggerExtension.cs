using System;
using System.Threading.Tasks;
using VSSystem.ServiceProcess.Workers;

namespace VSSystem.ServiceProcess.Extensions
{
    public static class LoggerExtension
    {
        #region Service
        #region Log Common
        public static Task LogInfoAsync(this IService sender, string contents)
        {
            return sender?.Logger?.LogInfoAsync(sender.Name, contents, sender.Name);
        }
        public static Task LogDebugAsync(this IService sender, string contents)
        {
            return sender?.Logger?.LogDebugAsync(sender.Name, contents, sender.Name);
        }
        public static Task LogWarningAsync(this IService sender, string contents)
        {
            return sender?.Logger?.LogWarningAsync(sender.Name, contents, sender.Name);
        }
        public static Task LogErrorAsync(this IService sender, string contents)
        {
            return sender?.Logger?.LogErrorAsync(sender.Name, contents, sender.Name);
        }
        public static Task LogErrorAsync(this IService sender, Exception ex)
        {
            return sender?.Logger?.LogErrorAsync(sender.Name, ex, sender.Name);
        }
        public static void LogInfo(this IService sender, string contents)
        {
            sender?.Logger?.LogInfo(sender.Name, contents, sender.Name);
        }
        public static void LogDebug(this IService sender, string contents)
        {
            sender?.Logger?.LogDebug(sender.Name, contents, sender.Name);
        }
        public static void LogWarning(this IService sender, string contents)
        {
            sender?.Logger?.LogWarning(sender.Name, contents, sender.Name);
        }
        public static void LogError(this IService sender, string contents)
        {
            sender?.Logger?.LogError(sender.Name, contents, sender.Name);
        }
        public static void LogError(this IService sender, Exception ex)
        {
            sender?.Logger?.LogError(sender.Name, ex, sender.Name);
        }
        #endregion

        #region Log with Tag
        public static Task LogInfoWithTagAsync(this IService sender, string tagName, string contents)
        {
            return sender?.Logger?.LogInfoWithTagAsync(sender.Name, tagName, contents, sender.Name);
        }
        public static Task LogDebugAsync(this IService sender, string tagName, string contents)
        {
            return sender?.Logger?.LogDebugWithTagAsync(sender.Name, tagName, contents, sender.Name);
        }
        public static Task LogWarningAsync(this IService sender, string tagName, string contents)
        {
            return sender?.Logger?.LogWarningWithTagAsync(sender.Name, tagName, contents, sender.Name);
        }
        public static Task LogErrorAsync(this IService sender, string tagName, string contents)
        {
            return sender?.Logger?.LogErrorWithTagAsync(sender.Name, tagName, contents, sender.Name);
        }
        public static Task LogErrorAsync(this IService sender, string tagName, Exception ex)
        {
            return sender?.Logger?.LogErrorWithTagAsync(sender.Name, tagName, ex, sender.Name);
        }
        public static void LogInfoWithTag(this IService sender, string tagName, string contents)
        {
            sender?.Logger?.LogInfoWithTag(sender.Name, tagName, contents, sender.Name);
        }
        public static void LogDebugWithTag(this IService sender, string tagName, string contents)
        {
            sender?.Logger?.LogDebugWithTag(sender.Name, tagName, contents, sender.Name);
        }
        public static void LogWarningWithTag(this IService sender, string tagName, string contents)
        {
            sender?.Logger?.LogWarningWithTag(sender.Name, tagName, contents, sender.Name);
        }
        public static void LogErrorWithTag(this IService sender, string tagName, string contents)
        {
            sender?.Logger?.LogErrorWithTag(sender.Name, tagName, contents, sender.Name);
        }
        public static void LogErrorWithTag(this IService sender, string tagName, Exception ex)
        {
            sender?.Logger?.LogErrorWithTag(sender.Name, tagName, ex, sender.Name);
        }
        #endregion



        #endregion

        #region Worker
        #region Log Common
        public static Task LogInfoAsync(this IWorker sender, string contents)
        {
            return sender?.Logger?.LogInfoAsync(sender.Name, contents, sender.ServiceName);
        }
        public static Task LogDebugAsync(this IWorker sender, string contents)
        {
            return sender?.Logger?.LogDebugAsync(sender.Name, contents, sender.ServiceName);
        }
        public static Task LogWarningAsync(this IWorker sender, string contents)
        {
            return sender?.Logger?.LogWarningAsync(sender.Name, contents, sender.ServiceName);
        }
        public static Task LogErrorAsync(this IWorker sender, string contents)
        {
            return sender?.Logger?.LogErrorAsync(sender.Name, contents, sender.ServiceName);
        }
        public static Task LogErrorAsync(this IWorker sender, Exception ex)
        {
            return sender?.Logger?.LogErrorAsync(sender.Name, ex, sender.ServiceName);
        }
        public static void LogInfo(this IWorker sender, string contents)
        {
            sender?.Logger?.LogInfo(sender.Name, contents, sender.ServiceName);
        }
        public static void LogDebug(this IWorker sender, string contents)
        {
            sender?.Logger?.LogDebug(sender.Name, contents, sender.ServiceName);
        }
        public static void LogWarning(this IWorker sender, string contents)
        {
            sender?.Logger?.LogWarning(sender.Name, contents, sender.ServiceName);
        }
        public static void LogError(this IWorker sender, string contents)
        {
            sender?.Logger?.LogError(sender.Name, contents, sender.ServiceName);
        }
        public static void LogError(this IWorker sender, Exception ex)
        {
            sender?.Logger?.LogError(sender.Name, ex, sender.ServiceName);
        }
        #endregion

        #region Log with Tag
        public static Task LogInfoWithTagAsync(this IWorker sender, string tagName, string contents)
        {
            return sender?.Logger?.LogInfoWithTagAsync(sender.Name, tagName, contents, sender.ServiceName);
        }
        public static Task LogDebugWithTagAsync(this IWorker sender, string tagName, string contents)
        {
            return sender?.Logger?.LogDebugWithTagAsync(sender.Name, tagName, contents, sender.ServiceName);
        }
        public static Task LogWarningWithTagAsync(this IWorker sender, string tagName, string contents)
        {
            return sender?.Logger?.LogWarningWithTagAsync(sender.Name, tagName, contents, sender.ServiceName);
        }
        public static Task LogErrorWithTagAsync(this IWorker sender, string tagName, string contents)
        {
            return sender?.Logger?.LogErrorWithTagAsync(sender.Name, tagName, contents, sender.ServiceName);
        }
        public static Task LogErrorWithTagAsync(this IWorker sender, string tagName, Exception ex)
        {
            return sender?.Logger?.LogErrorWithTagAsync(sender.Name, tagName, ex, sender.ServiceName);
        }
        public static void LogInfoWithTag(this IWorker sender, string tagName, string contents)
        {
            sender?.Logger?.LogInfoWithTag(sender.Name, tagName, contents, sender.ServiceName);
        }
        public static void LogDebugWithTag(this IWorker sender, string tagName, string contents)
        {
            sender?.Logger?.LogDebugWithTag(sender.Name, tagName, contents, sender.ServiceName);
        }
        public static void LogWarningWithTag(this IWorker sender, string tagName, string contents)
        {
            sender?.Logger?.LogWarningWithTag(sender.Name, tagName, contents, sender.ServiceName);
        }
        public static void LogErrorWithTag(this IWorker sender, string tagName, string contents)
        {
            sender?.Logger?.LogErrorWithTag(sender.Name, tagName, contents, sender.ServiceName);
        }
        public static void LogErrorWithTag(this IWorker sender, string tagName, Exception ex)
        {
            sender?.Logger?.LogErrorWithTag(sender.Name, tagName, ex, sender.ServiceName);
        }
        #endregion

        #endregion


    }
}
