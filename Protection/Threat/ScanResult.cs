using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VSSystem.Protection.Threat
{
    public enum ScanResult : int
    {
        Unknown = 0,
        NoThreatFound = 1,
        ThreatFound = 2,
        FileNotFound = 3,
        Error = 4,
        Timeout = 5,
    }
}
