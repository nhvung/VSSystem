using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSSystem.Models
{
    public enum EComponentType : byte
    {
        Undefine = 0,
        Service = 1,
        Web = 2,
        Tool = 4,
        Host = 8,
        SubHost = 16
    }
}
