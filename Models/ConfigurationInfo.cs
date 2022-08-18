using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VSSystem.Models
{
    public class ConfigurationInfo
    {
        int _Server_ID;
        public int Server_ID { get { return _Server_ID; } set { _Server_ID = value; } }
        List<string> _IPAddresses;
        public List<string> IPAddresses { get { return _IPAddresses; } set { _IPAddresses = value; } }
        int _Component_ID;
        public int Component_ID { get { return _Component_ID; } set { _Component_ID = value; } }
        string _ComponentName;
        public string ComponentName { get { return _ComponentName; } set { _ComponentName = value; } }
        string _Path;
        public string Path { get { return _Path; } set { _Path = value; } }
        string _Base64Value;
        public string Base64Value { get { return _Base64Value; } set { _Base64Value = value; } }
        byte _Status;
        public byte Status { get { return _Status; } set { _Status = value; } }
        long _LastUpdatedTicks;
        public long LastUpdatedTicks { get { return _LastUpdatedTicks; } set { _LastUpdatedTicks = value; } }
    }
}
