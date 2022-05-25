using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VSSystem.IO
{
    public class CsvFile
    {
        const string _quot = "\"";
        const string _comment = "#";
        static readonly char[] _delimiters = new char[] { ',', '\t' };
        static string[] _ReadValuesPerLine(string line, char[] delimiters)
        {
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
                            value = line.Substring(lastIdx + 1, idx - lastIdx)?.Trim(' ', ',');
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
                value = line.Substring(lastIdx)?.Trim(' ', ',');
                if (value.StartsWith(_quot) && value.EndsWith(_quot))
                {
                    value = value.Substring(1, value.Length - 2);
                }
                values.Add(value);
            }
            catch { }
            return values.ToArray();
        }
        static void _ReadValues(Stream inputStream, Action<string[]> valuesAction, char[] delimiters, int startLineIndex = 0, int endLineIndex = -1)
        {
            try
            {
                if (delimiters == null || delimiters.Length == 0)
                {
                    delimiters = _delimiters;
                }
                char[] specialChars = new char[] { '\uFEFF' };
                using (var sr = new StreamReader(inputStream))
                {
                    int lineIdx = 0;
                    do
                    {
                        string line = sr.ReadLine();
                        if (line.StartsWith(_comment) || lineIdx < startLineIndex)
                        {
                            continue;
                        }
                        var values = _ReadValuesPerLine(line.Trim(specialChars), delimiters);
                        valuesAction?.Invoke(values);
                        lineIdx++;

                    } while (!sr.EndOfStream && (lineIdx < endLineIndex || endLineIndex == -1));
                    sr.Close();
                    sr.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static void _ReadValuesWithLineIndex(Stream inputStream, Action<int, string[]> valuesAction, char[] delimiters, int startLineIndex = 0, int endLineIndex = -1)
        {
            try
            {
                if (delimiters == null || delimiters.Length == 0)
                {
                    delimiters = _delimiters;
                }
                char[] specialChars = new char[] { '\uFEFF' };
                using (var sr = new StreamReader(inputStream))
                {
                    int lineIdx = 0;
                    int fileLineIdx = 0;
                    do
                    {
                        fileLineIdx++;
                        string line = sr.ReadLine();
                        if (line.StartsWith(_comment) || lineIdx < startLineIndex)
                        {
                            lineIdx++;
                            continue;
                        }
                        var values = _ReadValuesPerLine(line.Trim(specialChars), delimiters);
                        valuesAction?.Invoke(fileLineIdx, values);
                        lineIdx++;

                    } while (!sr.EndOfStream && (lineIdx < endLineIndex || endLineIndex == -1));
                    sr.Close();
                    sr.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static void _ReadValues(Stream inputStream, Action<string[]> valuesAction, int startLineIndex = 0, int endLineIndex = -1)
        {
            _ReadValues(inputStream, valuesAction, _delimiters, startLineIndex, endLineIndex);
        }
        static void _ReadValuesWithLineIndex(Stream inputStream, Action<int, string[]> valuesAction, int startLineIndex = 0, int endLineIndex = -1)
        {
            _ReadValuesWithLineIndex(inputStream, valuesAction, _delimiters, startLineIndex, endLineIndex);
        }
        public static string[] ReadHeader(string filePath, char[] delimiters = null)
        {
            string[] result = new string[0];
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    result = ReadHeader(fs, delimiters);
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public static string[] ReadHeader(Stream inputStream, char[] delimiters = null)
        {
            try
            {
                string[] result = new string[0];
                try
                {
                    using (var sr = new StreamReader(inputStream))
                    {
                        do
                        {
                            string line = sr.ReadLine();
                            if (line.StartsWith(_comment))
                            {
                                continue;
                            }
                            result = _ReadValuesPerLine(line, delimiters);
                            break;

                        } while (!sr.EndOfStream);
                        sr.Close();
                        sr.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string[] ReadHeader(byte[] dataBytes, char[] delimiters = null)
        {
            string[] result = default;
            try
            {
                using (var stream = new MemoryStream(dataBytes))
                {
                    result = ReadHeader(stream, delimiters);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch { }
            return result;
        }
        static public List<string[]> ReadRawData(Stream stream, char[] delimiters = null, int startLineIndex = 0, int endLineIndex = -1)
        {
            List<string[]> result = new List<string[]>();

            try
            {
                if (stream?.Length > 0)
                {
                    _ReadValues(stream, values =>
                    {
                        result.Add(values);
                    }, delimiters, startLineIndex, endLineIndex);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        static public List<string[]> ReadRawData(string filePath, char[] delimiters = null, int startLineIndex = 0, int endLineIndex = -1)
        {
            List<string[]> result = new List<string[]>();
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    result = ReadRawData(fs, delimiters, startLineIndex, endLineIndex);
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        static public List<TResult> ReadData<TResult>(Stream stream, string[] headers, int startLineIndex = 0, int endLineIndex = -1) where TResult : class
        {

            try
            {
                List<TResult> result = new List<TResult>();
                if (stream?.Length > 0)
                {
                    Type rType = typeof(TResult);
                    PropertyInfo[] props = rType.GetProperties();
                    Dictionary<string, int> rFields = props.Select((field, idx) => new { field.Name, idx })
                        .ToDictionary(ite => ite.Name, ite => ite.idx, StringComparer.InvariantCultureIgnoreCase);

                    if (headers?.Length > 0)
                    {
                        rFields = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
                        for (int i = 0; i < headers.Length; i++)
                        {
                            rFields[headers[i]] = i;
                        }
                    }
                    _ReadValues(stream, values =>
                    {
                        object value;
                        TResult res = Activator.CreateInstance<TResult>();
                        foreach (PropertyInfo prop in props)
                        {
                            if (!rFields.ContainsKey(prop.Name)) continue;
                            try
                            {
                                try { value = values[rFields[prop.Name]]; }
                                catch { value = null; }

                                value = value == DBNull.Value || value == null
                                        ? prop.PropertyType == typeof(string) ? Activator.CreateInstance(prop.PropertyType, string.Empty.ToArray())
                                        : prop.PropertyType == typeof(byte[]) ? new byte[0]
                                        : prop.PropertyType == typeof(char[]) ? new char[0]
                                        : Activator.CreateInstance(prop.PropertyType)
                                        : value;
                                if (prop.PropertyType.IsEnum) prop.SetValue(res, Enum.ToObject(prop.PropertyType, value), null);
                                else if (prop.PropertyType == typeof(Guid))
                                {
                                    Guid gValue;
                                    if (value.GetType() == typeof(byte[]))
                                        gValue = new Guid((byte[])value);
                                    else
                                        gValue = new Guid(value.ToString());
                                    prop.SetValue(res, gValue, null);
                                }
                                else if (prop.PropertyType == typeof(byte[]))
                                {
                                    prop.SetValue(res, value, null);
                                }
                                else if (prop.PropertyType == typeof(char[]))
                                {
                                    prop.SetValue(res, value, null);
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(value.ToString()))
                                    {
                                        prop.SetValue(res, Convert.ChangeType(value.ToString(), prop.PropertyType), null);
                                    }

                                }
                            }
                            catch //(Exception ex)
                            {
                            }
                        }
                        result.Add(res);
                    }, startLineIndex, endLineIndex);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public List<KeyValuePair<int, TResult>> ReadDataWithLineIndex<TResult>(Stream stream, string[] headers, int startLineIndex = 0, int endLineIndex = -1) where TResult : class
        {

            try
            {
                List<KeyValuePair<int, TResult>> result = new List<KeyValuePair<int, TResult>>();
                if (stream?.Length > 0)
                {
                    Type rType = typeof(TResult);
                    PropertyInfo[] props = rType.GetProperties();
                    Dictionary<string, int> rFields = props.Select((field, idx) => new { field.Name, idx })
                        .ToDictionary(ite => ite.Name, ite => ite.idx, StringComparer.InvariantCultureIgnoreCase);

                    if (headers?.Length > 0)
                    {
                        rFields = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
                        for (int i = 0; i < headers.Length; i++)
                        {
                            rFields[headers[i]] = i;
                        }
                    }
                    _ReadValuesWithLineIndex(stream, (lineIndex, values) =>
                    {
                        object value;
                        TResult res = Activator.CreateInstance<TResult>();
                        foreach (PropertyInfo prop in props)
                        {
                            if (!rFields.ContainsKey(prop.Name)) continue;
                            try
                            {
                                try { value = values[rFields[prop.Name]]; }
                                catch { value = null; }

                                value = value == DBNull.Value || value == null
                                        ? prop.PropertyType == typeof(string) ? Activator.CreateInstance(prop.PropertyType, string.Empty.ToArray())
                                        : prop.PropertyType == typeof(byte[]) ? new byte[0]
                                        : prop.PropertyType == typeof(char[]) ? new char[0]
                                        : Activator.CreateInstance(prop.PropertyType)
                                        : value;
                                if (prop.PropertyType.IsEnum) prop.SetValue(res, Enum.ToObject(prop.PropertyType, value), null);
                                else if (prop.PropertyType == typeof(Guid))
                                {
                                    Guid gValue;
                                    if (value.GetType() == typeof(byte[]))
                                        gValue = new Guid((byte[])value);
                                    else
                                        gValue = new Guid(value.ToString());
                                    prop.SetValue(res, gValue, null);
                                }
                                else if (prop.PropertyType == typeof(byte[]))
                                {
                                    prop.SetValue(res, value, null);
                                }
                                else if (prop.PropertyType == typeof(char[]))
                                {
                                    prop.SetValue(res, value, null);
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(value.ToString()))
                                    {
                                        prop.SetValue(res, Convert.ChangeType(value.ToString(), prop.PropertyType), null);
                                    }

                                }
                            }
                            catch //(Exception ex)
                            {
                            }
                        }
                        result.Add(new KeyValuePair<int, TResult>(lineIndex, res));
                    }, startLineIndex, endLineIndex);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static public List<TResult> ReadData<TResult>(string filePath, string[] headers, int startLineIndex = 0, int endLineIndex = -1) where TResult : class
        {
            List<TResult> result = new List<TResult>();
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    result = ReadData<TResult>(fs, headers, startLineIndex, endLineIndex);
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        static public List<TResult> ReadData<TResult>(byte[] dataBytes, string[] headers, int startLineIndex = 0, int endLineIndex = -1) where TResult : class
        {
            List<TResult> result = new List<TResult>();

            try
            {
                using (var stream = new MemoryStream(dataBytes))
                {
                    result = ReadData<TResult>(stream, headers, startLineIndex, endLineIndex);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch { }
            return result;
        }
        static public List<KeyValuePair<int, TResult>> ReadDataWithLineIndex<TResult>(byte[] dataBytes, string[] headers, int startLineIndex = 0, int endLineIndex = -1) where TResult : class
        {
            List<KeyValuePair<int, TResult>> result = new List<KeyValuePair<int, TResult>>();

            try
            {
                using (var stream = new MemoryStream(dataBytes))
                {
                    result = ReadDataWithLineIndex<TResult>(stream, headers, startLineIndex, endLineIndex);
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch { }
            return result;
        }
        static public int WriteData<TSource>(string fileName, string[] headers, IEnumerable<TSource> srcObjs, bool includeHeader)
            where TSource : class
        {
            int result = 0;
            if (srcObjs?.Count() > 0)
            {
                try
                {
                    Type rType = typeof(TSource);
                    PropertyInfo[] props = rType.GetProperties();
                    Dictionary<string, PropertyInfo> rFields = props.ToDictionary(ite => ite.Name, StringComparer.InvariantCultureIgnoreCase);
                    if (headers == null || headers.Length == 0)
                    {
                        headers = rFields.Keys.ToArray();
                    }
                    using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                        {
                            if (includeHeader)
                            {
                                string line = GetCsvLine(headers.ToList());
                                sw.WriteLine(line);
                            }
                            foreach (var srcObj in srcObjs)
                            {
                                var values = GetValues(srcObj, headers, rFields);
                                string line = GetCsvLine(values);
                                sw.WriteLine(line);
                                result++;
                            }
                            sw.Close();
                            sw.Dispose();
                        }
                        fs.Close();
                        fs.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }
        static string GetCsvLine(List<string> values)
        {
            try
            {
                string[] tVals = values.Select(v => v.Contains(',') || v.Contains('\"') ? $"{_quot}{v.Replace(_quot, _quot + _quot)}{_quot}" : v).ToArray();
                return string.Join(",", tVals);
            }
            catch
            {
                return string.Join(",", values.Select(ite => ""));
            }
        }
        static List<string> GetValues<TSource>(TSource src, string[] headers, Dictionary<string, PropertyInfo> rFields)
        {
            try
            {
                List<string> result = new List<string>();
                foreach (var header in headers)
                {
                    if (rFields.ContainsKey(header))
                    {
                        var field = rFields[header];
                        string value = field.GetValue(src)?.ToString() ?? "";
                        result.Add(value);
                    }
                    else
                    {
                        result.Add("");
                    }
                }
                return result;
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
