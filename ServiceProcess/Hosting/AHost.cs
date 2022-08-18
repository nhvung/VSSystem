using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using VSSystem.Extensions;
using VSSystem.IO;
using VSSystem.Logger;
using VSSystem.Logger.Extensions;
using VSSystem.Net;
using VSSystem.ServiceProcess.Extensions;

namespace VSSystem.ServiceProcess.Hosting
{
    public abstract partial class AHost : IHost
    {
        protected string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }
        protected DirectoryInfo WorkingFolder => GlobalVariables.WorkingFolder;
        protected VSSystem.Models.OSVersion OSVersion => GlobalVariables.OSVersion;
        protected string _privateKey;
        protected string _rootName;
        protected ALogger _logger;
        public ALogger Logger { get { return _logger; } }
        protected List<string> _IPv4Addresses;
        bool _Enabled;
        public bool Enabled { get { return _Enabled; } set { _Enabled = value; } }
        protected Configuration.IniConfiguration _ini;
        protected int _server_ID, _component_ID;
        protected string _GlobalConfigurationUrl, _InstanceConfigurationUrl;
        public IServiceProvider Services => throw new NotImplementedException();

        public AHost(string name, string rootName = default, string privateKey = default)
        {
            _Name = name;
            _privateKey = privateKey;
            _rootName = rootName;
            _server_ID = SystemExtension.ID_PRE_INIT;
            _component_ID = SystemExtension.ID_PRE_INIT;
            _logger = null;
            _Enabled = true;
        }

