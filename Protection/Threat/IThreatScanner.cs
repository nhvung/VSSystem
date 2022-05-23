using System;

namespace VSSystem.Protection.Threat
{
    public interface IThreatScanner
    {
        ScanResult ScanFile(string filePath, int timeout = 30000);
        //ScanResult ScanFolder(string folderPath);
    }
}
