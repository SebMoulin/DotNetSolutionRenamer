using System;
using System.Collections.Generic;

namespace SolutionTemplateRenamer.Business
{
    public interface IRenameDotNetSolutionService
    {
        IEnumerable<RenamingProcessStatus> Rename();
        string[] GetSupportedFileExtentions();
        RenamingProcessStatus Initialize(string oldName, string newName, string folderFullPath);
    }

    public class RenamingProcessStatus
    {
        public RenamingStatus Status { get; set; }

        public RenamingProcessStep RenamingProcessStep { get; set; }
        public string Error { get; set; }
        public InContentReplacementStatus InContentReplacementStatus { get; set; }
    }

    public enum RenamingStatus
    {
        Ok,
        Error
    }

    public enum RenamingProcessStep
    {
        ReplacingContent,
        RenamingFilesAndFolder,
        Done
    }

    public class InContentReplacementStatus
    {
        public int Current { get; set; }
        public int Total { get; set; }
    }
}