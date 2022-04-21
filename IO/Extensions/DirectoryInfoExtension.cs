using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VSSystem.Threading.Tasks.Extensions;

namespace VSSystem.IO.Extensions
{
    public static class DirectoryInfoExtension
    {


        #region Private
        static void _DeleteSpecialFiles(DirectoryInfo folder, List<string> fileNames, List<string> prefixFileNames, List<string> fileExtensions)
        {
            try
            {
                FileInfo[] files = folder.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (fileNames?.Contains(file.Name, StringComparer.InvariantCultureIgnoreCase) ?? false)
                    {
                        file.Delete();
                    }
                }
                if (prefixFileNames?.Count > 0)
                {
                    foreach (string prefixFileName in prefixFileNames)
                    {
                        if (string.IsNullOrEmpty(prefixFileName)) continue;
                        files = folder.GetFiles(prefixFileName + "*.*");
                        foreach (FileInfo file in files)
                        {
                            file.Delete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static void _CopyToParallel(DirectoryInfo sourceFolder, DirectoryInfo destinationFolder, bool overwrite, int numberOfTasks, ExcludeCondition excludeCondition, Action<string> executeFileLog)
        {
            try
            {
                if (!sourceFolder.Exists) throw new Exception("_copyFolder Exception: Source directory not existed.");
                List<string> excFileNames = new List<string>(), excFileExtensions = new List<string>(), excFolderNames = new List<string>(), excFilePreNames = new List<string>();
                if (excludeCondition != null)
                {
                    excFileExtensions = excludeCondition.FileExtensions;
                    excFileNames = excludeCondition.FileNames;
                    excFolderNames = excludeCondition.FolderNames;
                    excFilePreNames = excludeCondition.FilePrefixNames;
                }
                _CopyToParallel(sourceFolder, destinationFolder, excFileNames, excFileExtensions, excFolderNames, excFilePreNames, overwrite, numberOfTasks, executeFileLog);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        static void _CopyToParallel(DirectoryInfo sourceFolder, DirectoryInfo destinationFolder, List<string> excludeFileNames, List<string> excludeExtensions, List<string> excludeFolderNames, List<string> excFilePreNames, bool overwrite, int numberOfTasks, Action<string> executeFileLog)
        {
            try
            {
                if (!destinationFolder.Exists) destinationFolder.Create();
                FileInfo[] files = sourceFolder.EnumerateFiles().ToArray();

                if (files.Length > 0)
                {
                    files.ConsecutiveRun(file =>
                    {
                        try
                        {
                            if (excludeFileNames.Contains(file.Name, StringComparer.InvariantCultureIgnoreCase) || excludeExtensions.Contains(file.Extension, StringComparer.InvariantCultureIgnoreCase))
                            {
                                return;
                            }
                            if (excFilePreNames.Any(preName => file.Name.StartsWith(preName, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                return;
                            }
                            file.Attributes = FileAttributes.Archive;
                            executeFileLog?.Invoke(string.Format("Copy .../{0}/{2} to .../{1}/{2}", sourceFolder.Name, destinationFolder.Name, file.Name));
                            file.CopyTo(destinationFolder.FullName + "/" + file.Name, overwrite);
                        }
                        catch
                        {
                            return;
                        }

                    }, numberOfTasks);
                }

                DirectoryInfo[] subDirectories = sourceFolder.GetDirectories();
                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    if (excludeFolderNames.Contains(subDirectory.Name, StringComparer.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    _CopyToParallel(subDirectory, new DirectoryInfo(destinationFolder.FullName + "/" + subDirectory.Name), excludeFileNames, excludeExtensions, excludeFolderNames, excFilePreNames, overwrite, numberOfTasks, executeFileLog);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static void _CopyTo(DirectoryInfo sourceFolder, DirectoryInfo destinationFolder, bool overwrite, ExcludeCondition excludeCondition, Action<string> executeFileLog)
        {
            try
            {
                if (!sourceFolder.Exists)
                {
                    throw new Exception("_copyFolder Exception: Source directory not existed.");
                }
                List<string> excFileNames = new List<string>()
                , excFileExtensions = new List<string>()
                , excFolderNames = new List<string>()
                , excFilePreNames = new List<string>()
                , excFolderPreNames = new List<string>();

                if (excludeCondition != null)
                {
                    excFileExtensions = excludeCondition.FileExtensions ?? new List<string>();
                    excFileNames = excludeCondition.FileNames ?? new List<string>();
                    excFilePreNames = excludeCondition.FilePrefixNames ?? new List<string>();
                    excFolderNames = excludeCondition.FolderNames ?? new List<string>();
                    excFolderPreNames = excludeCondition.FolderPrefixNames ?? new List<string>();
                }
                _CopyTo(sourceFolder, destinationFolder, excFileNames, excFilePreNames, excFileExtensions, excFolderNames, excFolderPreNames, overwrite, executeFileLog);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static void _CopyTo(DirectoryInfo sourceFolder, DirectoryInfo destinationFolder
        , List<string> excludeFileNames, List<string> excludeFilePreNames
        , List<string> excludeExtensions
        , List<string> excludeFolderNames, List<string> excludeFolderPreNames
        , bool overwrite, Action<string> executeFileLog)
        {
            try
            {
                if (!destinationFolder.Exists) destinationFolder.Create();
                FileInfo[] files = sourceFolder.GetFiles();
                if (files?.Length > 0)
                {
                    if (excludeFilePreNames?.Count > 0)
                    {
                        foreach (var preName in excludeFilePreNames)
                        {
                            files = files.Where(ite => ite.Name.IndexOf(preName, StringComparison.InvariantCultureIgnoreCase) < 0).ToArray();
                        }
                    }
                    if (files?.Length > 0)
                    {
                        foreach (FileInfo file in files)
                        {
                            if (excludeFileNames.Contains(file.Name, StringComparer.InvariantCultureIgnoreCase))
                            {
                                continue;
                            }
                            if (excludeExtensions.Contains(file.Extension, StringComparer.InvariantCultureIgnoreCase))
                            {
                                continue;
                            }

                            file.Attributes = FileAttributes.Archive;

                            executeFileLog?.Invoke(string.Format("Copy .../{0}/{2} to .../{1}/{2}", sourceFolder.Name, destinationFolder.Name, file.Name));

                            string destinationFileName = destinationFolder.FullName + "/" + file.Name;

                            file.CopyTo(destinationFileName, overwrite);

                            //File.SetCreationTime(destinationFileName, file.CreationTime);
                            File.SetCreationTimeUtc(destinationFileName, file.CreationTimeUtc);

                            //File.SetLastAccessTime(destinationFileName, file.LastAccessTime);
                            File.SetLastAccessTimeUtc(destinationFileName, file.LastAccessTimeUtc);

                            //File.SetLastWriteTime(destinationFileName, file.LastWriteTime);
                            File.SetLastWriteTimeUtc(destinationFileName, file.LastWriteTimeUtc);
                        }
                    }

                }

                DirectoryInfo[] subDirectories = sourceFolder.GetDirectories();
                if (subDirectories?.Length > 0)
                {
                    if (excludeFolderPreNames?.Count > 0)
                    {
                        foreach (var preName in excludeFolderPreNames)
                        {
                            subDirectories = subDirectories.Where(ite => ite.Name.IndexOf(preName, StringComparison.InvariantCultureIgnoreCase) < 0).ToArray();
                        }
                    }
                    foreach (DirectoryInfo subDirectory in subDirectories)
                    {
                        if (excludeFolderNames.Contains(subDirectory.Name, StringComparer.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        _CopyTo(subDirectory, new DirectoryInfo(destinationFolder.FullName + "/" + subDirectory.Name), excludeFileNames, excludeFilePreNames, excludeExtensions, excludeFolderNames, excludeFolderPreNames, overwrite, executeFileLog);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static List<KeyValuePair<string, string>> _GetDiffFiles(DirectoryInfo srcFolder, DirectoryInfo desFolder, bool checkMd5 = false)
        {

            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            FileInfo[] srcFiles = srcFolder.GetFiles();
            foreach (var srcFile in srcFiles)
            {
                FileInfo desFile = new FileInfo(desFolder.FullName + "/" + srcFile.Name);
                if (!srcFile.IsSame(desFile, checkMd5))
                {
                    result.Add(new KeyValuePair<string, string>(srcFile.FullName, desFile.FullName));
                }
            }

            DirectoryInfo[] srcSubFolders = srcFolder.GetDirectories();
            foreach (var srcSubFolder in srcSubFolders)
            {
                var desSubFolder = new DirectoryInfo(desFolder.FullName + "/" + srcSubFolder.Name);
                var diffFiles = _GetDiffFiles(srcSubFolder, desSubFolder, checkMd5);
                if (diffFiles?.Count > 0)
                {
                    result.AddRange(diffFiles);
                }
            }

            return result;
        }

        static int _SynchronizeFolder(this DirectoryInfo srcFolder, DirectoryInfo destFolder, bool checkMd5
        , List<string> excludeFileNames, List<string> excludeFilePreNames
        , List<string> excludeExtensions
        , List<string> excludeFolderNames, List<string> excludeFolderPreNames)
        {
            if (!srcFolder.Exists) return 0;
            int result = 0;
            try
            {
                var files = srcFolder.GetFiles();
                if (files?.Length > 0)
                {
                    if (excludeFilePreNames?.Count > 0)
                    {
                        foreach (var name in excludeFilePreNames)
                        {
                            files = files.Where(ite => ite.Name.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) < 0).ToArray();
                        }
                    }
                    if (excludeFileNames?.Count > 0)
                    {
                        files = files.Where(ite => !excludeFileNames.Contains(ite.Name, StringComparer.InvariantCultureIgnoreCase)).ToArray();
                    }
                    if (excludeExtensions?.Count > 0)
                    {
                        files = files.Where(ite => !excludeExtensions.Contains(ite.Extension, StringComparer.InvariantCultureIgnoreCase)).ToArray();
                    }
                    if (!destFolder.Exists)
                    {
                        destFolder.Create();
                    }

                    int nThread = 2;
                    if (files?.Length > 0)
                    {
                        var execResult = files.ConsecutiveRun(file =>
                        {
                            FileInfo destFile = new FileInfo(destFolder.FullName + "/" + file.Name);
                            if (!file.IsSame(destFile, checkMd5))
                            {
                                file.CopyToV2(destFile.FullName, true);
                                return 1;
                            }
                            return 0;
                        }, nThread);
                        result += execResult.Sum();
                    }
                }


                DirectoryInfo[] subDirectories = srcFolder.GetDirectories();
                if (subDirectories?.Length > 0)
                {
                    if (!destFolder.Exists)
                    {
                        destFolder.Create();
                    }
                    if (excludeFolderPreNames?.Count > 0)
                    {
                        foreach (var preName in excludeFolderPreNames)
                        {
                            subDirectories = subDirectories.Where(ite => ite.Name.IndexOf(preName, StringComparison.InvariantCultureIgnoreCase) < 0).ToArray();
                        }
                    }
                    foreach (DirectoryInfo subDirectory in subDirectories)
                    {
                        if (excludeFolderNames.Contains(subDirectory.Name, StringComparer.InvariantCultureIgnoreCase))
                        {
                            continue;
                        }

                        DirectoryInfo destSubFolder = new DirectoryInfo(destFolder.FullName + "/" + subDirectory.Name);
                        result += _SynchronizeFolder(subDirectory, destSubFolder, checkMd5, excludeFileNames, excludeFilePreNames, excludeExtensions, excludeFolderNames, excludeFolderPreNames);
                    }


                }

            }
            catch// (Exception ex)
            {
            }
            return result;
        }

        #endregion

        #region Public
        public static int CleanExpired(this DirectoryInfo folder, DateTime checkDate, TimeSpan keepingTime, int keepingFolderLevel, int currentFolderLevel
            , Action<Exception> errorAction = null)
        {
            int totalRemainItems = 0;
            try
            {
                DateTime dtExpired = checkDate.Date.Add(-keepingTime);
                var allFiles = folder.GetFiles();
                var deletedFiles = allFiles.Where(ite => ite.CreationTimeUtc.Date < dtExpired).ToArray();

                if (deletedFiles.Length > 0)
                {
                    int deletedFileCount = 0;
                    foreach (var file in deletedFiles)
                    {

                        try
                        {
                            file.Attributes = FileAttributes.Archive;
                            file.Delete();
                            deletedFileCount++;
                        }
                        catch (Exception ex)
                        {
                            errorAction?.Invoke(ex);
                        }
                    }
                    totalRemainItems = allFiles.Length - deletedFileCount;
                }

                DirectoryInfo[] subFolders = folder.GetDirectories();
                foreach (var subFolder in subFolders)
                {
                    int totalSubRemainItems = subFolder.CleanExpired(checkDate, keepingTime, keepingFolderLevel, currentFolderLevel + 1, errorAction);
                    if (totalSubRemainItems == 0)
                    {
                        if (subFolder.CreationTimeUtc.Date < dtExpired)
                        {
                            totalRemainItems++;
                        }
                    }
                    else
                    {
                        totalRemainItems += totalSubRemainItems;
                    }
                }
                if (totalRemainItems == 0 && keepingFolderLevel > -1 && currentFolderLevel > keepingFolderLevel)
                {
                    try
                    {
                        folder.Delete();
                    }
                    catch (Exception ex)
                    {
                        errorAction?.Invoke(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex);
            }
            return totalRemainItems;
        }
        public static void Empty(this DirectoryInfo folder, int keepingFolderLevel, int currentFolderLevel
            , Action<Exception> errorAction = null)
        {
            try
            {
                bool includeCurrent = currentFolderLevel > keepingFolderLevel;
                if (includeCurrent)
                {
                    var files = folder.GetFiles();
                    if (files?.Length > 0)
                    {
                        int deletedFileCount = 0;
                        foreach (var file in files)
                        {

                            try
                            {
                                file.Attributes = FileAttributes.Archive;
                                file.Delete();
                                deletedFileCount++;
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
                        Empty(subFolder, keepingFolderLevel, currentFolderLevel + 1, errorAction);
                    }
                }

                if (includeCurrent)
                {
                    try
                    {
                        folder.Delete();
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex);
            }
        }
        public static void Empty(this DirectoryInfo folder, bool deleteCurrentFolder = false
             , Action<Exception> errorAction = null)
        {
            try
            {
                if (!folder.Exists) return;
                if (deleteCurrentFolder)
                {
                    Empty(folder, -1, 0, errorAction);
                }
                else
                {
                    Empty(folder, 0, 0, errorAction);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static int Empty(this DirectoryInfo folder, DateTime checkDate, TimeSpan keepTime)
        {
            try
            {
                FileInfo[] files = folder.GetFiles();

                FileInfo[] deleteFiles = files.Where(ite => ((checkDate - ite.CreationTimeUtc.Date).TotalDays > keepTime.TotalDays)).ToArray();

                if (deleteFiles.Length > 0)
                {
                    foreach (var file in deleteFiles)
                    {
                        file.Attributes = FileAttributes.Archive;
                        file.Delete();
                    }
                }

                int totalExistsItems = files.Length - deleteFiles.Length;

                DirectoryInfo[] subFolders = folder.GetDirectories();
                if (subFolders.Length > 0)
                {
                    foreach (var subFolder in subFolders)
                    {
                        int remainFileItemCount = Empty(subFolder, checkDate, keepTime);

                        if (remainFileItemCount == 0)
                        {
                            if ((checkDate - subFolder.CreationTimeUtc.Date).TotalDays <= keepTime.TotalDays)
                            {
                                totalExistsItems++;
                            }

                        }
                        else
                        {
                            totalExistsItems += remainFileItemCount;
                        }
                    }
                }
                if (totalExistsItems == 0)
                {
                    if ((checkDate - folder.CreationTimeUtc.Date).TotalDays > keepTime.TotalDays)
                    {
                        folder.Delete();
                    }
                }

                return totalExistsItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CopyTo(this DirectoryInfo sourceFolder, DirectoryInfo destinationFolder, ExcludeCondition excludeCondition = null, Action<string> executeFileLog = null)
        {
            _CopyTo(sourceFolder, destinationFolder, true, excludeCondition, executeFileLog);
        }
        public static void CopyTo(this DirectoryInfo sourceFolder, DirectoryInfo destinationFolder, bool overwrite, ExcludeCondition excludeCondition = null, Action<string> executeFileLog = null)
        {
            _CopyTo(sourceFolder, destinationFolder, overwrite, excludeCondition, executeFileLog);
        }
        public static void CopyToParallel(this DirectoryInfo sourceFolder, DirectoryInfo destinationFolder, int numberOfTasks, ExcludeCondition excludeCondition = null, Action<string> executeFileLog = null)
        {
            _CopyToParallel(sourceFolder, destinationFolder, true, numberOfTasks, excludeCondition, executeFileLog);
        }
        public static void CopyToParallel(this DirectoryInfo sourceFolder, DirectoryInfo destinationFolder, bool overwrite, int numberOfTasks, ExcludeCondition excludeCondition = null, Action<string> executeFileLog = null)
        {
            _CopyToParallel(sourceFolder, destinationFolder, overwrite, numberOfTasks, excludeCondition, executeFileLog);
        }
        public static void DeleteSpecialFiles(this DirectoryInfo folder, List<string> fileNames, List<string> prefixFileNames, List<string> fileExtensions)
        {
            try
            {
                _DeleteSpecialFiles(folder, fileNames, prefixFileNames, fileExtensions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int SynchronizeFolder(this DirectoryInfo srcFolder, DirectoryInfo destFolder, bool checkMd5 = false, ExcludeCondition excludeCondition = default)
        {
            List<string> excFileNames = new List<string>()
                , excFileExtensions = new List<string>()
                , excFolderNames = new List<string>()
                , excFilePreNames = new List<string>()
                , excFolderPreNames = new List<string>();

            if (excludeCondition != null)
            {
                excFileExtensions = excludeCondition.FileExtensions ?? new List<string>();
                excFileNames = excludeCondition.FileNames ?? new List<string>();
                excFilePreNames = excludeCondition.FilePrefixNames ?? new List<string>();
                excFolderNames = excludeCondition.FolderNames ?? new List<string>();
                excFolderPreNames = excludeCondition.FolderPrefixNames ?? new List<string>();
            }
            return _SynchronizeFolder(srcFolder, destFolder, checkMd5, excFileNames, excFilePreNames, excFileExtensions, excFolderNames, excFolderPreNames);
        }
        public static void BackupFolder(this DirectoryInfo sourceFolder, bool overwrite, ExcludeCondition excludeCondition = null, Action<string> executeFileLog = null)
        {
            DirectoryInfo destinationFolder = new DirectoryInfo(sourceFolder.FullName + "/@BK");
            _CopyTo(sourceFolder, destinationFolder, overwrite, excludeCondition, executeFileLog);
        }
        #endregion
    }
}
