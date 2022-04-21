using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VSSystem._Models;
using VSSystem.Net.Extensions;

namespace VSSystem.Extensions
{
    public class SystemExtension
    {
        public const int ID_PRE_INIT = -2, ID_INITED = -1;
        public static int RegisterServer()
        {
            int server_ID = ID_INITED;

            if (!string.IsNullOrWhiteSpace(GlobalVariables.OnlineConfig?.ApiUrl))
            {
                try
                {
                    string url = $"{GlobalVariables.OnlineConfig.ApiUrl}/api/configuration/registerserver";
                    var serverObj = new ServerInfo
                    {
                        Name = GlobalVariables.HostName,
                        IPAddress = GlobalVariables.HostIPAddress,
                    };
                    var configResult = serverObj.PostJsonAsync(url, GlobalVariables.OnlineConfig.Timeout, serverObj, true).Result;
                    if (configResult.StatusCode == HttpStatusCode.OK)
                    {
                        string jsonResponse = configResult.ToStringAsync(Encoding.UTF8).Result;
                        serverObj = JsonConvert.DeserializeObject<ServerInfo>(jsonResponse);
                        if (serverObj != null)
                        {
                            server_ID = serverObj.ID;
                        }
                    }
                }
                catch { } 
            }
            return server_ID;
        }
        public static int RegisterComponent(int server_ID, string componentName, byte type, int httpPort, int httpsPort, string rootName)
        {
            int component_ID = ID_INITED;
            if (!string.IsNullOrWhiteSpace(GlobalVariables.OnlineConfig?.ApiUrl))
            {
                try
                {
                    string url = $"{GlobalVariables.OnlineConfig.ApiUrl}/api/configuration/registercomponent";
                    var componentObj = new ComponentInfo
                    {
                        Server_ID = server_ID,
                        Name = componentName,
                        Type = type,
                        HttpPort = httpPort,
                        HttpsPort = httpsPort,
                        RootName = rootName
                    };
                    var configResult = componentObj.PostJsonAsync(url, GlobalVariables.OnlineConfig.Timeout, componentObj, true).Result;
                    if (configResult.StatusCode == HttpStatusCode.OK)
                    {
                        string jsonResponse = configResult.ToStringAsync(Encoding.UTF8).Result;
                        componentObj = JsonConvert.DeserializeObject<ComponentInfo>(jsonResponse);
                        if (componentObj != null)
                        {
                            component_ID = componentObj.ID;
                        }
                    }
                }
                catch { } 
            }
            return component_ID;
        }
    }
}
