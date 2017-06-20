using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionTemplateRenamer.Business
{
    public class SolutionRenamerService : IRenameDotNetSolutionService
    {
        public const string DefaultAppTemplateName = "PXL.Apollo.Mobile";
        
        public static readonly string[] FileExtensionsToRename = new[] { ".cs", ".xaml", ".sln", ".csproj", ".xml", ".proj", ".modelproj", ".layerdiagram", ".layout", ".ruleset", ".suppressions", ".appxmanifest" };
        private string _oldName;
        private string _newName;
        private string _folderFullPath;

        public RenamingProcessStatus Initialize(string oldName, string newName, string folderFullPath)
        {
            var processStatus = new RenamingProcessStatus()
            {
                Status = RenamingStatus.Ok
            };
            if (string.IsNullOrWhiteSpace(folderFullPath))
            {
                processStatus.Status = RenamingStatus.Error;
                processStatus.Error = "Path of the solution to be renamed cannot be empty";
            }
            if (processStatus.Status == RenamingStatus.Error)
                return processStatus;
            _folderFullPath = folderFullPath;

            if (string.IsNullOrWhiteSpace(newName))
            {
                processStatus.Status = RenamingStatus.Error;
                processStatus.Error = "Application new name cannot be empty";
            }
            _newName = newName;
            _oldName = oldName;
            return processStatus;
        }
        public string[] GetSupportedFileExtentions()
        {
            return FileExtensionsToRename;
        }

        public IEnumerable<RenamingProcessStatus> Rename()
        {
            var processStatus = new RenamingProcessStatus()
            {
                Status = RenamingStatus.Ok,
                InContentReplacementStatus = new InContentReplacementStatus(),
                RenamingProcessStep = RenamingProcessStep.ReplacingContent
            };

            var filesPaths = GetFoldersAndFilesPaths(_folderFullPath);

            processStatus.InContentReplacementStatus.Total = filesPaths.Count();

            if (string.IsNullOrWhiteSpace(_oldName))
            {
                _oldName = DefaultAppTemplateName;
            }

            foreach (var inContentReplacment in ReplaceInContent(filesPaths, _oldName, _newName))
            {
                processStatus.InContentReplacementStatus.Current = inContentReplacment.Current;
                yield return processStatus;
            }

            processStatus.RenamingProcessStep = RenamingProcessStep.RenamingFilesAndFolder;
            yield return processStatus;
            ReplaceFoldersAndFileNames(_folderFullPath, _oldName, _newName);

            processStatus.RenamingProcessStep = RenamingProcessStep.Done;
            yield return processStatus;
        }

        private IEnumerable<string> GetFoldersAndFilesPaths(string sourceFolder)
        {
            return from folder in
                   Directory.EnumerateDirectories(sourceFolder, "*.*", SearchOption.AllDirectories).Concat(new[] { sourceFolder })
                   from file in Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                   where FileExtensionsToRename.Any(file.ToLower().EndsWith)
                   select file;
        }

        private void ReplaceFoldersAndFileNames(string sourceFolder, string oldAppName, string newAppName)
        {
            string filePath = null;
            while ((filePath = FindFileOrDirectory(sourceFolder, oldAppName)) != null)
            {
                var newPath = filePath.Replace(oldAppName, newAppName);

                if (Directory.Exists(filePath))
                {
                    Console.WriteLine("Renaming folder {0} to {1}", filePath, newPath);
                    Directory.Move(filePath, newPath);
                }
                else if (File.Exists(filePath))
                {
                    Console.WriteLine("Renaming file {0} to {1}", filePath, newPath);
                    File.Move(filePath, newPath);
                }
            }
        }

        private IEnumerable<InContentReplacementStatus> ReplaceInContent(IEnumerable<string> filesPaths, string oldAppName, string newAppName)
        {
            var totalFile = filesPaths.Count();
            var current = 0;
            foreach (var f in filesPaths)
            {
                var t = File.ReadAllText(f, Encoding.UTF8);

                var t2 = t.Replace(oldAppName, newAppName);

                if (t2 != t)
                {
                    Console.WriteLine("Replacing content in {0}", f);
                }

                File.SetAttributes(f, FileAttributes.Archive);
                File.WriteAllText(f, t2, Encoding.UTF8);
                current++;
                yield return new InContentReplacementStatus()
                {
                    Current = current,
                    Total = totalFile
                };
            }
        }

        private string FindFileOrDirectory(string sourceFolder, string oldAppName)
        {
            var folders = from folder in Directory.EnumerateDirectories(sourceFolder, "*.*", SearchOption.AllDirectories)
                          where folder.Contains(oldAppName)
                          select folder;

            var files = from file in Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.AllDirectories)
                        where file.Contains(oldAppName)
                        select file;

            return folders.Concat(files).FirstOrDefault();
        }
    }
}
