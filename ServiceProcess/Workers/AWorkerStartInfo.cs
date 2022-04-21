using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSSystem.ServiceProcess.Workers
{
    public class AWorkerStartInfo
    {

        string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        bool _Enabled;
        public bool Enabled { get { return _Enabled; } set { _Enabled = value; } }

        string _ServiceName;
        public string ServiceName { get { return _ServiceName; } set { _ServiceName = value; } }

        public AWorkerStartInfo()
        {
            _Enabled = false;
            _Name = string.Empty;
            _ServiceName = string.Empty;
        }
        public AWorkerStartInfo(string name, bool enabled, string serviceName)
        {
            _Enabled = enabled;
            _Name = name;
            _ServiceName = serviceName;
        }
    }
}
