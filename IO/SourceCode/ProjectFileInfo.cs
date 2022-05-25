using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using VSSystem.IO.Extensions;

namespace VSSystem.IO.SourceCode
{
    public class ProjectFileInfo
    {

        string _Framework;
        public string Framework { get { return _Framework; } set { _Framework = value; } }

        string _Version;
        public string Version { get { return _Version; } set { _Version = value; } }

        string _FileVersion;
        public string FileVersion { get { return _FileVersion; } set { _FileVersion = value; } }

        string _OutputType;
        public string OutputType { get { return _OutputType; } set { _OutputType = value; } }

        string _Description;
        public string Description { get { return _Description; } set { _Description = value; } }

        string _AssemblyName;
        public string AssemblyName { get { return _AssemblyName; } set { _AssemblyName = value; } }

        string _Company;
        public string Company { get { return _Company; } set { _Company = value; } }

        string _Authors;
        public string Authors { get { return _Authors; } set { _Authors = value; } }

        string _Copyright;
        public string Copyright { get { return _Copyright; } set { _Copyright = value; } }


        string _ProjectFilePath;
        public string ProjectFilePath { get { return _ProjectFilePath; } set { _ProjectFilePath = value; } }

        public ProjectFileInfo(string projectFilePath)
        {
            _ProjectFilePath = projectFilePath;
            _LoadFile();
        }

