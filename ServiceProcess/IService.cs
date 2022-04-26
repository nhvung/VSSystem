using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VSSystem.Logger;

namespace VSSystem.ServiceProcess
{
    public interface IService
    {
        string Name { get; }
        EServiceStatus Status { get; }
        Task StartAsync();
        Task StopAsync();
        void EnableWorkers(params string[] names);
        void EnableAllWorkers();
        void DisableWorkers(params string[] names);
        void DisableAllWorkers();
        List<object> Workers { get; }
        ALogger Logger { get; }
    }
}