        protected Task _InitializeParametersAsync(string[] args)
        {
            if (GlobalVariables.WorkingFolder == null)
            {
                GlobalVariables.WorkingFolder = new DirectoryInfo(Directory.GetCurrentDirectory());
            }
            if (Environment.OSVersion?.VersionString?.IndexOf("Microsoft Windows", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                GlobalVariables.OSVersion = VSSystem.Models.OSVersion.Windows;
            }
            else if (Environment.OSVersion?.VersionString?.IndexOf("Unix ", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                GlobalVariables.OSVersion = VSSystem.Models.OSVersion.Unix;
            }

            try
            {
                Dictionary<string, string> mArgs = CommandLine.GetCommandLineArgs(new string[]
                {
                    "--tlog", "-l",
                    "--binDir", "-d",
                    "--serviceName", "-sn",
                });

                _isUseConsoleLogger = true;
                if (mArgs?.ContainsKey("--tlog") ?? false)
                {
                    try
                    {
                        string tLogValue = mArgs["--tlog"];
                        _isUseConsoleLogger = !tLogValue?.Equals("1") ?? true;
                    }
                    catch
                    {
                    }
                }
                else if (mArgs?.ContainsKey("-l") ?? false)
                {
                    try
                    {
                        string tLogValue = mArgs["-l"];
                        _isUseConsoleLogger = !tLogValue?.Equals("1") ?? true;
                    }
                    catch
                    {
                    }
                }

                if (_isUseConsoleLogger)
                {
                    _Name = this.ReadServiveName(_Name);

                    if (OSVersion == VSSystem.Models.OSVersion.Windows)
                    {
                        bool createInstallFileConfirm = this.ConfirmCreateInstallationFiles();
                        if (createInstallFileConfirm)
                        {
                            this.CreateWindowsInstallationFile(GlobalVariables.WorkingFolder, _Name);
                            this.CreateWindowsUninstallationFile(GlobalVariables.WorkingFolder, _Name);
                        }
                    }
                    else if (OSVersion == VSSystem.Models.OSVersion.Unix)
                    {
                        bool createInstallFileConfirm = this.ConfirmCreateInstallationFiles();
                        if (createInstallFileConfirm)
                        {
                            this.CreateUbuntuInstallationFile(GlobalVariables.WorkingFolder, _Name);
                            this.CreateUbuntuUninstallationFile(GlobalVariables.WorkingFolder, _Name);
                            VSSystem.Extensions.CLIExtension.Execute(new System.Collections.Generic.List<string>() { "chmod 777 *.sh" });
                        }
                    }
                }

                if (mArgs?.ContainsKey("--binDir") ?? false)
                {
                    string sValue = mArgs["--binDir"]?.Replace("\"", "");
                    GlobalVariables.WorkingFolder = new DirectoryInfo(sValue);
                }
                else if (mArgs?.ContainsKey("-d") ?? false)
                {
                    string sValue = mArgs["-d"]?.Replace("\"", "");
                    GlobalVariables.WorkingFolder = new DirectoryInfo(sValue);
                }

                if (mArgs?.ContainsKey("--serviceName") ?? false)
                {
                    string sValue = mArgs["--serviceName"]?.Replace("\"", "");
                    _Name = sValue;
                }
                else if (mArgs?.ContainsKey("-sn") ?? false)
                {
                    string sValue = mArgs["-sn"]?.Replace("\"", "");
                    _Name = sValue;
                }

                GlobalVariables.HostIPAddress = string.Empty;
                try
                {
                    _IPv4Addresses = new List<string>();

                    foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        var ipProps = ni.GetIPProperties();

                        var gwObj = ipProps?.GatewayAddresses?.FirstOrDefault();
                        if (gwObj != null && !gwObj.Address.ToString().Equals("0.0.0.0"))
                        {
                            if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                                || ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                            {
                                foreach (UnicastIPAddressInformation ipObj in ipProps.UnicastAddresses)
                                {
                                    if (ipObj.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                    {
                                        _IPv4Addresses.Add(ipObj.Address.ToString());
                                    }
                                }
                            }
                        }
                    }

                    if (_IPv4Addresses?.Count > 0)
                    {
                        GlobalVariables.HostIPAddress = string.Join("|", _IPv4Addresses);
                    }
                }
                catch { }

                string onlineIniFilePath = WorkingFolder.FullName + "/config.online.ini";
                if (File.Exists(onlineIniFilePath))
                {
                    string onlineConfigSection = "online_config";
                    var onlineIni = new Configuration.IniConfiguration(onlineIniFilePath);
                    _GlobalConfigurationUrl = onlineIni.ReadValue<string>(onlineConfigSection, "global_url");
                    _InstanceConfigurationUrl = onlineIni.ReadValue<string>(onlineConfigSection, "instance_url");
                }
            }
            catch { }

            return Task.CompletedTask;
        }

        protected virtual void _InitializeStaticValues() { }
        protected virtual void _InitializeInjectionServices() { }
        protected void _AddInjectedServices(params IService[] services)
        {
            InjectionServices.AddServices(services);
        }
        protected void _AddInjectedHosts(params AHost[] hosts)
        {
            InjectionHosts.AddHosts(hosts);
        }
        protected virtual void _AdditionalHostInit() { }
        protected virtual async Task<Microsoft.Extensions.Hosting.IHost> _InitializeHost(string[] args)
        {
            var hostBuilder = await _InitializeHostBuilder(args);
            if (OSVersion == VSSystem.Models.OSVersion.Windows)
            {
                hostBuilder = hostBuilder.UseWindowsService();
            }
            else if (OSVersion == VSSystem.Models.OSVersion.Unix)
            {
                hostBuilder = hostBuilder.UseSystemd();
            }
            var host = hostBuilder.Build();
            return host;
        }
        protected virtual void _UseConfiguration(string[] args)
        {
            try
            {
                _ini = _LoadInstanceConfiguration().Result;
            }
            catch { }
        }
        protected virtual Task<IHostBuilder> _InitializeHostBuilder(string[] args)
        {
            var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);
            _UseConfiguration(args);
            _InitializeLogger();
            return Task.FromResult(builder);
        }
        async protected virtual Task<VSSystem.Configuration.IniConfiguration> _LoadInstanceConfiguration()
        {
            try
            {
                string iniFilePath = WorkingFolder.FullName + "/config.ini";
                if (!string.IsNullOrWhiteSpace(_rootName))
                {
                    iniFilePath = WorkingFolder.FullName + $"/ServiceConfigs/{_Name?.ToLower()}.config.ini";
                }

                this.LogInfo($"Load instance configuration. URL: {_InstanceConfigurationUrl}");

                var ini = await _LoadOnlineConfiguration(iniFilePath, _InstanceConfigurationUrl);
                return ini;
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                return null;
            }
        }
        async protected virtual Task<VSSystem.Configuration.IniConfiguration> _LoadGlobalConfiguration()
        {
            try
            {
                string iniFilePath = WorkingFolder.FullName + "/global.ini";
                if (WorkingFolder?.Parent?.Exists ?? false)
                {
                    iniFilePath = WorkingFolder.Parent.FullName + "/global.ini";
                }

                this.LogInfo($"Load global configuration. URL: {_GlobalConfigurationUrl}");

                var ini = await _LoadOnlineConfiguration(iniFilePath, _GlobalConfigurationUrl);
                return ini;
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                return null;
            }
        }
        async protected Task<VSSystem.Configuration.IniConfiguration> _LoadOnlineConfiguration(string filePath, string url)
        {
            try
            {
                FileInfo iniFile = new FileInfo(filePath);
                var ini = new VSSystem.Configuration.IniConfiguration(filePath);

                if (!string.IsNullOrWhiteSpace(url))
                {
                    string iniValue = string.Empty;
                    if (File.Exists(filePath))
                    {
                        iniValue = File.ReadAllText(filePath, Encoding.UTF8);
                    }

                    var configurationObj = new VSSystem.Models.ConfigurationInfo
                    {
                        IPAddresses = _IPv4Addresses,
                        ComponentName = _Name,
                        Base64Value = VSSystem.Security.Cryptography.EncryptToBase64String(iniValue, _privateKey),
                        LastUpdatedTicks = iniFile.LastWriteTimeUtc.Ticks,
                    };
                    var requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(configurationObj));

                    var iniResponse = await VSSystem.Net.Extensions.HttpWebClient64Extension.PostDataAsync(this, url, 30, ContentType.Json, requestData, true);
                    if (iniResponse.StatusCode == HttpStatusCode.MethodNotAllowed)
                    {
                        iniResponse = await VSSystem.Net.Extensions.HttpWebClient64Extension.GetDataAsync(this, url, 30, ContentType.Json, true);
                    }
                    if (iniResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        iniValue = await iniResponse.ToStringAsync(System.Text.Encoding.UTF8);
                        if (!string.IsNullOrWhiteSpace(iniValue))
                        {
                            configurationObj = JsonConvert.DeserializeObject<VSSystem.Models.ConfigurationInfo>(iniValue);
                            if (!string.IsNullOrWhiteSpace(configurationObj.Base64Value))
                            {
                                iniValue = VSSystem.Security.Cryptography.DecryptFromBase64String<string>(configurationObj.Base64Value, _privateKey);
                                ini.LoadFromString(iniValue);
                                ini.Save();
                            }
                        }
                    }
                }
                return ini;
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                return null;
            }
        }

