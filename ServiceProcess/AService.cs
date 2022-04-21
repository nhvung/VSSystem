using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSSystem._Models;
using VSSystem.Configuration;
using VSSystem.Extensions;
using VSSystem.Logger;
using VSSystem.Models;
using VSSystem.Net;
using VSSystem.Net.Extensions;
using VSSystem.Security;
using VSSystem.ServiceProcess.Extensions;
using VSSystem.ServiceProcess.Workers;

namespace VSSystem.ServiceProcess
{
    public abstract class AService : IService
    {
        protected string _name;
        public string Name { get { return _name; } }
        EServiceStatus _status;
        public EServiceStatus Status { get { return _status; } }
        Dictionary<string, KeyValuePair<IWorker, CancellationTokenSource>> _mWorkers;
        CancellationTokenSource _cancellationTokenSource;
        protected IniConfiguration _ini;
        protected DirectoryInfo WorkingFolder => GlobalVariables.WorkingFolder;
        protected ALogger _logger;
        public ALogger Logger { get { return _logger; } }

        public List<object> Workers
        {
            get
            {
                return _mWorkers?.OrderBy(worker=> worker.Key, StringComparer.InvariantCultureIgnoreCase)?.Select((worker, idx) => (object)new
                {
                    No = (idx + 1).ToString(),
                    Name = worker.Key,
                    Type = worker.Value.Key.Type.ToString(),
                    Enabled = worker.Value.Key.Enabled
                })?.ToList();
            }
        }
        protected string _privateKey;
        protected string[] _defaultSections;
        protected string _rootComponentName;
        int _Server_ID, _Component_ID;

        protected AService(ServiceStartInfo startInfo, ALogger logger = null)
        {
            _name = startInfo.Name;
            _status = EServiceStatus.Stopped;
            _mWorkers = new Dictionary<string, KeyValuePair<IWorker, CancellationTokenSource>>(StringComparer.InvariantCultureIgnoreCase);
            _defaultSections = startInfo.DefaultSections;
            _rootComponentName = startInfo.RootComponentName;
            _Server_ID = startInfo.Server_ID;
            _Component_ID = SystemExtension.ID_PRE_INIT;
            _privateKey = startInfo.PrivateKey;
            _logger = logger;
        }
        protected AService(string name, int server_ID, string rootComponentName, string privateKey, string[] defaultSections = null, ALogger logger = null)
        {
            _name = name;
            _status = EServiceStatus.Stopped;
            _mWorkers = new Dictionary<string, KeyValuePair<IWorker, CancellationTokenSource>>(StringComparer.InvariantCultureIgnoreCase);
            _defaultSections = defaultSections;
            _rootComponentName = rootComponentName;
            _Server_ID = server_ID;
            _Component_ID = SystemExtension.ID_PRE_INIT;
            _privateKey = privateKey;
            _logger = logger;
        }
        public void SetLogger(ALogger logger)
        {
            _logger = logger;
        }
        public virtual async Task StartAsync()
        {
            await this.LogInfoAsync($"{_name} Service starting...");
            await _InitConfiguration();
            _InitStaticValues();
            _cancellationTokenSource = new CancellationTokenSource();
            await Task.Run(_Execute);
        }

        public virtual async Task StopAsync()
        {
            await this.LogInfoAsync($"{_name} Service stopping...");
            foreach (var cs in _mWorkers)
            {
                try
                {
                    cs.Value.Value.Cancel();
                }
                catch { }
            }
            _mWorkers = new Dictionary<string, KeyValuePair<IWorker, CancellationTokenSource>>(StringComparer.InvariantCultureIgnoreCase);

            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch { }
            _status = EServiceStatus.Stopped;
            await this.LogInfoAsync($"{_name} Service stopped.");
        }

