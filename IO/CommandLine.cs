using System;
using System.Collections.Generic;
using System.Linq;

namespace VSSystem.IO
{
    public class CommandLine
    {
        const string _quot = "\"";
        static readonly char[] _delimiters = new char[] { ' ', '=' };
        static string[] _ReadValuesPerLine(string line, char[] delimiters = default)
        {

            //Console.WriteLine(line);

            if (delimiters == null || delimiters.Length == 0)
            {
                delimiters = _delimiters;
            }
            List<string> values = new List<string>();
            try
            {
                int lastIdx = -1;
                int idx = 0;
                bool inText = false;
                string value;
                while (idx < line.Length)
                {
                    if (line[idx] == '\"')
                    {
                        inText = !inText;
                    }
                    else if (delimiters.Contains(line[idx]))
                    {
                        if (!inText)
                        {
                            value = line.Substring(lastIdx + 1, idx - lastIdx)?.Trim(' ', '=');
                            if (value.StartsWith(_quot) && value.EndsWith(_quot))
                            {
                                value = value.Substring(1, value.Length - 2)?.Replace(_quot + _quot, _quot);
                            }
                            values.Add(value);
                            lastIdx = idx;
                        }
                    }
                    idx++;
                }
                if (lastIdx > -1)
                {
                    value = line.Substring(lastIdx)?.Trim(' ', '=');
                    if (value.StartsWith(_quot) && value.EndsWith(_quot))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                }
                else
                {
                    value = line;
                }
                values.Add(value);
            }
            catch //(Exception ex)
            {

            }
            return values.ToArray();
        }

        public static Dictionary<string, string> GetCommandLineArgs(string[] keyWords = default, string[] additionalArgs = default)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            try
            {
                List<string> inputArgs = new List<string>();

                string[] cmdArgs = Environment.GetCommandLineArgs();
                if (cmdArgs?.Length > 0)
                {
                    foreach (var arg in cmdArgs)
                    {
                        string tArgs = arg;
                        if (arg.Contains(' '))
                        {
                            tArgs = _quot + arg + _quot;
                        }
                        inputArgs.Add(tArgs);
                    }
                }
                if (additionalArgs?.Length > 0)
                {
                    foreach (var arg in additionalArgs)
                    {
                        string tArgs = arg;
                        if (arg.Contains(' '))
                        {
                            tArgs = _quot + arg + _quot;
                        }
                        inputArgs.Add(tArgs);
                    }
                }
                string mergeArgs = string.Join(" ", inputArgs);

                var tArgObjs = _ReadValuesPerLine(mergeArgs);

                if (tArgObjs?.Length > 0)
                {
                    int i = 0;
                    while (i < tArgObjs.Length)
                    {
                        string keyWord = tArgObjs[i].Trim(' ');

                        result[keyWord] = string.Empty;
                        if (keyWords?.Length > 0)
                        {
                            if (keyWords.Contains(keyWord, StringComparer.InvariantCultureIgnoreCase))
                            {
                                int j = i + 1;

                                if (j < tArgObjs.Length)
                                {
                                    string value = tArgObjs[j];
                                    result[keyWord] = value;
                                    i += 2;
                                }
                                else
                                {
                                    i++;
                                }
                            }
                            else
                            {
                                i++;
                            }
                        }
                        else
                        {
                            i++;
                        }
                    }
                }


            }
            catch { }

            return result;
        }
    }
}