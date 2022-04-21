using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VSSystem.Logger;

namespace VSSystem.ServiceProcess.Workers
{
    public abstract class AWorker : IWorker
    {
        protected string _name;
        public string Name { get { return _name; } }
        protected bool _enabled;
        public bool Enabled { get { return _enabled; } set { _enabled = value; } }
        protected EWorkerType _type;
        public EWorkerType Type { get { return _type; } }

        protected string _ServiceName;
        public string ServiceName { get { return _ServiceName; } }
        protected DirectoryInfo WorkingFolder { get { return GlobalVariables.WorkingFolder; } }
        protected ALogger _logger;
        public ALogger Logger { get { return _logger; } }
        protected AWorker(AWorkerStartInfo startInfo, ALogger logger = null)
        {
            _type = EWorkerType.Base;
            _name = startInfo.Name;
            _enabled = startInfo.Enabled;
            _ServiceName = startInfo.ServiceName;
            _logger = logger;
        }

        protected object _lockObj;
        public Task RunAsync(CancellationToken cancellationToken)
        {
            //this.LogDebug(string.Format("{0} worker running.", _name));
            _lockObj = new object();
            return _RunAsync(cancellationToken);
        }
        protected abstract Task _RunAsync(CancellationToken cancellationToken);

        public override string ToString()
        {
            return _name;
        }
        protected virtual bool _ValuesInitialized() { return true; }
    }
}