        protected async virtual Task _Execute()
        {
            _InitializeWorkers();
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                if (_mWorkers?.Count > 0)
                {
                    foreach (var worker in _mWorkers)
                    {
                        if (worker.Value.Key.Enabled)
                        {
                            _ = Task.Run(() => worker.Value.Key.RunAsync(worker.Value.Value.Token), _cancellationTokenSource.Token);
                        }
                        else
                        {
                            await this.LogWarningAsync(string.Format("{0} worker was disabled.", worker.Value.Key.Name));
                        }
                    }

                    await this.LogInfoAsync($"{_name} Service Started.");

                    try
                    {
                        _status = EServiceStatus.Running;
                        await Task.Delay(Timeout.Infinite, _cancellationTokenSource.Token);
                    }
                    catch
                    {
                        break;
                    }
                }
                else
                {
                    await this.LogWarningAsync($"{_name} Service has no worker.");
                    _status = EServiceStatus.Stopped;
                    await Task.Delay(Timeout.Infinite, _cancellationTokenSource.Token);
                }
            }
        }
        protected void AddWorker(IWorker worker)
        {
            if (_mWorkers == null)
            {
                _mWorkers = new Dictionary<string, KeyValuePair<IWorker, CancellationTokenSource>>(StringComparer.InvariantCultureIgnoreCase);
            }
            if (!_mWorkers.ContainsKey(worker.Name))
            {
                _mWorkers[worker.Name] = new KeyValuePair<IWorker, CancellationTokenSource>(worker, new CancellationTokenSource());
            }
        }
        protected abstract void _InitializeWorkers();
        public virtual void EnableWorkers(params string[] names)
        {
            try
            {
                if (names?.Length > 0)
                {
                    foreach (var name in names)
                    {
                        if (_mWorkers?.ContainsKey(name) ?? false)
                        {
                            if (!_mWorkers[name].Key?.Enabled ?? false)
                            {
                                _mWorkers[name].Key.Enabled = true;
                                _mWorkers[name] = new KeyValuePair<IWorker, CancellationTokenSource>(_mWorkers[name].Key, new CancellationTokenSource());
                                _ = Task.Run(() => _mWorkers[name].Key.RunAsync(_mWorkers[name].Value.Token), _cancellationTokenSource.Token);
                                _ = this.LogDebugAsync(string.Format("{0} worker running.", name));
                            }

                        }
                    }
                }
            }
            catch { }
        }
        public virtual void EnableAllWorkers()
        {
            try
            {
                foreach (var name in _mWorkers.Keys)
                {
                    if (!_mWorkers[name].Key?.Enabled ?? false)
                    {
                        _mWorkers[name].Key.Enabled = true;
                        _mWorkers[name] = new KeyValuePair<IWorker, CancellationTokenSource>(_mWorkers[name].Key, new CancellationTokenSource());
                        _ = Task.Run(() => _mWorkers[name].Key.RunAsync(_mWorkers[name].Value.Token), _cancellationTokenSource.Token);
                        _ = this.LogInfoAsync(string.Format("{0} worker running.", name));
                    }
                }

            }
            catch { }
        }
        public virtual void DisableWorkers(params string[] names)
        {
            try
            {
                if (names?.Length > 0)
                {
                    foreach (var name in names)
                    {
                        if (_mWorkers?.ContainsKey(name) ?? false)
                        {
                            if (_mWorkers[name].Key?.Enabled ?? false)
                            {
                                _mWorkers[name].Key.Enabled = false;
                                try
                                {
                                    _mWorkers[name].Value.Cancel();
                                    _ = this.LogWarningAsync(string.Format("{0} worker disabled.", name));
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { }
        }
        public virtual void DisableAllWorkers()
        {
            try
            {
                foreach (var name in _mWorkers.Keys)
                {
                    if (_mWorkers[name].Key?.Enabled ?? false)
                    {
                        _mWorkers[name].Key.Enabled = false;
                        try
                        {
                            _mWorkers[name].Value.Cancel();
                            _ = this.LogWarningAsync(string.Format("{0} worker disabled.", name));
                        }
                        catch { }
                    }
                }

            }
            catch { }
        }
        protected async virtual Task _InitConfiguration()
        {
            try
            {
                string iniFilePath = WorkingFolder.FullName + "/config.ini";
                if (!string.IsNullOrWhiteSpace(_rootComponentName))
                {
                    iniFilePath = WorkingFolder.FullName + $"/ServiceConfigs/{_name?.ToLower()}.config.ini";
                }
                _ini = new IniConfiguration(iniFilePath);
                if (!string.IsNullOrWhiteSpace(GlobalVariables.OnlineConfig?.ApiUrl))
                {
                    await _SyncConfigs(iniFilePath, _defaultSections);
                }
            }
            catch (Exception ex)
            {
                _ = this.LogErrorAsync(ex);
            }
        }
        protected virtual void _InitStaticValues() { }

        #region Online Configuration

        async protected virtual Task _SyncConfigs(string iniFilePath, string[] defaultSections = null)
        {
            try
            {
                if (_Server_ID == SystemExtension.ID_PRE_INIT)
                {
                    _Server_ID = SystemExtension.RegisterServer();
                }
                if (_Component_ID == SystemExtension.ID_PRE_INIT)
                {
                    _Component_ID = SystemExtension.RegisterComponent(_Server_ID, _name, (byte)EComponentType.Service, 0, 0, _rootComponentName);
                }

                if (_Server_ID > 0 && _Component_ID > 0)
                {
                    var localIni = new IniConfiguration(iniFilePath, defaultSections);
                    string configValue = localIni.ToString();
                    configValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(configValue));

                    string url = $"{GlobalVariables.OnlineConfig.ApiUrl}/api/configuration/syncconfig";

                    string configPath = iniFilePath.Substring(GlobalVariables.WorkingFolder.FullName.Length);

                    ConfigurationInfo configObj = new ConfigurationInfo
                    {
                        Component_ID = _Component_ID,
                        Server_ID = _Server_ID,
                        Path = configPath,
                        Base64Value = Cryptography.EncryptToBase64String(configValue, _privateKey),
                        Status = 1
                    };

                    string jsonRequest = JsonConvert.SerializeObject(configObj);
                    byte[] requestData = Encoding.UTF8.GetBytes(jsonRequest);
                    var configResult = await this.PostJsonAsync(url, GlobalVariables.OnlineConfig.Timeout, requestData, true);
                    if (configResult.StatusCode == HttpStatusCode.OK)
                    {
                        string jsonResponse = await configResult.ToStringAsync(Encoding.UTF8);
                        configObj = JsonConvert.DeserializeObject<ConfigurationInfo>(jsonResponse);
                        if (configObj != null)
                        {
                            if (configObj.Status == 2)
                            {
                                try
                                {
                                    configValue = Cryptography.DecryptFromBase64String<string>(configObj.Base64Value, _privateKey);
                                    configValue = Encoding.UTF8.GetString(Convert.FromBase64String(configValue));

                                    var cloudIni = new IniConfiguration();
                                    cloudIni.LoadFromString(configValue);

                                    foreach (var section in cloudIni.Sections)
                                    {
                                        var kvObjs = cloudIni[section];
                                        if (kvObjs?.Count > 0)
                                        {
                                            foreach (var kvObj in kvObjs)
                                            {
                                                localIni.AddValue(section, kvObj.Key, kvObj.Value);
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _ = this.LogErrorAsync(ex);
            }
        }

        #endregion
    }
}
