using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.ServiceProcess
{
    public class ServiceStartInfo
    {

        string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        string _RootComponentName;
        public string RootComponentName { get { return _RootComponentName; } set { _RootComponentName = value; } }

        int _Server_ID;
        public int Server_ID { get { return _Server_ID; } set { _Server_ID = value; } }

        string _PrivateKey;
        public string PrivateKey { get { return _PrivateKey; } set { _PrivateKey = value; } }

        string[] _DefaultSections;
        public string[] DefaultSections { get { return _DefaultSections; } set { _DefaultSections = value; } }

        Action<string> _InfoLogAction;
        public Action<string> InfoLogAction { get { return _InfoLogAction; } set { _InfoLogAction = value; } }

        Action<string> _DebugLogAction;
        public Action<string> DebugLogAction { get { return _DebugLogAction; } set { _DebugLogAction = value; } }

        Action<string> _WarningLogAction;
        public Action<string> WarningLogAction { get { return _WarningLogAction; } set { _WarningLogAction = value; } }

        Action<Exception> _ErrorLogAction;
        public Action<Exception> ErrorLogAction { get { return _ErrorLogAction; } set { _ErrorLogAction = value; } }
        public ServiceStartInfo()
        {
            _Name = string.Empty;
            _RootComponentName = string.Empty;
            _Server_ID = 0;
            _PrivateKey = string.Empty;
            _DefaultSections = null;
            _InfoLogAction = null;
            _DebugLogAction = null;
            _WarningLogAction = null;
            _ErrorLogAction = null;
        }
    }
}
