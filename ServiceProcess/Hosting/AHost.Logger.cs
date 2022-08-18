using System;
using VSSystem.Logger;

namespace VSSystem.ServiceProcess.Hosting
{
    public abstract partial class AHost
    {
        #region Logger
        bool _isUseConsoleLogger;
        protected virtual void _InitializeLogger()
        {
            try
            {
                string logSection = "logger";

                if ((GlobalVariables.WorkingFolder?.Exists ?? false) && _logger == null)
                {
                    ELogMode logMode = ELogMode.None;

                    string iniFilePath = GlobalVariables.WorkingFolder.FullName + "/config.ini";
                    if (!string.IsNullOrWhiteSpace(_rootName))
                    {
                        iniFilePath = WorkingFolder.FullName + $"/LogConfigs/{_Name?.ToLower()}.config.ini";
                    }
                    Configuration.IniConfiguration ini = new Configuration.IniConfiguration(iniFilePath);

                    if (ini.ReadValue<string>(logSection, "mode", "File")?.Equals("File", StringComparison.InvariantCultureIgnoreCase) ?? false)
                    {
                        logMode = ELogMode.File;
                    }
                    else if (ini.ReadValue<string>(logSection, "mode", "File")?.Equals("Api", StringComparison.InvariantCultureIgnoreCase) ?? false)
                    {
                        logMode = ELogMode.Api;
                    }

                    if (_isUseConsoleLogger)
                    {
                        logMode = ELogMode.Console;
                    }

                    ELogLevel logLevel = ELogLevel.None;
                    if (ini.ReadValue<string>(logSection, "info", "")?.Equals("1") ?? false)
                    {
                        logLevel = logLevel | ELogLevel.Info;
                    }
                    if (ini.ReadValue<string>(logSection, "debug", "")?.Equals("1") ?? false)
                    {
                        logLevel = logLevel | ELogLevel.Debug;
                    }
                    if (ini.ReadValue<string>(logSection, "warning", "1")?.Equals("1") ?? false)
                    {
                        logLevel = logLevel | ELogLevel.Warning;
                    }
                    if (ini.ReadValue<string>(logSection, "error", "1")?.Equals("1") ?? false)
                    {
                        logLevel = logLevel | ELogLevel.Error;
                    }

                    if ((GlobalVariables.OnlineConfig?.IncludeLogger ?? false) && !_isUseConsoleLogger)
                    {
                        logMode = ELogMode.Api;
                        string apiUrl = GlobalVariables.OnlineConfig.ApiUrl;
                        _logger = ALogger.Create(logMode, logLevel, apiUrl);
                    }
                    else
                    {
                        if (logMode == ELogMode.Api)
                        {
                            string apiUrl = ini.ReadValue<string>(logSection, "url");
                            _logger = ALogger.Create(logMode, logLevel, apiUrl);
                        }
                        else
                        {
                            _logger = ALogger.Create(logMode, logLevel, GlobalVariables.WorkingFolder.FullName);
                        }
                    }
                    _logger?.ClearAsync();
                }
            }
            catch { }
        }

        #endregion
    }
}
