using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSSystem.ServiceProcess.Workers
{
    public class IntervalWorkerStartInfo : AWorkerStartInfo
    {

        int _Interval;
        public int Interval { get { return _Interval; } set { _Interval = value; } }

        int _NumberOfThreads;
        public int NumberOfThreads { get { return _NumberOfThreads; } set { _NumberOfThreads = value; } }

        EWorkerIntervalUnit _IntervalUnit;
        public EWorkerIntervalUnit IntervalUnit { get { return _IntervalUnit; } set { _IntervalUnit = value; } }
        public IntervalWorkerStartInfo() : base()
        {

        }
        public IntervalWorkerStartInfo(string name, bool enabled, string serviceName, int interval, int numberOfThreads, EWorkerIntervalUnit intervalUnit) 
            : base(name, enabled, serviceName)
        {
            _Interval = interval;
            _NumberOfThreads = numberOfThreads;
            _IntervalUnit = intervalUnit;
        }
    }
}
