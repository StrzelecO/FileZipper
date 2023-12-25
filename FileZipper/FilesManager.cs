using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileZipper
{
    class FilesManager
    {
        private FileFolderValidator validator;
        public FilesManager(FileFolderValidator validator)
        {
            this.validator = validator;
        }

        public void ZipFiles(string sourceFolder, string zipPath, string template, int days)
        {
            string[] files = Directory.GetFiles(sourceFolder);
            DateTime someDays = DateTime.Now.AddDays(-days);
            var oldFiles = files.Where(f =>
            {
                FileInfo fileInfo = new FileInfo(f);
                return fileInfo.CreationTime < someDays;
            }).ToList();

            var oldMatchingFiles = oldFiles;

            if (!string.IsNullOrEmpty(template))
            {
                oldMatchingFiles = oldFiles.Where(f => Regex.IsMatch(Path.GetFileName(f), template)).ToList();
            }

            if(oldMatchingFiles.Count == 0)
            {
                LoggerManager.logger.Warn($"There are no files that meet the requirements in source folder: {sourceFolder}");
                Environment.Exit(1);
            }

            try
            {
                using(ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    foreach(var file in oldMatchingFiles)
                    {
                        archive.CreateEntryFromFile(file,Path.GetFileName(file));
                    }
                    LoggerManager.logger.Info($"Zip files from ${sourceFolder} to ${zipPath}");
                }
            }
            catch (Exception ex)
            {
                LoggerManager.logger.Error($"Failed to zip files from {sourceFolder} - {ex.Message}");
            }
        }

        public void MoveFiles(string zipPath, string destinatonFolder)
        {
            validator.CheckIfFolderExsists(destinatonFolder);

            string destinatonPath = Path.Combine(destinatonFolder, Path.GetFileName(zipPath));
            destinatonPath = CreateNewPathIfNeeded(destinatonPath);
            validator.CheckIfPathExsists(destinatonPath);

            if(Path.GetPathRoot(zipPath) == Path.GetPathRoot(destinatonFolder))
            {
                try
                {
                    Directory.Move(zipPath, destinatonPath);
                    LoggerManager.logger.Info($"Move zipped files from {zipPath} to {destinatonPath}");
                }
                catch (Exception ex)
                {
                    LoggerManager.logger.Error($"Failed to move folder - {ex.Message}");
                    File.Delete(zipPath);
                    Environment.Exit(1);
                }
            }
            else
            {
                CopyZippedFolder(zipPath, Path.GetDirectoryName(destinatonPath));
                try
                {
                    File.Delete(zipPath);
                    LoggerManager.logger.Info($"Delete folder: {zipPath}");
                }
                catch (Exception ex)
                {
                    LoggerManager.logger.Error($"Failed to delete unnecessary folder: {zipPath} - {ex.Message}");
                    Environment.Exit(1);
                }
            }
        }

        public void CopyZippedFolder(string sourceFolder, string destinatonFolder)
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(sourceFolder));
            Directory.CreateDirectory(destinatonFolder);
            FileInfo[] allFiles = dir.GetFiles();
            try
            {
                foreach (FileInfo fi in allFiles)
                {
                    if(fi.Extension == ".zip")
                    {
                        string targetFilePath = Path.Combine(destinatonFolder, fi.Name);
                        targetFilePath = CreateNewPathIfNeeded(targetFilePath);
                        fi.CopyTo(targetFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.logger.Error($"Files to copy zipped folder: {sourceFolder} - {ex.Message}");
                Environment.Exit(0);
            }
        }

        public void DeleteFiles(string path)
        {
            DeleteFiles(path, null);
        }

        public void DeleteFiles(string path, string template)
        {
            string[] files = Directory.GetFiles(path);
            if (!string.IsNullOrEmpty(template))
            {
                foreach (var file in files)
                {
                    try
                    {
                        if (Regex.IsMatch(file, template))
                            File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        LoggerManager.logger.Error($"Failed to delete files from: {path}  - {ex.Message}");
                        Environment.Exit(1);
                    }
                }
            }
            else
            {
                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        LoggerManager.logger.Error($"Failed to delete files from: {path}  - {ex.Message}");
                        Environment.Exit(1);
                    }
                }
                string[] folders = Directory.GetDirectories(path);
                foreach (var folder in folders)
                {
                    try
                    {
                        Directory.Delete(folder, true);
                    }
                    catch (Exception ex)
                    {
                        LoggerManager.logger.Error($"Failed to delete files from: {Path.GetFileName(folder)}  - {ex.Message}");
                        Environment.Exit(1);
                    }
                }
            }
            LoggerManager.logger.Info($"Delete files from: {path}");
        }

        public string CreateNewPathIfNeeded(string path)
        {
            int num = 0;
            string newPath = path;
            if (File.Exists(newPath))
            {
                num++;
                newPath = Path.Combine(Path.GetDirectoryName(path), $"{Path.GetFileNameWithoutExtension(path)}_{num}.zip");
            }
            return newPath;
        }
    }
}
