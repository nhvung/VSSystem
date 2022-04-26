using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VSSystem.Models
{
    public partial class VersionInfo
    {
        string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        string _Version;
        public string Version { get { return _Version; } set { _Version = value; } }

        string _Description;
        public string Description { get { return _Description; } set { _Description = value; } }
        public VersionInfo()
        {
            _Name = string.Empty;
            _Version = string.Empty;
            _Description = string.Empty;
        }
    }
}
