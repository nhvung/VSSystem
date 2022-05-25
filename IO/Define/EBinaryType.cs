using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.IO.Define
{
    public enum EBinaryType : byte
    {
        Undefine = 0,
        Boolean = 1,
        Byte = 2,
        Char = 3,
        Int16 = 4,
        UInt16 = 5,
        Int32 = 6,
        UInt32 = 7,
        Int64 = 8,
        UInt64 = 9,
        Float = 10,
        Double = 11,
        Decimal = 12,
        String = 13,
        Bytes = 14,
        SByte = 15
    }
}
