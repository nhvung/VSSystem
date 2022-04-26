using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.IO.SourceCode
{
    public class TargetFramework
    {
        public const string NetCore31 = "netcoreapp3.1";
        public const string Net5 = "net5.0";
        public const string Net6 = "net6.0";
    }

    public enum ETargetFramework : int
    {
        DOTNETCORE31,
        DOTNET5,
        DOTNET6
    }
}
