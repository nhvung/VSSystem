using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.ServiceProcess.Workers
{
    public enum EWorkerType : int
    {
        Base = 100,

        Interval = 200,
        Pool = 201,

        Schedule = 300,

    }
}