        public void AdditionalHostInit() { _AdditionalHostInit(); }
        public virtual void ConfigureHostDefaults(string[] args)
        {
            try
            {
                _UseConfiguration(args);
                _InitializeLogger();
                _InitializeStaticValues();
                _InitializeInjectionServices();
            }
            catch { }
        }

        protected Task _RunAdditionComponents()
        {
            try
            {
                this.LogInfo($"Loading addition components...");

                _InitializeStaticValues();
                _InitializeInjectionServices();

                this.LogInfo($"Load addition components done.");

                return InjectionServices.StartAllServicesAsync();
            }
            catch { }
            return Task.CompletedTask;
        }
        protected virtual void _InitializeInjectionHosts(string[] args)
        {
            InjectionHosts.ConfigureHostDefaults(args);
        }

        public void Dispose()
        {

        }

        async public virtual Task StartAsync(string[] args, CancellationToken cancellationToken = default)
        {
            try
            {
                await _InitializeParametersAsync(args);
                var host = await _InitializeHost(args);
                var hostTask = host.RunAsync(cancellationToken);

                try
                {

                    int processID = 0;
                    var currentProcess = Process.GetCurrentProcess();
                    if (currentProcess != null)
                    {
                        processID = currentProcess.Id;
                    }
                    if (OSVersion == VSSystem.Models.OSVersion.Windows)
                    {
                        this.CreateWindowsStopServiceFile(WorkingFolder, _Name, processID);
                        this.CreateWindowsRestartServiceFile(WorkingFolder, _Name, processID);
                    }
                    else if (OSVersion == VSSystem.Models.OSVersion.Unix)
                    {
                        this.CreateUbuntuStopServiceFile(WorkingFolder, _Name, processID);
                        this.CreateUbuntuRestartServiceFile(WorkingFolder, _Name, processID);
                        VSSystem.Extensions.CLIExtension.Execute(new System.Collections.Generic.List<string>() { "chmod 777 *.sh" });
                    }
                }
                catch { }

                this.LogInfo($"{_Name} Host started.");

                _ = _RunAdditionComponents();

                await hostTask;
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }
        }

        public virtual Task StartAsync(CancellationToken cancellationToken = default)
        {
            return StartAsync(null, cancellationToken);
        }

        public async virtual Task StopAsync(CancellationToken cancellationToken = default)
        {
            this.LogInfo($"{_Name} Host stopping...");
            await InjectionServices.StopAllServicesAsync();
            this.LogInfo($"{_Name} Host stopped.");
        }
        public async virtual Task RunAsync(string[] args, CancellationToken cancellationToken = default)
        {
            await StartAsync(args, cancellationToken);
            await StopAsync(cancellationToken);
        }
    }
}