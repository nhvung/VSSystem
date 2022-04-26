using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Logger
{
    public enum ELogMode : byte
    {
        None = 0,
        Console = 1,
        File = 2,
        Api = 3,
    }
}
