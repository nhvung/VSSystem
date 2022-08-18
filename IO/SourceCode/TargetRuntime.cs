using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.IO.SourceCode
{
    public class TargetRuntime
    {
        public const string Windows_x64 = "win-x64";
        public const string Linux_x64 = "linux-x64";
        public const string OSX_x64 = "osx-x64";
        public const string Portal = "";
    }
    public enum ETargetRuntime : int
    {
        Windows_x64,
        Linux_x64,
        OSX_x64,
        Portal
    }
}
