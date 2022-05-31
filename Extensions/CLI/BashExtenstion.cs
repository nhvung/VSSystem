using System.Collections.Generic;
using System.Diagnostics;
using System;
namespace VSSystem.Extensions
{
    public class BashExtension : CLIExtension
    {
        public BashExtension() : base("/bin/bash")
        {
            _prefixCommand = "-c";
        }
    }
}