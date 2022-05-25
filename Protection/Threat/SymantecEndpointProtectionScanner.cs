using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace VSSystem.Protection.Threat
{
    public class SymantecEndpointProtectionScanner : AThreatScanner
    {
       

        public SymantecEndpointProtectionScanner(string workingFolderPath, string executeFileName = "DoScan.exe") : base(workingFolderPath, executeFileName)
        {
        }
        
        protected override ScanResult _ScanFile_Windows(string filePath, int timeout)
        {
            if(string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return ScanResult.FileNotFound;
            }
            ScanResult result = ScanResult.Unknown;
            try
            {
                if(_workingFolder?.Exists ?? false)
                {
                    ProcessStartInfo psi = new ProcessStartInfo(_workingFolder.FullName + "/DoScan.exe");
                    psi.WorkingDirectory = _workingFolder.FullName;

                    psi.Arguments = $"/scanfile \"{filePath}\"";
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    psi.ErrorDialog = false;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;

                    using(var process = Process.Start(psi))
                    {
                        process.WaitForExit(timeout);

                        if(!process.HasExited)
                        {
                            process.Kill();
                            result = ScanResult.Timeout;
                        }
                        else
                        {
                            if(File.Exists(filePath))
                            {
                                result = ScanResult.NoThreatFound;
                            }
                            else
                            {
                                result = ScanResult.ThreatFound;
                            }
                        }
                       

                        process.Dispose();
                    }
                }
            }
            catch
            {
                result = ScanResult.Error;
            }
            return result;
        }

        
    }
}
