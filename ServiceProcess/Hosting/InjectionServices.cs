using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSSystem.ServiceProcess.Hosting
{
    public class InjectionServices
    {

        static Dictionary<string, IService> _items;
        public static void AddServices(params IService[] services)
        {
            if (_items == null)
            {
                _items = new Dictionary<string, IService>(StringComparer.InvariantCultureIgnoreCase);
            }
            if (services?.Length > 0)
            {
                foreach (var service in services)
                {
                    if (service != null)
                    {
                        if (!_items.ContainsKey(service.Name))
                        {
                            _items.Add(service.Name, service);
                        }
                    }
                }
            }
        }
        public static List<object> GetAllServices()
        {
            return _items?.OrderBy(ite => ite.Key, StringComparer.InvariantCultureIgnoreCase)
                ?.Select((service, idx) => (object)new
                {
                    No = (idx + 1).ToString(),
                    service.Value.Name,
                    Status = service.Value.Status.ToString(),
                    Workers = service.Value.Workers,
                })?.ToList();
        }
        async public static Task StartAllServicesAsync()
        {
            if (_items?.Count > 0)
            {
                foreach (var service in _items)
                {
                    if (service.Value.Status == EServiceStatus.Stopped)
                    {
                        await service.Value.StartAsync();
                    }
                }
            }
        }

        async public static Task StopAllServicesAsync()
        {
            if (_items?.Count > 0)
            {
                foreach (var service in _items)
                {
                    if (service.Value.Status == EServiceStatus.Running)
                    {
                        await service.Value.StopAsync();
                    }

                }
            }
        }

        async public static Task StartServiceAsync(string name)
        {
            if (_items?.ContainsKey(name) ?? false)
            {
                if (_items[name].Status == EServiceStatus.Stopped)
                {
                    await _items[name].StartAsync();
                }
            }
        }

        async public static Task StopServiceAsync(string name)
        {
            if (_items?.ContainsKey(name) ?? false)
            {
                if (_items[name].Status == EServiceStatus.Running)
                {
                    await _items[name].StopAsync();
                }
            }
        }
        public static List<object> GetWorkers(string serviceName)
        {
            List<object> result = new List<object>();
            try
            {
                if (_items?.ContainsKey(serviceName) ?? false)
                {
                    result = _items[serviceName].Workers;
                }
            }
            catch { }
            return result;
        }
        public static Task EnableWorkersAsync(string serviceName, List<string> workerNames)
        {
            try
            {
                if (_items?.ContainsKey(serviceName) ?? false)
                {
                    _items[serviceName].EnableWorkers(workerNames?.ToArray());
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        public static Task DisableWorkersAsync(string serviceName, List<string> workerNames)
        {
            try
            {
                if (_items?.ContainsKey(serviceName) ?? false)
                {
                    _items[serviceName].DisableWorkers(workerNames?.ToArray());
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        public static Task DisableAllWorkersAsync(string serviceName)
        {
            try
            {
                if (_items?.ContainsKey(serviceName) ?? false)
                {
                    _items[serviceName].DisableAllWorkers();
                }
            }
            catch { }
            return Task.CompletedTask;
        }
    }
}