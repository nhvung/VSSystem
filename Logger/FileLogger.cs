using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSystem.Logger
{
    class FileLogger : ALogger
    {
        const int MaxLength = 5242880;
        DirectoryInfo _workingFolder;
        public FileLogger(string workingFolderPath = null)
        {

            try
            {
                if(string.IsNullOrWhiteSpace(workingFolderPath))
                {
                    _workingFolder = new DirectoryInfo(Directory.GetCurrentDirectory());
                }
                else
                {
                    _workingFolder = new DirectoryInfo(workingFolderPath);
                }
            }
            catch
            {
                _workingFolder = new DirectoryInfo(Directory.GetCurrentDirectory());
            }
        }
        protected override Task _ClearAsync()
        {
            try
            {
                DateTime now = DateTime.Now;
                DirectoryInfo folder = new DirectoryInfo(_workingFolder.FullName + "/log");
                if (folder.Exists)
                {
                    DirectoryInfo bkFolder = new DirectoryInfo(folder.Parent.FullName + string.Format("/log_old/{0:yyyyMMdd.HHmmss}", now));
                    if (!bkFolder.Parent.Exists)
                    {
                        bkFolder.Parent.Create();
                    }
                    folder.MoveTo(bkFolder.FullName);
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        async Task _Log(string name, string content, string sLogLevel)
        {

            try
            {
                DirectoryInfo folder = new DirectoryInfo(_workingFolder.FullName + "/log");
                string type = string.IsNullOrEmpty(sLogLevel) ? "" : "/" + sLogLevel;
                string logPath = folder.FullName + type + "/" + name + ".log";
                FileInfo logFile = new FileInfo(logPath);
                if (logFile.Exists && logFile.Length >= MaxLength)
                {
                    DirectoryInfo bkFolder = new DirectoryInfo(folder.Parent.FullName + "/log_bk");
                    FileInfo logFileBK = new FileInfo(bkFolder.FullName + type + "/" + name + "_" + logFile.LastAccessTime.ToString("yyyyMMdd") + ".log");
                    if (!logFileBK.Directory.Exists) logFileBK.Directory.Create();
                    logFile.MoveTo(logFileBK.FullName);
                }
                if (!logFile.Directory.Exists) logFile.Directory.Create();
                using (FileStream fs = logFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    fs.Seek(0, SeekOrigin.End);
                    using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        DateTime now = DateTime.Now;
                        string contents = string.Format("{0:MM/dd/yyyy HH:mm:ss} - {1}", now, content);
                        try
                        {
                            await writer.WriteLineAsync(contents);
                            await writer.WriteLineAsync("============================================");
                        }
                        catch { }
                        writer.Close();
                    }
                    fs.Close();
                }
            }
            catch { }
        }
        const string _quot = "\"";
        string _GetCsvLine(IEnumerable<string> values)
        {
            try
            {
                string[] tVals = values.Select(v => v.Contains(',') ? $"{_quot}{v.Replace(_quot, _quot + _quot)}{_quot}" : v).ToArray();
                return string.Join(",", tVals);
            }
            catch
            {
                return string.Join(",", values.Select(ite => ""));
            }
        }
        async Task _LogCsv(string name, string[] headers, string[] values, string component)
        {

            try
            {
                DirectoryInfo folder = new DirectoryInfo(_workingFolder.FullName + "/log");
                string type = "/csv";
                string logPath = folder.FullName + type + "/" + name + ".csv";
                FileInfo logFile = new FileInfo(logPath);
                if (logFile.Exists && logFile.Length >= MaxLength)
                {
                    DirectoryInfo bkFolder = new DirectoryInfo(folder.Parent.FullName + "/log_bk");
                    FileInfo logFileBK = new FileInfo(bkFolder.FullName + type + "/" + name + "_" + logFile.LastAccessTime.ToString("yyyyMMdd") + ".csv");
                    if (!logFileBK.Directory.Exists) logFileBK.Directory.Create();
                    logFile.MoveTo(logFileBK.FullName);
                }
                if (!logFile.Directory.Exists) logFile.Directory.Create();

                List<string> csvHeaderObjs = new List<string>() { "LogTime" };
                csvHeaderObjs.AddRange(headers);
                string headerString = _GetCsvLine(csvHeaderObjs);
                using (FileStream fs = logFile.Open(FileMode.OpenOrCreate, FileAccess.Read)) 
                {
                    using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                    {
                        string line = null;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            if (line.Equals(headerString, StringComparison.InvariantCultureIgnoreCase))
                            {
                                headerString = string.Empty;
                            }
                            break;
                        }
                        reader.Close();
                    }
                    fs.Close();
                }
                using (FileStream fs = logFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        if(!string.IsNullOrWhiteSpace(headerString))
                        {
                            await writer.WriteLineAsync(headerString);
                        }

                        fs.Seek(0, SeekOrigin.End);

                        DateTime now = DateTime.Now;
                        string timeValue = string.Format("{0:MM/dd/yyyy HH:mm:ss}", now);
                        List<string> csvValueObjs = new List<string>() { timeValue };
                        csvValueObjs.AddRange(values);
                        string valueString = _GetCsvLine(csvValueObjs);
                        try
                        {
                            await writer.WriteLineAsync(valueString);
                        }
                        catch { }
                        writer.Close();
                    }
                    fs.Close();
                }
            }
            catch { }
        }
        protected override Task _LogAsync(string name, string content, ELogLevel logLevel, string component)
        {
            string sLogLevel = logLevel.ToString().ToLower();
            return _Log(name, content, sLogLevel);
        }

        protected override Task _LogCsvAsync(string name, string[] headers, string[] values, string component)
        {
            return _LogCsv(name, headers, values, component);
        }
    }
}
