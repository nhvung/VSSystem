using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace VSSystem.Protection.Threat
{
    public class WindowsDefenderScanner : AThreatScanner
    {
       
        public WindowsDefenderScanner(string workingFolderPath, string executeFileName = "MpCmdRun.exe") : base(workingFolderPath, executeFileName)
        {
        }
        
        protected override ScanResult _ScanFile_Windows(string filePath, int timeout)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return ScanResult.FileNotFound;
            }
            ScanResult result = ScanResult.Unknown;
            try
            {
                if (_workingFolder?.Exists ?? false)
                {
                    ProcessStartInfo psi = new ProcessStartInfo(_workingFolder.FullName + "/" + ExecuteFileName);
                    psi.WorkingDirectory = _workingFolder.FullName;

                    psi.Arguments = $"-scan -scantype 3 -file \"{filePath}\" -DisableRemediation";
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;
                    psi.ErrorDialog = false;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;

                    using (var process = Process.Start(psi))
                    {
                        process.WaitForExit(timeout);

                        if (!process.HasExited)
                        {
                            process.Kill();
                            result = ScanResult.Timeout;
                        }
                        else
                        {
                            if (process.ExitCode == 0)
                            {
                                result = ScanResult.NoThreatFound;
                            }
                            else if (process.ExitCode == 2)
                            {
                                result = ScanResult.ThreatFound;
                                try
                                {
                                    //File.Delete(filePath);
                                }
                                catch { }
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
