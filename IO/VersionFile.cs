using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using VSSystem.Models;

namespace VSSystem.IO
{
    public class VersionFile
    {

        public static void SaveToFile(string fileName, VersionInfo versionInfo)
        {
            try
            {
                var mappingCharacters = new KeyValuePair<string, string>[] 
                {
                    new KeyValuePair<string, string>("&", "&amp;"),
                    new KeyValuePair<string, string>("<", "&lt;"),
                    new KeyValuePair<string, string>(">", "&gt;"),
                    new KeyValuePair<string, string>("'", "&apos;"),
                    new KeyValuePair<string, string>("\"", "&quot;"),
                };

                FileInfo versionFile = new FileInfo(fileName);
                if(!versionFile.Directory.Exists)
                {
                    versionFile.Directory.Create();
                }

                foreach(var m in mappingCharacters)
                {
                    versionInfo.Name = versionInfo.Name.Replace(m.Key, m.Value);
                    versionInfo.Version = versionInfo.Version.Replace(m.Key, m.Value);
                    versionInfo.Description = versionInfo.Description.Replace(m.Key, m.Value);
                }

                File.WriteAllLines(versionFile.FullName, new string[] {
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                    "<package>",
                    $"<name>{versionInfo.Name}</name>",
                    $"<version>{versionInfo.Version}</version>",
                    $"<description>{versionInfo.Description}</description>",
                    "</package>",
                });
            }
            catch { }
        }

        public static VersionInfo GetVersion(string fileName)
        {
            VersionInfo result = new VersionInfo();

            try
            {
                FileInfo versionFile = new FileInfo(fileName);
                if (versionFile?.Exists ?? false)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(versionFile.FullName);
                    var packageXml = xml.SelectSingleNode("package");
                    if(packageXml?.ChildNodes?.Count > 0)
                    {
                        foreach(XmlNode xmlNode in packageXml)
                        {
                            if(xmlNode.Name.Equals("name", StringComparison.InvariantCultureIgnoreCase))
                            {
                                result.Name = xmlNode.InnerText;
                            }
                            else if (xmlNode.Name.Equals("version", StringComparison.InvariantCultureIgnoreCase))
                            {
                                result.Version = xmlNode.InnerText;
                            }
                            else if (xmlNode.Name.Equals("description", StringComparison.InvariantCultureIgnoreCase))
                            {
                                result.Description = xmlNode.InnerText;
                            }
                        }
                    }
                }
            }
            catch { }
            return result;
        }
    }
}
