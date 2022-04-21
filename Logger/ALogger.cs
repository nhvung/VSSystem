using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VSSystem.Logger
{
    public abstract class ALogger
    {

        protected static object _lockObj;
        protected ELogLevel _logLevel;
        public static ALogger Create(ELogMode logMode, ELogLevel logLevel = ELogLevel.Full, string initializePath = null)
        {
            if (logMode == ELogMode.Console)
            {
                return new ConsoleLogger() { _logLevel = logLevel };
            }
            else if (logMode == ELogMode.File)
            {
                return new FileLogger(initializePath) { _logLevel = logLevel };
            }
            else if (logMode == ELogMode.Api)
            {
                return new ApiLogger(initializePath) { _logLevel = logLevel };
            }
            return null;
        }
        bool _IsEnabled(ELogLevel logLevel)
        {
            return (_logLevel & logLevel) == logLevel;
        }
        protected abstract Task _ClearAsync();
        protected abstract Task _LogAsync(string name, string content, ELogLevel logLevel, string component);
        protected abstract Task _LogCsvAsync(string name, string[] headers, string[] values, string component);
        protected virtual Task _LogWithTagAsync(string name, string tagName, string content, ELogLevel logLevel, string component)
        {
            return _LogAsync(name, content, logLevel, component);
        }

        #region Public methods
        public Task ClearAsync() { return _ClearAsync(); }
        public Task LogInfoAsync(string name, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Info))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                return _LogAsync(name, content, ELogLevel.Info, component);
            }
            return Task.CompletedTask;
        }
        public Task LogInfoWithTagAsync(string name, string tagName, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Info))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                return _LogWithTagAsync(name, tagName, content, ELogLevel.Info, component);
            }
            return Task.CompletedTask;
        }
        public Task LogDebugAsync(string name, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Debug))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                return _LogAsync(name, content, ELogLevel.Debug, component);
            }
            return Task.CompletedTask;
        }
        public Task LogDebugWithTagAsync(string name, string tagName, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Debug))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                return _LogWithTagAsync(name, tagName, content, ELogLevel.Debug, component);
            }
            return Task.CompletedTask;
        }
        public Task LogWarningAsync(string name, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Warning))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                return _LogAsync(name, content, ELogLevel.Warning, component);
            }
            return Task.CompletedTask;
        }
        public Task LogWarningWithTagAsync(string name, string tagName, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Warning))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                return _LogWithTagAsync(name, tagName, content, ELogLevel.Warning, component);
            }
            return Task.CompletedTask;
        }
        public Task LogErrorAsync(string name, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Error))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                return _LogAsync(name, content, ELogLevel.Error, component);
            }
            return Task.CompletedTask;
        }
        public Task LogErrorWithTagAsync(string name, string tagName, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Error))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                return _LogWithTagAsync(name, tagName, content, ELogLevel.Error, component);
            }
            return Task.CompletedTask;
        }
        public Task LogErrorAsync(string name, Exception ex, string component)
        {
            string content = string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
            return LogErrorAsync(name, content, component);
        }
        public Task LogErrorWithTagAsync(string name, string tagName, Exception ex, string component)
        {
            string content = string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
            return LogErrorWithTagAsync(name, tagName, content, component);
        }
        public Task LogCsvAsync(string name, string[] headers, string[] values, string component)
        {
            if (_IsEnabled(ELogLevel.Csv))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                return _LogCsvAsync(name, headers, values, component);
            }
            return Task.CompletedTask;
        }

        public void Clear() { _ClearAsync().Wait(); }
        public void LogInfo(string name, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Info))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                _LogAsync(name, content, ELogLevel.Info, component).Wait();
            }
        }
        public void LogInfoWithTag(string name, string tagName, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Info))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                _LogWithTagAsync(name, tagName, content, ELogLevel.Info, component).Wait();
            }
        }
        public void LogDebug(string name, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Debug))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                _LogAsync(name, content, ELogLevel.Debug, component).Wait();
            }
        }
        public void LogDebugWithTag(string name, string tagName, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Debug))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                _LogWithTagAsync(name, tagName, content, ELogLevel.Debug, component).Wait();
            }
        }
        public void LogWarning(string name, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Warning))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                _LogAsync(name, content, ELogLevel.Warning, component).Wait();
            }
        }
        public void LogWarningWithTag(string name, string tagName, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Warning))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                _LogWithTagAsync(name, tagName, content, ELogLevel.Warning, component).Wait();
            }
        }
        public void LogError(string name, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Error))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                _LogAsync(name, content, ELogLevel.Error, component).Wait();
            }
        }
        public void LogErrorWithTag(string name, string tagName, string content, string component)
        {
            if (_IsEnabled(ELogLevel.Error))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                _LogWithTagAsync(name, tagName, content, ELogLevel.Error, component).Wait();
            }
        }
        public void LogError(string name, Exception ex, string component)
        {
            string content = string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
            LogError(name, content, component);
        }
        public void LogErrorWithTag(string name, string tagName, Exception ex, string component)
        {
            string content = string.Format("{0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
            LogErrorWithTag(name, tagName, content, component);
        }
        public void LogCsv(string name, string[] headers, string[] values, string component)
        {
            if (_IsEnabled(ELogLevel.Csv))
            {
                if (_lockObj == null)
                {
                    _lockObj = new object();
                }
                _LogCsvAsync(name, headers, values, component).Wait();
            }
        }
        #endregion
    }
}
