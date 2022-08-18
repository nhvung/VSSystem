using System;
using System.Collections.Generic;
using System.Linq;
using VSSystem.Threading.Tasks.Extensions;

namespace VSSystem.ServiceProcess.Hosting
{
    public class InjectionHosts
    {
        static Dictionary<string, AHost> _items;
        public static void AddHosts(params AHost[] hosts)
        {

            try
            {
                if (_items == null)
                {
                    _items = new Dictionary<string, AHost>(StringComparer.InvariantCultureIgnoreCase);
                }
                if (hosts?.Length > 0)
                {
                    foreach (var host in hosts)
                    {
                        if (host != null)
                        {
                            if (!_items.ContainsKey(host.Name))
                            {
                                _items.Add(host.Name, host);

                            }
                        }
                    }
                }
            }
            catch { }
        }
        public static void ConfigureHostDefaults(string[] args)
        {

            try
            {
                if (_items?.Count > 0)
                {
                    int nTask = _items.Count;
                    _items.Values.ConsecutiveRun(host => host.ConfigureHostDefaults(args), 1);
                }
            }
            catch { }
        }

        public static void ReloadHosts(string[] args, params string[] hostNames)
        {

            try
            {
                if (_items?.Count > 0 && hostNames?.Length > 0)
                {
                    var hostObjs = _items.Where(ite => hostNames.Contains(ite.Key, StringComparer.InvariantCultureIgnoreCase))?.ToDictionary(ite => ite.Key, ite => ite.Value);
                    if (hostObjs?.Count > 0)
                    {
                        int nTask = hostObjs.Count;
                        hostObjs.Values.ConsecutiveRun(host => host.ConfigureHostDefaults(args), nTask);
                    }
                }
            }
            catch { }
        }

        public static List<object> GetHosts()
        {
            List<object> result = new List<object>();
            if (_items?.Count > 0)
            {
                int idx = 1;
                foreach (var item in _items)
                {
                    result.Add(new
                    {
                        No = idx.ToString(),
                        Name = item.Key,
                        Enabled = item.Value.Enabled,
                    });
                    idx++;
                }
            }

            return result;
        }

        public static void DisableHosts(params string[] hostNames)
        {
            if (hostNames?.Length > 0 && _items?.Count > 0)
            {
                foreach (string hostName in hostNames)
                {
                    if (_items.ContainsKey(hostName))
                    {
                        _items[hostName].Enabled = false;
                    }
                }
            }
        }
        public static void DisableAllHosts()
        {
            if (_items?.Count > 0)
            {
                foreach (var item in _items)
                {
                    item.Value.Enabled = false;
                }
            }
        }
        public static void EnableHosts(params string[] hostNames)
        {
            if (hostNames?.Length > 0 && _items?.Count > 0)
            {
                foreach (string hostName in hostNames)
                {
                    if (_items.ContainsKey(hostName))
                    {
                        _items[hostName].Enabled = true;
                    }
                }
            }
        }
        public static void EnableAllHosts()
        {
            if (_items?.Count > 0)
            {
                foreach (var item in _items)
                {
                    item.Value.Enabled = true;
                }
            }
        }
        public static bool GetHostEnabled(string hostName)
        {
            return _items?.ContainsKey(hostName) ?? false ? _items[hostName].Enabled : false;
        }
    }
}