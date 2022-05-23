using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VSSystem.Configuration
{
    public class IniConfiguration : AConfiguration
    {
        public IniConfiguration() : base() { }
        public IniConfiguration(string path, params string[] defaultSections)
            : base(path, defaultSections)
        {
        }
        protected override void _initializeValues()
        {
            try
            {
                Dictionary<string, string> cSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                FileInfo file = new FileInfo(_path);
                if (!file.Exists)
                {
                    if (!file.Directory.Exists) file.Directory.Create();
                    File.WriteAllText(_path, "");
                }
                StreamReader reader = new StreamReader(_path);
                string line = null;
                int commandEmptyIdx = 0, commandSemicolonIdx = 0, commandHashIdx = 0;
                cSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                _items["_NonSection_"] = cSection;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        if (cSection != null)
                        {
                            cSection["_CommandEmpty_" + commandEmptyIdx] = line;
                            commandEmptyIdx++;
                            continue;
                        }
                    }
                    else if (line.StartsWith(";"))
                    {
                        if (cSection != null)
                        {
                            cSection["_CommandSemiColonIdx_" + commandSemicolonIdx] = line;
                            commandSemicolonIdx++;
                            continue;
                        }
                    }
                    else if (line.StartsWith("#"))
                    {
                        if (cSection != null)
                        {
                            cSection["_CommandHashIdx_" + commandHashIdx] = line;
                            commandHashIdx++;
                            continue;
                        }
                    }
                    else if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        cSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                        _items[line.Substring(1, line.Length - 2)] = cSection;
                        commandEmptyIdx = 0;
                        commandSemicolonIdx = 0;
                        commandHashIdx = 0;
                        continue;
                    }
                    int idx = line.IndexOf('=');
                    if (idx == -1) cSection[line] = "";
                    else
                    {
                        cSection[line.Substring(0, idx)] = line.Substring(idx + 1);
                    }
                }
                reader.Close();
                reader.Dispose();

                if (_defaultSections?.Length > 0)
                {
                    foreach (var section in _items.Keys)
                    {
                        if (section.Equals("_NonSection_", StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }
                        if (!_defaultSections.Contains(section, StringComparer.InvariantCultureIgnoreCase))
                        {
                            _items.Remove(section);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("_initializeValues Exception: " + ex.Message);
            }
        }

        protected override void _ApplyChange()
        {
            try
            {
                FileInfo configFile = new FileInfo(_path);
                if (configFile.Exists)
                {
                    List<string> lines = _getConfigLines();
                    if (lines?.Count > 0)
                    {
                        File.WriteAllLines(configFile.FullName, lines);
                    }
                }
            }
            catch { }
        }

        List<string> _getConfigLines()
        {
            List<string> lines = new List<string>();
            try
            {
                if (_items?.Count > 0)
                {
                    foreach (var sectionObj in _items)
                    {
                        if (sectionObj.Key.Equals("_NonSection_"))
                        {

                        }
                        else
                        {
                            lines.Add("[" + sectionObj.Key + "]");
                            foreach (var kv in sectionObj.Value)
                            {
                                if (
                                    kv.Key.StartsWith("_CommandSemiColonIdx_", StringComparison.InvariantCultureIgnoreCase) ||
                                    kv.Key.StartsWith("_CommandHashIdx_", StringComparison.InvariantCultureIgnoreCase)
                                    )
                                {
                                    lines.Add(kv.Value);
                                }
                                else if (kv.Key.StartsWith("_CommandEmpty_", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    continue;
                                }
                                else
                                {
                                    lines.Add(string.Format("{0}={1}", kv.Key, kv.Value));
                                }
                            }
                            lines.Add(string.Empty);
                        }
                    }
                }
            }
            catch { }
            return lines;
        }
        public override string ToString()
        {
            var lines = _getConfigLines();
            return string.Join(Environment.NewLine, lines);
        }
        protected override void _initializeValuesFromString(string value)
        {
            try
            {
                Dictionary<string, string> cSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                FileInfo file = new FileInfo(_path);
                if (!file.Exists)
                {
                    if (!file.Directory.Exists) file.Directory.Create();
                    File.WriteAllText(_path, "");
                }
                var reader = new StringReader(value);
                string line = null;
                int commandEmptyIdx = 0, commandSemicolonIdx = 0, commandHashIdx = 0;
                cSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                _items["_NonSection_"] = cSection;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        if (cSection != null)
                        {
                            cSection["_CommandEmpty_" + commandEmptyIdx] = line;
                            commandEmptyIdx++;
                            continue;
                        }
                    }
                    else if (line.StartsWith(";"))
                    {
                        if (cSection != null)
                        {
                            cSection["_CommandSemiColonIdx_" + commandSemicolonIdx] = line;
                            commandSemicolonIdx++;
                            continue;
                        }
                    }
                    else if (line.StartsWith("#"))
                    {
                        if (cSection != null)
                        {
                            cSection["_CommandHashIdx_" + commandHashIdx] = line;
                            commandHashIdx++;
                            continue;
                        }
                    }
                    else if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        cSection = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                        _items[line.Substring(1, line.Length - 2)] = cSection;
                        commandEmptyIdx = 0;
                        commandSemicolonIdx = 0;
                        commandHashIdx = 0;
                        continue;
                    }
                    int idx = line.IndexOf('=');
                    if (idx == -1) cSection[line] = "";
                    else
                    {
                        cSection[line.Substring(0, idx)] = line.Substring(idx + 1);
                    }
                }
                reader.Close();
                reader.Dispose();

                if (_defaultSections?.Length > 0)
                {
                    foreach (var section in _items.Keys)
                    {
                        if (section.Equals("_NonSection_", StringComparison.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }
                        if (!_defaultSections.Contains(section, StringComparer.InvariantCultureIgnoreCase))
                        {
                            _items.Remove(section);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("_initializeValues Exception: " + ex.Message);
            }
        }
    }
}
