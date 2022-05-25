using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Logger
{
    class ApiAddLogRequest
    {
        int _Server_ID;
        public int Server_ID { get { return _Server_ID; } set { _Server_ID = value; } }

        int _Component_ID;
        public int Component_ID { get { return _Component_ID; } set { _Component_ID = value; } }

        string _LogType;
        public string LogType { get { return _LogType; } set { _LogType = value; } }

        string _Contents;
        public string Contents { get { return _Contents; } set { _Contents = value; } }

        long _LogTicks;
        public long LogTicks { get { return _LogTicks; } set { _LogTicks = value; } }

        string _LogName;
        public string LogName { get { return _LogName; } set { _LogName = value; } }

        public ApiAddLogRequest()
        {
            _Server_ID = -1;
            _Component_ID = -1;
            _LogType = string.Empty;
            _Contents = string.Empty;
            _LogTicks = 0;
            _LogName = string.Empty;
        }
    }
}
