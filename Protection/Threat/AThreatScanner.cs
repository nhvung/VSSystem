using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VSSystem.Protection.Threat
{
    public abstract class AThreatScanner : IThreatScanner
    {

        protected string _ExecuteFileName;
        public string ExecuteFileName { get { return _ExecuteFileName; } set { _ExecuteFileName = value; } }
        protected DirectoryInfo _workingFolder;
        protected AThreatScanner(string workingFolderPath, string executeFileName)
        {
            _workingFolder = new DirectoryInfo(workingFolderPath);
            _ExecuteFileName = executeFileName;
        }
        public ScanResult ScanFile(string filePath, int timeout = 30000)
        {
            if (Environment.OSVersion.VersionString?.IndexOf("windows", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return _ScanFile_Windows(filePath, timeout);
            }
            return ScanResult.Unknown;
        }
        protected abstract ScanResult _ScanFile_Windows(string filePath, int timeout);
    }
}
