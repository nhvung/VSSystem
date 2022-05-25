using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.IO
{
    public class ExcludeCondition
    {
        List<string> _fileNames, _folderNames, _fileExtensions, _filePrefixNames, _folderPrefixNames;
        public List<string> FileNames { get { return _fileNames ?? new List<string>(); } set { _fileNames = value; } }
        public List<string> FolderNames { get { return _folderNames ?? new List<string>(); } set { _folderNames = value; } }
        public List<string> FileExtensions { get { return _fileExtensions ?? new List<string>(); } set { _fileExtensions = value; } }
        public List<string> FilePrefixNames { get { return _filePrefixNames ?? new List<string>(); } set { _filePrefixNames = value; } }
        public List<string> FolderPrefixNames { get { return _folderPrefixNames ?? new List<string>(); } set { _folderPrefixNames = value; } }
        public ExcludeCondition()
        {
            _fileExtensions = new List<string>();
            _fileNames = new List<string>();
            _folderNames = new List<string>();
            _filePrefixNames = new List<string>();
            _folderPrefixNames = new List<string>();
        }

        public static ExcludeCondition SourceCodeExcludeCondition()
        {
            ExcludeCondition result = new ExcludeCondition()
            {
                _fileExtensions = new List<string>() { ".vspscc", ".bak", ".sln", ".scc", ".pdb" },
                _fileNames = new List<string>() { },
                _folderNames = new List<string>() { "@BK", "BK", "log", "logfile", "logfiles", "log_bk", "log_old", ".vs", "TempUnZipInfo", "packages", "Temp" },
                _filePrefixNames = new List<string>() { },
                _folderPrefixNames = new List<string>() { }
            };
            return result;
        }

        public static ExcludeCondition WebExcludeCondition()
        {
            ExcludeCondition result = new ExcludeCondition()
            {
                _fileExtensions = new List<string>() { ".vspscc", ".bak", ".sln", ".scc", ".pdb", ".bak", ".tmp", ".installlog" },
                _fileNames = new List<string>() { },
                _folderNames = new List<string>() { "@BK", "BK", "log", "logfile", "logfiles", "log_bk", "log_old", ".vs", "TempUnZipInfo", "packages", "Temp", "Download", "Session" },
                _filePrefixNames = new List<string>() { },
                _folderPrefixNames = new List<string>() { "logfiles" }
            };
            return result;
        }
        public static ExcludeCondition ServiceExcludeCondition()
        {
            ExcludeCondition result = new ExcludeCondition()
            {
                _fileExtensions = new List<string>() { ".vspscc", ".bak", ".sln", ".scc", ".pdb", ".bak", ".tmp" },
                _fileNames = new List<string>() { },
                _folderNames = new List<string>() { "@BK", "BK", "log", "logfile", "logfiles", "log_bk", "log_old", ".vs", "TempUnZipInfo", "packages", "Temp", "Download" },
                _filePrefixNames = new List<string>() { },
                 _folderPrefixNames = new List<string>() { "logfiles" }
            };
            return result;
        }

        public static ExcludeCondition PublishSourceExcludeCondition()
        {
            ExcludeCondition result = new ExcludeCondition()
            {
                _fileExtensions = new List<string>() { ".vspscc", ".bak", ".sln", ".scc", ".pdb", ".bak", ".tmp" },
                _fileNames = new List<string>() { "aspnetcorev2_inprocess.dll", "e_sqlite3.dll", "sni.dll" },
                _folderNames = new List<string>() { },
                _filePrefixNames = new List<string>() { },
                _folderPrefixNames = new List<string>() { }
            };
            return result;
        }
    }
}
