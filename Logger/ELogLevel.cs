using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Logger
{
    public enum ELogLevel : byte
    {
        None = 0,
        Info = 1,
        Debug = 2,
        Warning = 4,
        Error = 8,
        Csv = 16,
        Full = 31
    }
}
