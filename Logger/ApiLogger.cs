using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using VSSystem._Models;

namespace VSSystem.Logger
{
    class ApiLogger : ALogger
    {
        const int ID_PRE_INIT = -2, ID_INITED = -1;

        string _apiUrl;
        HttpClient _client;
        string _hostName, _ipAddress;
        int _Server_ID;
        Dictionary<string, int> _Components;

        public ApiLogger(string apiUrl)
        {
            _apiUrl = apiUrl;
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };
            _client = new HttpClient(handler);
            _ipAddress = string.Empty;
            _hostName = Dns.GetHostName();
            _client.Timeout = new TimeSpan(0, 5, 0);
            try
            {
                var ipv4Addresses = new List<string>();

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
                                    ipv4Addresses.Add(ipObj.Address.ToString());
                                }
                            }
                        }
                    }
                }

                if (ipv4Addresses?.Count > 0)
                {
                    _ipAddress = string.Join("|", ipv4Addresses);
                }
            }
            catch { }
            _Server_ID = ID_PRE_INIT;
            _Components = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
        }


        protected override Task _ClearAsync() { return Task.CompletedTask; }
        void _RegisterServer()
        {
            try
            {
                lock (_lockObj)
                {
                    ServerInfo serverObj = new ServerInfo
                    {
                        Name = _hostName,
                        IPAddress = _ipAddress,
                    };
                    var jsonServer = JsonConvert.SerializeObject(serverObj);
                    var contentObj = new StringContent(jsonServer, Encoding.UTF8, "application/json");

                    var responseObj = _client.PostAsync(_apiUrl + "/api/log/registerserver", contentObj).Result;
                    if (responseObj?.StatusCode == HttpStatusCode.OK)
                    {
                        var responseString = responseObj.Content.ReadAsStringAsync().Result;
                        serverObj = JsonConvert.DeserializeObject<ServerInfo>(responseString);
                        if (serverObj != null)
                        {
                            _Server_ID = serverObj.ID;
                        }
                    }
                }
            }
            catch { }
        }

        int _RegisterComponent(string component)
        {
            int component_ID = ID_INITED;
            try
            {
                lock (_lockObj)
                {
                    var componentObj = new ComponentInfo()
                    {
                        Server_ID = _Server_ID,
                        Name = component
                    };
                    var jsonComponent = JsonConvert.SerializeObject(componentObj);
                    var contentObj = new StringContent(jsonComponent, Encoding.UTF8, "application/json");

                    var responseObj = _client.PostAsync(_apiUrl + "/api/log/registercomponent", contentObj).Result;
                    if (responseObj?.StatusCode == HttpStatusCode.OK)
                    {
                        var responseString = responseObj.Content.ReadAsStringAsync().Result;
                        var newComponentObj = JsonConvert.DeserializeObject<ComponentInfo>(responseString);
                        if (newComponentObj != null)
                        {
                            _Components[newComponentObj.Name] = newComponentObj.ID;
                            component_ID = newComponentObj.ID;
                        }
                    }
                }
            }
            catch { }
            return component_ID;
        }
        protected override Task _LogAsync(string name, string content, ELogLevel logLevel, string component)
        {
            try
            {
                if (_Server_ID == ID_PRE_INIT)
                {
                    _RegisterServer();
                }
                int component_ID = ID_PRE_INIT;
                if (!_Components.ContainsKey(component))
                {
                    component_ID = _RegisterComponent(component);
                }
                else
                {
                    component_ID = _Components[component];
                }

                if (_Server_ID > 0 && component_ID > 0)
                {
                    ApiAddLogRequest logObj = new ApiAddLogRequest
                    {
                        Server_ID = _Server_ID,
                        Component_ID = component_ID,
                        LogName = name,
                        Contents = content,
                        LogTicks = DateTime.UtcNow.Ticks,
                        LogType = logLevel.ToString()
                    };

                    var jsonLog = JsonConvert.SerializeObject(logObj);
                    var contentObj = new StringContent(jsonLog, Encoding.UTF8, "application/json");

                    return _client.PostAsync(_apiUrl + "/api/log/add", contentObj);
                }
            }
            catch { }
            return Task.CompletedTask;
        }

        protected override Task _LogCsvAsync(string name, string[] headers, string[] values, string component)
        {
            return Task.CompletedTask;
        }
        protected override Task _LogWithTagAsync(string name, string tagName, string content, ELogLevel logLevel, string component)
        {
            try
            {
                if (_Server_ID == ID_PRE_INIT)
                {
                    _RegisterServer();
                }
                int component_ID = ID_PRE_INIT;
                if (!_Components.ContainsKey(component))
                {
                    component_ID = _RegisterComponent(component);
                }
                else
                {
                    component_ID = _Components[component];
                }

                if (_Server_ID > 0 && component_ID > 0)
                {
                    ApiAddLogWithTagRequest logObj = new ApiAddLogWithTagRequest
                    {
                        Server_ID = _Server_ID,
                        Component_ID = component_ID,
                        LogName = name,
                        TagName = tagName,
                        Contents = content,
                        LogTicks = DateTime.UtcNow.Ticks,
                        LogType = logLevel.ToString()
                    };

                    var jsonLog = JsonConvert.SerializeObject(logObj);
                    var contentObj = new StringContent(jsonLog, Encoding.UTF8, "application/json");

                    return _client.PostAsync(_apiUrl + "/api/log/add", contentObj);
                }
            }
            catch { }
            return Task.CompletedTask;
        }
    }
}
