using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSSystem.IO.Extensions;
using VSSystem.Logger;
using VSSystem.ServiceProcess.Extensions;
using VSSystem.Threading.Tasks.Extensions;

namespace VSSystem.ServiceProcess.Workers
{
    public abstract class PoolWorker : IntervalWorker
    {
        protected const string _SIGN_FILE_EXTENSION = ".sign";
        protected string _networkSharedPoolPath = "";
        protected DirectoryInfo _poolFolder;
        protected Action _initPoolFolderAction;
        protected int _signFileLevel;
        protected string _signFileExtension, _processFileExtension;
        protected bool _deleteSignFileWhenFinish;
        protected bool _deleteFolderSignFileWhenFinish;
        protected SearchOption _searchOption;
        
        public PoolWorker(IntervalWorkerStartInfo startInfo, ALogger logger = null) 
            : base(startInfo, logger)
        {
            _networkSharedPoolPath = "";
            _initPoolFolderAction = null;
            _signFileLevel = 0;
            _signFileExtension = _SIGN_FILE_EXTENSION;
            _deleteSignFileWhenFinish = true;
            _processFileExtension = string.Empty;
            _deleteFolderSignFileWhenFinish = false;
            _type = EWorkerType.Pool;
            _searchOption = SearchOption.TopDirectoryOnly;
        }
        protected virtual bool PreProcessPoolFolder() { return true; }

        void MainProcessPoolFolder(CancellationToken cancellationToken)
        {
            try
            {
                if (!_poolFolder.Exists)
                {
                    this.LogWarningAsync(_poolFolder.FullName + " not found. Please check again.");
                    return;
                }
                ProcessPoolFolder(_poolFolder, 0, cancellationToken);
            }
            catch (Exception ex)
            {
                this.LogErrorAsync(ex);
            }
        }

        protected virtual void MainFileProcess(CancellationToken cancellationToken)
        {
            try
            {
                if (PreProcessPoolFolder())
                {
                    MainProcessPoolFolder(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                this.LogErrorAsync(ex);
            }
        }

        protected virtual void ProcessPoolFolder(DirectoryInfo folder, int level, CancellationToken cancellationToken)
        {
            try
            {
                if (level == _signFileLevel)
                {
                    FileInfo[] signFiles = folder.GetFiles("*" + _signFileExtension, _searchOption);
                    if(signFiles?.Length > 0)
                    {
                        if (_NumberOfThreads > 1)
                        {
                            signFiles.ConsecutiveRun(ite =>
                            {
                                ProcessSignFile(ite, cancellationToken);
                                try
                                {
                                    if (ite.Exists && _deleteSignFileWhenFinish)
                                    {
                                        ite.Delete();
                                    }
                                }
                                catch { }
                            }, _NumberOfThreads);
                            if(_deleteFolderSignFileWhenFinish)
                            {
                                try
                                {
                                    folder.Empty(true);
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            foreach (FileInfo signFile in signFiles)
                            {
                                ProcessSignFile(signFile, cancellationToken);
                                try
                                {
                                    if (signFile.Exists && _deleteSignFileWhenFinish)
                                    {
                                        signFile.Delete();
                                    }
                                }
                                catch { }
                                
                            }
                            if (_deleteFolderSignFileWhenFinish)
                            {
                                try
                                {
                                    folder.Empty(true);
                                }
                                catch { }
                            }
                        }
                    }                    
                }
                else
                {
                    DirectoryInfo[] subFolders = folder.GetDirectories();
                    foreach (DirectoryInfo sfolder in subFolders)
                    {
                        ProcessPoolFolder(sfolder, level + 1, cancellationToken);
                    }
                }

            }
            catch (Exception ex)
            {
                this.LogErrorAsync(ex);
            }
        }


        protected virtual void ProcessSignFile(FileInfo signFile, CancellationToken cancellationToken) 
        {
            if(!string.IsNullOrWhiteSpace(_processFileExtension))
            {
                FileInfo processFile = GetProcessFile(signFile, _processFileExtension);
                if(processFile.Exists)
                {
                    ProcessFile(processFile, cancellationToken);
                    if(_deleteSignFileWhenFinish)
                    {

                        try
                        {
                            processFile.Attributes = FileAttributes.Archive;
                            processFile.Delete();
                        }
                        catch { }
                    }
                }
            }
        }
        protected virtual void ProcessFile(FileInfo processFile, CancellationToken cancellationToken) { }

        protected override Task _RunInternalTasksAsync(CancellationToken cancellationToken)
        {
            try
            {

                _initPoolFolderAction?.Invoke();
                bool checkAgain = false;
                if (!string.IsNullOrWhiteSpace(_networkSharedPoolPath))
                {
                    _poolFolder = new DirectoryInfo(_networkSharedPoolPath + "/" + _poolFolder.Name);
                }
            CheckPoolFolderAgain:
                if (!_poolFolder.Exists && !checkAgain && !string.IsNullOrWhiteSpace(_networkSharedPoolPath))
                {
                    _poolFolder = new DirectoryInfo(_networkSharedPoolPath + "/" + _poolFolder.Name);
                    checkAgain = true;
                    goto CheckPoolFolderAgain;
                }

                if (!_poolFolder.Exists)
                {
                    _poolFolder.Create();
                    return Task.CompletedTask;
                }
                MainFileProcess(cancellationToken);
            }
            catch (Exception ex)
            {
                this.LogErrorAsync(ex);
            }
            return Task.CompletedTask;
        }
        protected FileInfo GetProcessFile(FileInfo signFile, string fileExtension)
        {
            return new FileInfo(signFile.DirectoryName + "/" + Path.GetFileNameWithoutExtension(signFile.Name) + fileExtension);
        }
    }
}
