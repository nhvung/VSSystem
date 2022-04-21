using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSSystem.Models
{
    public class OnlineConfigurationInfo
    {
        string _ApiUrl;
        public string ApiUrl { get { return _ApiUrl; } set { _ApiUrl = value; } }

        int _Timeout;
        public int Timeout { get { return _Timeout; } set { _Timeout = value; } }

        string _ThisServerUrl;
        public string ThisServerUrl { get { return _ThisServerUrl; } set { _ThisServerUrl = value; } }

        bool _IncludeLogger;
        public bool IncludeLogger { get { return _IncludeLogger; } set { _IncludeLogger = value; } }

    }
}
