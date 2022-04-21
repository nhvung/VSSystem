using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSSystem.Configuration
{
    public abstract class AConfiguration
    {
        protected string _path;
        protected string[] _defaultSections;
        protected AConfiguration(string path, params string[] defaultSections)
        {
            _path = path;
            _defaultSections = defaultSections;
            _items = new Dictionary<string, Dictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);
            _initializeValues();
        }
        protected AConfiguration()
        {
            _path = string.Empty;
            _defaultSections = null;
            _items = new Dictionary<string, Dictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);
        }

        protected Dictionary<string, Dictionary<string, string>> _items;
        protected virtual void _initializeValues() { }
        protected virtual void _initializeValuesFromString(string value) { }
        protected virtual void _addConfig(string key, string value, string section)
        {
            if (!_items.ContainsKey(section))
            {
                _items[section] = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            }
            _items[section][key] = value;
        }
        public string[] Sections { get { return _items?.Keys?.Where(ite => !ite.Equals("_NonSection_", StringComparison.InvariantCultureIgnoreCase)).ToArray(); } }

        public Dictionary<string, string> this[string section] { get { return _items?.ContainsKey(section) ?? false ? _items[section] : new Dictionary<string, string>(); } }

        protected virtual void _ApplyChange() { }

        public TResult ReadValue<TResult>(string section, string key, string defaultValue = "")
        {
            TResult result = default(TResult);
            try
            {
                string value;
                if (_items.ContainsKey(section) && _items[section].ContainsKey(key))
                {
                    value = _items[section][key];
                }
                else
                {
                    if (!_items.ContainsKey(section)) _items[section] = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    _items[section][key] = defaultValue;
                    value = defaultValue;
                    _ApplyChange();
                }
                Type type = typeof(TResult);
                if (type.IsEnum)
                {
                    result = (TResult)Enum.Parse(type, value, true);
                }
                else
                {
                    result = (TResult)Convert.ChangeType(value, type);
                }

            }
            catch { }
            return result;
        }
        public object ReadValue(Type type, string section, string key, string defaultValue = "")
        {
            object result;
            try
            {
                string value;
                if (_items.ContainsKey(section) && _items[section].ContainsKey(key))
                {
                    value = _items[section][key];
                }
                else
                {
                    if (!_items.ContainsKey(section)) _items[section] = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    _items[section][key] = defaultValue;
                    value = defaultValue;
                    _ApplyChange();
                }

                if (type.IsEnum)
                {
                    result = Enum.Parse(type, value, true);
                }
                else
                {
                    result = Convert.ChangeType(value, type);
                }

            }
            catch
            {
                result = Activator.CreateInstance(type);
            }
            return result;
        }
        public TResult[] ReadValues<TResult>(string section, string pre_key = "")
        {
            Type type = typeof(TResult);
            TResult[] result = new TResult[0];
            try
            {
                if (_items.ContainsKey(section))
                {
                    string[] values = _items[section].Where(ite => string.IsNullOrEmpty(pre_key) || ite.Key.StartsWith(pre_key, StringComparison.InvariantCultureIgnoreCase)).Select(ite => ite.Value).ToArray();
                    result = values.Select(ite => type.IsEnum ? (TResult)Enum.Parse(type, ite, true) : (TResult)Convert.ChangeType(ite, type)).ToArray();
                }
            }
            catch { }
            return result;
        }
        public void RemoveSection(string section)
        {
            try
            {
                if (_items?.ContainsKey(section) ?? false)
                {
                    _items.Remove(section);
                    _ApplyChange();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void RemoveKey(string section, string key)
        {
            try
            {
                if ((_items?.ContainsKey(section) ?? false) && (_items[section]?.ContainsKey(key) ?? false))
                {
                    _items[section].Remove(key);
                    _ApplyChange();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ChangeValue(string section, string key, string value)
        {
            try
            {
                if ((_items?.ContainsKey(section) ?? false) && (_items[section]?.ContainsKey(key) ?? false))
                {
                    _items[section][key] = value;
                    _ApplyChange();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void AddValue(string section, string key, string value)
        {
            try
            {
                if (!_items.ContainsKey(section))
                {
                    _items[section] = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                }
                _items[section][key] = value;
                _ApplyChange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool ReadEnable(string section, bool enable = true)
        {
            int iEnable = ReadValue<int>(section, "enable", enable ? "1" : "0");
            return iEnable == 1;
        }
        public int ReadInterval(string section, int defaultTimeInterval = 60)
        {
            return ReadValue<int>(section, "interval", defaultTimeInterval.ToString());
        }
        public void ReadAllStaticConfigs<TStatisConfig>(params string[] sections)
        {

            try
            {
                if (sections?.Length > 0)
                {
                    Type scType = typeof(TStatisConfig);
                    TStatisConfig statisConfig = Activator.CreateInstance<TStatisConfig>();
                    var props = scType.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    foreach (string section in sections)
                    {
                        int sectionLen = section.Length + 1;
                        foreach (var prop in props)
                        {
                            if (prop.Name.StartsWith(section + "_", StringComparison.InvariantCultureIgnoreCase))
                            {
                                string key = prop.Name.Substring(sectionLen);
                                object value = null;

                                if (key.Equals("enable"))
                                {
                                    bool defaultValue = prop.GetValue(statisConfig).Equals(true);
                                    value = ReadEnable(section, defaultValue);
                                }
                                else if (key.Equals("interval"))
                                {
                                    int defaultValue = Convert.ToInt32(prop.GetValue(statisConfig) ?? 0);
                                    value = ReadInterval(section, defaultValue);
                                }
                                else if (prop.PropertyType == typeof(bool))
                                {
                                    object defaultValue = prop.GetValue(statisConfig).Equals(true) ? 1 : 0;
                                    value = ReadValue<int>(section, key, defaultValue.ToString()).Equals(1);
                                }
                                else if (prop.PropertyType.IsArray)
                                {
                                    Array defaultValue = (Array)prop.GetValue(statisConfig);
                                    List<object> lValues = new List<object>();
                                    for(int i = 0; i < defaultValue.Length; i++)
                                    {
                                        lValues.Add(defaultValue.GetValue(i));
                                    }
                                    string sDefaultValues = string.Join(",", lValues);

                                    var typeArg = prop.PropertyType.GetElementType();
                                    string sValue = ReadValue<string>(section, key, sDefaultValues);
                                    var tValues = sValue?.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (tValues?.Length > 0)
                                    {
                                        var tValueObjs = Array.CreateInstance(typeArg, tValues.Length);
                                        int index = 0;
                                        foreach (var tValue in tValues)
                                        {
                                            object tValueObj = null;
                                            try
                                            {
                                                if (typeArg.IsEnum)
                                                {
                                                    tValueObj = Enum.Parse(typeArg, tValue, true);
                                                }
                                                else
                                                {
                                                    tValueObj = Convert.ChangeType(tValue, typeArg);
                                                }
                                            }
                                            catch { }
                                            if (tValueObj != null)
                                            {
                                                tValueObjs.SetValue(tValueObj, index);
                                            }
                                            index++;
                                        }

                                        value = tValueObjs;
                                    }
                                }
                                else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                                {
                                    var typeArg = prop.PropertyType.GetGenericArguments()?.Single();
                                    IList defaultValue = (IList)prop.GetValue(statisConfig);
                                    List<object> lValues = new List<object>();
                                    for (int i = 0; i < defaultValue.Count; i++)
                                    {
                                        lValues.Add(defaultValue[i]);
                                    }
                                    string sDefaultValues = string.Join(",", lValues);
                                    string sValue = ReadValue<string>(section, key, sDefaultValues);
                                    var tValues = sValue?.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    if(tValues?.Length > 0)
                                    {
                                        value = Activator.CreateInstance(prop.PropertyType, null);
                                        var addMethod = prop.PropertyType.GetMethod("Add");

                                        List<object> tValueObjs = new List<object>();
                                        foreach(var tValue in tValues)
                                        {
                                            object tValueObj = null;
                                            try
                                            {
                                                if (typeArg.IsEnum)
                                                {
                                                    tValueObj = Enum.Parse(typeArg, tValue, true);
                                                }
                                                else
                                                {
                                                    tValueObj = Convert.ChangeType(tValue, typeArg);
                                                }
                                            }
                                            catch { }
                                            if(tValueObj != null)
                                            {
                                                if(addMethod != null)
                                                {
                                                    addMethod.Invoke(value, new object[] { tValueObj });
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    object defaultValue = prop.GetValue(statisConfig);
                                    value = ReadValue(prop.PropertyType, section, key, defaultValue?.ToString() ?? "");
                                }

                                try
                                {
                                    prop.SetValue(statisConfig, value, null);
                                }
                                catch //(Exception ex)
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ReadAllLocalConfigs<TConfig>(TConfig config, params string[] sections)
        {

            try
            {
                if (sections?.Length > 0)
                {
                    Type scType = typeof(TConfig);
                    var props = scType.GetProperties();
                    foreach (string section in sections)
                    {
                        int sectionLen = section.Length + 1;
                        foreach (var prop in props)
                        {
                            if (prop.Name.StartsWith(section + "_", StringComparison.InvariantCultureIgnoreCase))
                            {
                                string key = prop.Name.Substring(sectionLen);
                                object value;
                                if (key.Equals("enable"))
                                {
                                    bool defaultValue = prop.GetValue(config).Equals(true);
                                    value = ReadEnable(section, defaultValue);
                                }
                                else if (key.Equals("interval"))
                                {
                                    int defaultValue = Convert.ToInt32(prop.GetValue(config) ?? 0);
                                    value = ReadInterval(section, defaultValue);
                                }
                                else if (prop.PropertyType == typeof(bool))
                                {
                                    object defaultValue = prop.GetValue(config).Equals(true) ? 1 : 0;
                                    value = ReadValue<int>(section, key, defaultValue.ToString()).Equals(1);
                                }
                                else
                                {
                                    object defaultValue = prop.GetValue(config);
                                    value = ReadValue(prop.PropertyType, section, key, defaultValue?.ToString() ?? "");
                                }

                                try
                                {
                                    prop.SetValue(config, value, null);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void LoadFromString(string value, params string[] defaultSections)
        {
            _defaultSections = defaultSections;
            _items = new Dictionary<string, Dictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);
            _initializeValuesFromString(value);
        }
    }
}
