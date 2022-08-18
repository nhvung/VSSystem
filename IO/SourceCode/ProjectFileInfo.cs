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
        public bool Publish(string outputFolderPath, ETargetRuntime targetRuntime = ETargetRuntime.Portal, ETargetFramework targetFramework = ETargetFramework.DOTNET6, bool singleFile = true, bool selfContained = true, ExcludeCondition excludeCondition = default, CancellationToken cancellationToken = default)
        {
            return Publish(outputFolderPath, targetRuntime.ToString(), targetFramework.ToString(), singleFile, selfContained, excludeCondition, cancellationToken);
        }
        Process _process;
        public bool Publish(string outputFolderPath, string targetRuntime, string targetFramework, bool singleFile, bool selfContained, ExcludeCondition excludeCondition = default, CancellationToken cancellationToken = default)
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

                    string sTargetRuntime = string.Format("-r {0}", TargetRuntime.Windows_x64);
                    if (!string.IsNullOrWhiteSpace(targetRuntime))
                    {
                        sTargetRuntime = string.Format("-r {0}", targetRuntime);
                    }
                    if (!string.IsNullOrWhiteSpace(sTargetRuntime))
                    {
                        publishArgs.Add(sTargetRuntime);
                    }

                    publishArgs.Add(string.Format($"--nologo"));
                    if (singleFile)
                    {
                        publishArgs.Add(string.Format($"-p:PublishSingleFile=true"));
                        publishArgs.Add(string.Format($"-p:PublishTrimmed=true"));
                        publishArgs.Add(string.Format($"-p:SelfContained=true"));
                        publishArgs.Add(string.Format($"-p:SelfContained=true"));
                    }
                    else
                    {
                        publishArgs.Add(string.Format($"-p:SelfContained={(selfContained ? "true" : "false")}"));
                    }

                    publishArgs.Add(string.Format($"-p:ErrorOnDuplicatePublishOutputFiles=false"));
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
                            _DeleteFilesByName(outputFolder, excludeCondition.FileNames);
                        }
                        if (excludeCondition.FileExtensions?.Count > 0)
                        {
                            _DeleteFilesByExtension(outputFolder, excludeCondition.FileExtensions);
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

        public bool PublishToDLLs(string outputFolderPath, string targetRuntime, string targetFramework, bool selfContained = false, ExcludeCondition excludeCondition = default, CancellationToken cancellationToken = default)
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

                    string sTargetRuntime = string.Empty;
                    if (!string.IsNullOrWhiteSpace(targetRuntime))
                    {
                        sTargetRuntime = string.Format("-r {0}", targetRuntime);
                        publishArgs.Add(sTargetRuntime);
                    }

                    publishArgs.Add(string.Format($"--nologo --self-contained {(selfContained ? "true" : "false")}"));
                    publishArgs.Add(string.Format("-o \"{0}\"", outputFolderPath));

                    ProcessStartInfo psi = new ProcessStartInfo("dotnet", string.Join(" ", publishArgs));
                    psi.UseShellExecute = false;
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
                    if (excludeCondition == null)
                    {
                        excludeCondition = new ExcludeCondition();
                    }
                    if (excludeCondition != null)
                    {
                        excludeCondition.FileExtensions?.Add(".exe");
                        if (excludeCondition.FileNames?.Count > 0)
                        {
                            _DeleteFilesByName(outputFolder, excludeCondition.FileNames);
                        }
                        if (excludeCondition.FileExtensions?.Count > 0)
                        {
                            _DeleteFilesByExtension(outputFolder, excludeCondition.FileExtensions);
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

        public bool PublishToDocker(string outputFolderPath, string targetRuntime, string targetFramework, bool singleFile, string mappingPorts, ExcludeCondition excludeCondition = default, CancellationToken cancellationToken = default)
        {
            try
            {
                _process = null;
                if (!string.IsNullOrWhiteSpace(_ProjectFilePath) && File.Exists(_ProjectFilePath))
                {
                    List<string> publishArgs = new List<string>();
                    publishArgs.Add(string.Format("publish"));
                    publishArgs.Add(string.Format("\"{0}\"", _ProjectFilePath.Replace("\\", "/")));
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

                    string sTargetRuntime = string.Empty;
                    if (singleFile)
                    {
                        if (string.IsNullOrWhiteSpace(targetRuntime))
                        {
                            targetRuntime = TargetRuntime.Windows_x64;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(targetRuntime))
                    {
                        sTargetRuntime = string.Format("-r {0}", targetRuntime);
                        publishArgs.Add(sTargetRuntime);
                    }

                    if (singleFile)
                    {
                        publishArgs.Add(string.Format($"-p:PublishSingleFile=true"));
                        publishArgs.Add(string.Format($"-p:PublishTrimmed=true"));
                        publishArgs.Add(string.Format($"-p:SelfContained=true"));
                        if (string.IsNullOrWhiteSpace(targetRuntime))
                        {
                            targetRuntime = TargetRuntime.Windows_x64;
                        }
                    }
                    else
                    {
                        publishArgs.Add(string.Format("--nologo --self-contained false"));
                    }
                    publishArgs.Add(string.Format($"-p:ErrorOnDuplicatePublishOutputFiles=false"));



                    string newOutputFolderPath = outputFolderPath + "/" + _AssemblyName;
                    publishArgs.Add(string.Format("-o \"{0}\"", newOutputFolderPath.Replace("\\", "/")));

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

                    DirectoryInfo outputFolder = new DirectoryInfo(newOutputFolderPath);
                    if (excludeCondition == null)
                    {
                        excludeCondition = new ExcludeCondition();
                    }
                    if (excludeCondition != null)
                    {
                        if (!singleFile)
                        {
                            excludeCondition.FileExtensions?.Add(".exe");
                        }
                        excludeCondition.FileNames?.Add("web.config");
                        excludeCondition.FileNames?.Add($"{_AssemblyName}.deps.json");
                        if (excludeCondition.FileNames?.Count > 0)
                        {
                            _DeleteFilesByName(outputFolder, excludeCondition.FileNames);
                        }
                        if (excludeCondition.FileExtensions?.Count > 0)
                        {
                            _DeleteFilesByExtension(outputFolder, excludeCondition.FileExtensions);
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

                    outputFolder = new DirectoryInfo(outputFolderPath);
                    FileInfo dockerFile = new FileInfo(outputFolder.FullName + "/Dockerfile");

                    File.WriteAllText(dockerFile.FullName,
                        $"FROM mcr.microsoft.com/dotnet/aspnet:6.0" + Environment.NewLine
                        + $"WORKDIR /app/{_AssemblyName.ToLower()}" + Environment.NewLine
                        + $"COPY ./{_AssemblyName} ."
                    //+ Environment.NewLine
                    //+ Environment.NewLine
                    // + $"WORKDIR /app/{_AssemblyName.ToLower()}" + Environment.NewLine
                    // + $"ENTRYPOINT [\"./{_AssemblyName}\"]"
                    );

                    string packageName = $"{_AssemblyName.ToLower()}.{_FileVersion}";

                    File.WriteAllText(outputFolder.FullName + $"/docker.build.bat", $"docker image build -t \"{_AssemblyName.ToLower()}:{_FileVersion}\" .");
                    File.WriteAllText(outputFolder.FullName + $"/docker.build.sh", $"sudo docker image build -t \"{_AssemblyName.ToLower()}:{_FileVersion}\" .");

                    if (string.IsNullOrWhiteSpace(mappingPorts))
                    {
                        mappingPorts = "8080:80";
                    }
                    File.WriteAllText(outputFolder.FullName + $"/docker.run.bat", $"docker run -d -p {mappingPorts} --name {_AssemblyName.ToLower()} -w /app/{_AssemblyName.ToLower()} --entrypoint {_AssemblyName}.exe \"{_AssemblyName.ToLower()}:{_FileVersion}\"");
                    File.WriteAllText(outputFolder.FullName + $"/docker.run.sh", $"sudo docker run -d -p {mappingPorts} --name {_AssemblyName.ToLower()} -w /app/{_AssemblyName.ToLower()} --entrypoint ./{_AssemblyName} \"{_AssemblyName.ToLower()}:{_FileVersion}\"");

                    File.WriteAllText(outputFolder.FullName + $"/docker.stop.bat", $"docker stop {_AssemblyName.ToLower()}");
                    File.WriteAllText(outputFolder.FullName + $"/docker.stop.sh", $"sudo docker stop {_AssemblyName.ToLower()}");

                    File.WriteAllText(outputFolder.FullName + $"/docker.export.bat", $"docker build -o  \"type=tar,dest={_AssemblyName.ToLower()}.{_FileVersion}.tar\" -t \"{_AssemblyName.ToLower()}:{_FileVersion}\" .");
                    File.WriteAllText(outputFolder.FullName + $"/docker.export.sh", $"sudo docker build -o  \"type=tar,dest={_AssemblyName.ToLower()}.{_FileVersion}.tar\" -t \"{_AssemblyName.ToLower()}:{_FileVersion}\" .");

                    File.WriteAllText(outputFolder.FullName + $"/docker.import.bat", $"docker import \"{_AssemblyName.ToLower()}.{_FileVersion}.tar\" \"{_AssemblyName.ToLower()}:{_FileVersion}\"");
                    File.WriteAllText(outputFolder.FullName + $"/docker.import.sh", $"sudo docker import \"{_AssemblyName.ToLower()}.{_FileVersion}.tar\" \"{_AssemblyName.ToLower()}:{_FileVersion}\"");

                    File.WriteAllText(outputFolder.FullName + $"/docker.logs.bat", $"docker logs {_AssemblyName.ToLower()}");
                    File.WriteAllText(outputFolder.FullName + $"/docker.logs.sh", $"sudo docker logs {_AssemblyName.ToLower()}");

                    File.WriteAllText(outputFolder.FullName + $"/docker.remove.bat", $"docker stop {_AssemblyName.ToLower()}\ndocker rm {_AssemblyName.ToLower()}\ndocker rmi {_AssemblyName.ToLower()}:{_FileVersion}");
                    File.WriteAllText(outputFolder.FullName + $"/docker.remove.sh", $"sudo docker stop {_AssemblyName.ToLower()}&& \\\nsudo docker rm {_AssemblyName.ToLower()}&& \\\nsudo docker rmi {_AssemblyName.ToLower()}:{_FileVersion}");

                    return processFinish && !cancellationToken.IsCancellationRequested;
                }
            }
            catch { }
            return false;
        }

        void _DeleteFilesByName(DirectoryInfo folder, List<string> fileNames)
        {
            try
            {
                if (fileNames?.Count > 0)
                {
                    foreach (var fileName in fileNames)
                    {
                        FileInfo file = new FileInfo(folder.FullName + "/" + fileName);
                        if (file.Exists)
                        {
                            try
                            {
                                file.Delete();
                            }
                            catch { }
                        }
                    }

                    var subFolders = folder.GetDirectories();
                    if (subFolders?.Length > 0)
                    {
                        foreach (var subFolder in subFolders)
                        {
                            _DeleteFilesByName(subFolder, fileNames);
                        }
                    }
                }
            }
            catch { }
        }
        void _DeleteFilesByExtension(DirectoryInfo folder, List<string> fileExtensions)
        {
            try
            {
                if (fileExtensions?.Count > 0)
                {
                    foreach (var ext in fileExtensions)
                    {
                        var files = folder.GetFiles("*" + ext);
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

                    var subFolders = folder.GetDirectories();
                    if (subFolders?.Length > 0)
                    {
                        foreach (var subFolder in subFolders)
                        {
                            _DeleteFilesByExtension(subFolder, fileExtensions);
                        }
                    }
                }
            }
            catch { }
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
