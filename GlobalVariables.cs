using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VSSystem.Logger;

namespace VSSystem
{
    public class GlobalVariables
    {

        static string _HostName;
        public static string HostName { get { return _HostName; } set { _HostName = value; } }

        static string _HostIPAddress;
        public static string HostIPAddress { get { return _HostIPAddress; } set { _HostIPAddress = value; } }

        //static ALogger _Logger;
        //public static ALogger Logger { get { return _Logger; } set { _Logger = value; } }

        static DirectoryInfo _WorkingFolder;
        public static DirectoryInfo WorkingFolder { get { return _WorkingFolder; } set { _WorkingFolder = value; } }

        static Models.OnlineConfigurationInfo _OnlineConfig;
        public static Models.OnlineConfigurationInfo OnlineConfig { get { return _OnlineConfig; } set { _OnlineConfig = value; } }

        public static void InitializeValues()
        {

            try
            {

            }
            catch { }
        }
    }
}
