using System;
using System.Configuration;
namespace FileZipper
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourceFolder = ConfigurationManager.AppSettings["SourceFolder"];
            string destinationFolder = ConfigurationManager.AppSettings["DestinationFolder"];
            string template = ConfigurationManager.AppSettings["Template"];
            string zippedFileName = ConfigurationManager.AppSettings["ZippedFileName"];
            int deleteAfterDays = Int32.Parse(ConfigurationManager.AppSettings["Days"]);

            FileFolderValidator validator = new FileFolderValidator();
            FilesManager filesManager = new FilesManager(validator);
            validator.CheckConfiguredVariable(sourceFolder);
            validator.CheckConfiguredVariable(destinationFolder);
            validator.CheckConfiguredVariable(template);
            validator.CheckConfiguredVariable(zippedFileName);

            validator.CheckIfFolderExsists(sourceFolder);
            validator.CheckIfFolderIsEmpty(sourceFolder);

            validator.CheckIfFolderExsists(destinationFolder);

            string fileName = zippedFileName.Replace("{date}", DateTime.Now.ToString("dd-MM-yy"));
            string zipPath = Path.Combine(sourceFolder, fileName);
            string newZipPath = filesManager.CreateNewPathIfNeeded(zipPath);

            filesManager.ZipFiles(sourceFolder, newZipPath, template, deleteAfterDays);
            filesManager.MoveFiles(zipPath, destinationFolder);
            filesManager.DeleteFiles(sourceFolder, template);
        }
    }
}
