using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSSystem.Logger;

namespace VSSystem.ServiceProcess.Workers
{
    public interface IWorker
    {
        Task RunAsync(CancellationToken cancellationToken = default);
        string Name { get; }
        string ServiceName { get; }
        bool Enabled { get; set; }
        EWorkerType Type { get; }
        ALogger Logger { get; }
    }
}
