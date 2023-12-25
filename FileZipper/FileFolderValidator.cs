using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileZipper
{
    class FileFolderValidator
    {
        public void CheckIfFolderIsEmpty(string path)
        {
            string[] files = null;
            string[] folders = null;
            try
            {
                files = Directory.GetFiles(path);
                folders = Directory.GetDirectories(path);
            }
            catch (Exception ex)
            {
                LoggerManager.logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }

            if(files.Length == 0 && folders.Length == 0)
            {
                LoggerManager.logger.Warn($"Folder {path} is empty - cannot zip files from it");
                Environment.Exit(1);
            }
        }
        public void CheckIfFolderExsists(string path)
        {
            if (!Directory.Exists(path) || string.IsNullOrEmpty(path))
            {
                LoggerManager.logger.Error($"Folder {path} does not exsist - cannot use it");
                Environment.Exit(1);
            }
        }

        public void CheckIfPathExsists(string path)
        {
            if (File.Exists(path))
            {
                LoggerManager.logger.Error($"Failed to save file to path: {path} - file with this path already exists");
                Environment.Exit(1);
            }
        }
        public void CheckConfiguredVariable(string v)
        {
            if (string.IsNullOrEmpty(v))
            {
                LoggerManager.logger.Error($"Variable {v} from app.config is null");
                Environment.Exit(1);
            }
        }
    }
}
