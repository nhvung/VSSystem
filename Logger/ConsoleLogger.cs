using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VSSystem.Logger
{
    class ConsoleLogger : ALogger
    {
        protected override Task _ClearAsync()
        {
            try
            {
                Console.Clear();
            }
            catch { }
            return Task.CompletedTask;
        }

        Task _Log(string name, string content, string sLogLevel, ConsoleColor logColor)
        {
            try
            {
                lock (_lockObj)
                {
                    DateTime now = DateTime.Now;
                    Console.ForegroundColor = logColor;
                    Console.Write(sLogLevel);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": " + name);
                    Console.WriteLine(now.ToString("MM/dd/yyyy HH:mm:ss") + " - " + content);
                    Console.WriteLine("======================================================");
                    Console.WriteLine(Environment.NewLine);
                    Thread.Sleep(100);
                }
            }
            catch { }
            return Task.CompletedTask;
        }

        Task _Log(string name, string tagName, string content, string sLogLevel, ConsoleColor logColor)
        {
            try
            {
                lock (_lockObj)
                {
                    DateTime now = DateTime.Now;
                    Console.ForegroundColor = logColor;
                    Console.Write(sLogLevel);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": " + name);
                    Console.WriteLine("Tag: " + tagName);
                    Console.WriteLine(now.ToString("MM/dd/yyyy HH:mm:ss") + " - " + content);
                    Console.WriteLine("======================================================");
                    Console.WriteLine(Environment.NewLine);
                    Thread.Sleep(100);
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        Task _LogCsv(string name, string[] headers, string[] values)
        {
            try
            {
                lock (_lockObj)
                {
                    DateTime now = DateTime.Now;

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(": " + name);
                    List<string> headerObjs = new List<string>(), valueObjs = new List<string>();

                    int width = Math.Max("Log Time".Length, now.ToString("MM/dd/yyyy HH:mm:ss").Length);
                    headerObjs.Add(string.Format("{0,-" + width + "}", "Log Time"));
                    valueObjs.Add(string.Format("{0,-" + width + "}", now.ToString("MM/dd/yyyy HH:mm:ss")));

                    for (int i = 0; i < headers.Length && i < values.Length; i++)
                    {
                        width = Math.Max(headers[i].Length, values[i].Length);
                        headerObjs.Add(string.Format("{0,-" + width + "}", headers[i]));
                        valueObjs.Add(string.Format("{0,-" + width + "}", values[i]));
                    }



                    string sHeader = string.Join("|", headerObjs);
                    string sValue = string.Join("|", valueObjs);

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(sHeader);
                    Console.WriteLine(sValue);
                    Console.WriteLine("======================================================");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        protected override Task _LogAsync(string name, string content, ELogLevel logLevel, string component)
        {
            string sLogLevel = logLevel.ToString().ToLower();
            if (logLevel == ELogLevel.Info)
            {
                return _Log(name, content, sLogLevel, ConsoleColor.Green);
            }
            else if (logLevel == ELogLevel.Debug)
            {
                return _Log(name, content, sLogLevel, ConsoleColor.Blue);
            }
            else if (logLevel == ELogLevel.Warning)
            {
                return _Log(name, content, sLogLevel, ConsoleColor.Yellow);
            }
            else if (logLevel == ELogLevel.Error)
            {
                return _Log(name, content, sLogLevel, ConsoleColor.Red);
            }
            return Task.CompletedTask;
        }

        protected override Task _LogWithTagAsync(string name, string tagName, string content, ELogLevel logLevel, string component)
        {
            string sLogLevel = logLevel.ToString().ToLower();
            if (logLevel == ELogLevel.Info)
            {
                return _Log(name, tagName, content, sLogLevel, ConsoleColor.Green);
            }
            else if (logLevel == ELogLevel.Debug)
            {
                return _Log(name, tagName, content, sLogLevel, ConsoleColor.Blue);
            }
            else if (logLevel == ELogLevel.Warning)
            {
                return _Log(name, tagName, content, sLogLevel, ConsoleColor.Yellow);
            }
            else if (logLevel == ELogLevel.Error)
            {
                return _Log(name, tagName, content, sLogLevel, ConsoleColor.Red);
            }
            return Task.CompletedTask;
        }

        protected override Task _LogCsvAsync(string name, string[] headers, string[] values, string component)
        {
            return _LogCsv(name, headers, values);
        }
    }
}
