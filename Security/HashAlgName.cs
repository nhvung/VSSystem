using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Security
{
    public enum HashAlgName : byte
    {
        MD5 = 1,
        SHA1 = 2,
        SHA256 = 3,
        SHA384 = 4,
        SHA512 = 5
    }
}