        void _LoadFile()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_ProjectFilePath) && File.Exists(_ProjectFilePath))
                {
                    bool hasChanged = false;
                    var xmlDoc = new XmlDocument();
                    XmlElement projectElement = (XmlElement)xmlDoc.SelectSingleNode("Project");
                    try
                    {
                        xmlDoc.Load(_ProjectFilePath);
                        projectElement = (XmlElement)xmlDoc.SelectSingleNode("Project");
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.IndexOf("Root element is missing.", StringComparison.InvariantCultureIgnoreCase) >= 0)
                        {
                            if (projectElement == null)
                            {
                                projectElement = xmlDoc.CreateElement("Project");
                                projectElement.SetAttribute("Sdk", "Microsoft.NET.Sdk.Web");
                                xmlDoc.AppendChild(projectElement);
                                hasChanged = true;
                            }
                        }
                    }

                    _Framework = _GetValue(xmlDoc, "TargetFramework", TargetFramework.NetCore31, out hasChanged);
                    if (hasChanged)
                    {
                        xmlDoc.Save(_ProjectFilePath);
                    }

                    _Version = _GetValue(xmlDoc, "Version", "1.0.1", out hasChanged);
                    if (hasChanged)
                    {
                        xmlDoc.Save(_ProjectFilePath);
                    }

                    _FileVersion = _GetValue(xmlDoc, "FileVersion", "1.0.1", out hasChanged);
                    if (hasChanged)
                    {
                        xmlDoc.Save(_ProjectFilePath);
                    }

                    _OutputType = _GetValue(xmlDoc, "OutputType", "Exe", out hasChanged);
                    if (hasChanged)
                    {
                        xmlDoc.Save(_ProjectFilePath);
                    }

                    _Description = _GetValue(xmlDoc, "Description", "", out hasChanged);
                    if (hasChanged)
                    {
                        xmlDoc.Save(_ProjectFilePath);
                    }

                    _AssemblyName = _GetValue(xmlDoc, "AssemblyName", "", out hasChanged);
                    if (hasChanged)
                    {
                        xmlDoc.Save(_ProjectFilePath);
                    }

                    _Company = _GetValue(xmlDoc, "Company", "Evidence IQ", out hasChanged);
                    if (hasChanged)
                    {
                        xmlDoc.Save(_ProjectFilePath);
                    }
                    _Authors = _GetValue(xmlDoc, "Authors", "Evidence IQ", out hasChanged);
                    if (hasChanged)
                    {
                        xmlDoc.Save(_ProjectFilePath);
                    }
                    _Copyright = _GetValue(xmlDoc, "Copyright", "Evidence IQ", out hasChanged);
                    if (hasChanged)
                    {
                        xmlDoc.Save(_ProjectFilePath);
                    }
                }
            }
            catch { }
        }
        public bool Publish(string outputFolderPath, ETargetRuntime targetRuntime = ETargetRuntime.Windows_x64, ETargetFramework targetFramework = ETargetFramework.DOTNET6, ExcludeCondition excludeCondition = default, CancellationToken cancellationToken = default)
        {
            return Publish(outputFolderPath, targetRuntime.ToString(), targetFramework.ToString(), excludeCondition, cancellationToken);
        }
        Process _process;
        public bool Publish(string outputFolderPath, string targetRuntime, string targetFramework, ExcludeCondition excludeCondition = default, CancellationToken cancellationToken = default)
        {
            try
            {
                _process = null;
                if (!string.IsNullOrWhiteSpace(_ProjectFilePath) && File.Exists(_ProjectFilePath))
                {
                    List<string> publishArgs = new List<string>();
                    publishArgs.Add(string.Format("publish"));
                    publishArgs.Add(string.Format("\"{0}\"", _ProjectFilePath));
                    publishArgs.Add(string.Format("-c Release"));

                    string sTargetFramework = string.Format("-f {0}", targetFramework);
                    if (string.IsNullOrWhiteSpace(sTargetFramework))
                    {
                        sTargetFramework = _Framework;
                    }
                    if (!string.IsNullOrWhiteSpace(sTargetFramework))
                    {
                        publishArgs.Add(sTargetFramework);
                    }

                    string sTargetRuntime = sTargetRuntime = string.Format("-r {0}", targetRuntime);
                    if (!string.IsNullOrWhiteSpace(sTargetRuntime))
                    {
                        publishArgs.Add(sTargetRuntime);
                    }

                    publishArgs.Add(string.Format("--nologo -p:PublishSingleFile=true -p:SelfContained=true -p:PublishTrimmed=true"));
                    publishArgs.Add(string.Format("-o \"{0}\"", outputFolderPath));

                    ProcessStartInfo psi = new ProcessStartInfo("dotnet", string.Join(" ", publishArgs));
                    psi.UseShellExecute = false;
                    //psi.RedirectStandardInput = true;
                    psi.CreateNoWindow = true;
                    psi.RedirectStandardOutput = false;


                    _process = Process.Start(psi);

                    bool processFinish = false;
                    if (cancellationToken != null)
                    {
                        Task.Run(() =>
                        {

                            try
                            {
                                do
                                {
                                    if (processFinish)
                                    {
                                        break;
                                    }
                                    Thread.Sleep(500);

                                } while (!cancellationToken.IsCancellationRequested);
                                if (!processFinish)
                                {
                                    StopProcess();
                                }

                            }
                            catch { }
                        });
                    }

                    _process?.WaitForExit();
                    _process?.Close();

                    processFinish = true;

                    DirectoryInfo outputFolder = new DirectoryInfo(outputFolderPath);
                    if (excludeCondition != null)
                    {
                        if (excludeCondition.FileNames?.Count > 0)
                        {
                            foreach (var fileName in excludeCondition.FileNames)
                            {
                                FileInfo file = new FileInfo(outputFolder.FullName + "/" + fileName);
                                if (file.Exists)
                                {

                                    try
                                    {
                                        file.Delete();
                                    }
                                    catch { }
                                }
                            }
                        }
                        if (excludeCondition.FileExtensions?.Count > 0)
                        {
                            foreach (var ext in excludeCondition.FileExtensions)
                            {
                                var files = outputFolder.GetFiles("*" + ext);
                                if (files.Length > 0)
                                {
                                    foreach (var file in files)
                                    {
                                        try
                                        {
                                            file.Attributes = FileAttributes.Archive;
                                            file.Delete();
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }

                        if (excludeCondition.FolderNames?.Count > 0)
                        {
                            foreach (var name in excludeCondition.FolderNames)
                            {
                                var eFolder = new DirectoryInfo(outputFolder.FullName + "/" + name);

                                try
                                {
                                    eFolder.Empty(true);
                                }
                                catch { }
                            }
                        }
                    }

                    return processFinish && !cancellationToken.IsCancellationRequested;
                }
            }
            catch { }
            return false;
        }



        string _GetValue(XmlDocument xmlDoc, string propertyName, string defaultValue, out bool hasChanged)
        {
            hasChanged = false;
            string result = defaultValue;
            try
            {
                XmlElement entryNode = (XmlElement)xmlDoc.SelectSingleNode("Project/PropertyGroup/" + propertyName);
                if (entryNode == null)
                {
                    XmlElement projectElement = (XmlElement)xmlDoc.SelectSingleNode("Project");
                    if (projectElement == null)
                    {
                        projectElement = xmlDoc.CreateElement("Project");
                    }
                    XmlElement projectGroupElement = (XmlElement)xmlDoc.SelectSingleNode("Project/PropertyGroup");
                    if (projectGroupElement == null)
                    {
                        projectGroupElement = xmlDoc.CreateElement("PropertyGroup");
                    }
                    entryNode = xmlDoc.CreateElement(propertyName);
                    entryNode.InnerXml = defaultValue;
                    projectGroupElement.AppendChild(entryNode);
                    projectElement.AppendChild(projectGroupElement);
                    hasChanged = true;
                }
                else
                {
                    result = entryNode.InnerXml;
                }
            }
            catch { }
            return result;
        }

        void _SetValue(XmlDocument xmlDoc, string propertyName, string value)
        {
            try
            {
                XmlElement entryNode = (XmlElement)xmlDoc.SelectSingleNode("Project/PropertyGroup/" + propertyName);
                if (entryNode == null)
                {
                    XmlElement projectElement = (XmlElement)xmlDoc.SelectSingleNode("Project");
                    if (projectElement == null)
                    {
                        projectElement = xmlDoc.CreateElement("Project");
                    }
                    XmlElement projectGroupElement = (XmlElement)xmlDoc.SelectSingleNode("Project/PropertyGroup");
                    if (projectGroupElement == null)
                    {
                        projectGroupElement = xmlDoc.CreateElement("PropertyGroup");
                    }
                    entryNode = xmlDoc.CreateElement(propertyName);
                    projectGroupElement.AppendChild(entryNode);
                    projectElement.AppendChild(projectGroupElement);
                }
                entryNode.InnerXml = value;
            }
            catch { }
        }

        public void Save()
        {

            try
            {
                var xmlDoc = new XmlDocument();
                XmlElement projectElement = (XmlElement)xmlDoc.SelectSingleNode("Project");
                try
                {
                    xmlDoc.Load(_ProjectFilePath);
                    projectElement = (XmlElement)xmlDoc.SelectSingleNode("Project");
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("Root element is missing.", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        if (projectElement == null)
                        {
                            projectElement = xmlDoc.CreateElement("Project");
                            projectElement.SetAttribute("Sdk", "Microsoft.NET.Sdk.Web");
                            xmlDoc.AppendChild(projectElement);
                        }
                    }
                }

                _SetValue(xmlDoc, "TargetFramework", _Framework);
                _SetValue(xmlDoc, "Version", _Version);
                _SetValue(xmlDoc, "FileVersion", _FileVersion);
                _SetValue(xmlDoc, "OutputType", _OutputType);
                _SetValue(xmlDoc, "Description", _Description);
                _SetValue(xmlDoc, "AssemblyName", _AssemblyName);
                _SetValue(xmlDoc, "Company", _Company);
                _SetValue(xmlDoc, "Copyright", _Copyright);
                _SetValue(xmlDoc, "Authors", _Authors);

                xmlDoc.Save(_ProjectFilePath);
            }
            catch { }
        }

        public void StopProcess()
        {
            if (_process != null)
            {
                _process.Kill();
            }
        }
    }
}
