using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using SolutionTemplateRenamer.Annotations;
using SolutionTemplateRenamer.Business;

namespace SolutionTemplateRenamer.ViewModels
{
    public class MainWindowsViewModel : INotifyPropertyChanged
    {
        private readonly IRenameDotNetSolutionService _solutionRenamerservice;
        private string _folderPath;
        private string _newName;
        private ICommand _launchRenameCommand;
        private ICommand _openFolderPickerCommand;
        private int _totalFilesToRename;
        private int _currentlyProcessedFiles;
        private string _processStatus;
        private string _oldName;
        private string _supportedFileExtensions;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowsViewModel([NotNull] IRenameDotNetSolutionService solutionRenamerservice)
        {
            if (solutionRenamerservice == null) throw new ArgumentNullException(nameof(solutionRenamerservice));
            _solutionRenamerservice = solutionRenamerservice;
        }

        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                OnPropertyChanged();
            }
        }

        public string OldName
        {
            get { return _oldName; }
            set
            {
                _oldName = value;
                OnPropertyChanged();
            }
        }

        public string NewName
        {
            get { return _newName; }
            set
            {
                _newName = value;
                OnPropertyChanged();
            }
        }

        public string SupportedFileExtensions
        {
            get { return _supportedFileExtensions; }
            set
            {
                _supportedFileExtensions = value;
                OnPropertyChanged();
            }
        }

        public string ProcessStatus
        {
            get { return _processStatus; }
            set
            {
                _processStatus = value;
                OnPropertyChanged();
            }
        }

        public int TotalFilesToRename
        {
            get { return _totalFilesToRename; }
            set
            {
                _totalFilesToRename = value;
                OnPropertyChanged();
            }
        }

        public int CurrentlyProcessedFiles
        {
            get { return _currentlyProcessedFiles; }
            set
            {
                _currentlyProcessedFiles = value;
                OnPropertyChanged();
            }
        }

        public ICommand LaunchRenameCommand
        {
            get { return _launchRenameCommand; }
            set
            {
                _launchRenameCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenFolderPickerCommand
        {
            get { return _openFolderPickerCommand; }
            set
            {
                _openFolderPickerCommand = value;
                OnPropertyChanged();
            }
        }

        public void TemplateApplied()
        {
            TotalFilesToRename = 0;
            CurrentlyProcessedFiles = 0;
            ProcessStatus = "Ready";
            SupportedFileExtensions = string.Join(",", _solutionRenamerservice.GetSupportedFileExtentions());
            LaunchRenameCommand = new DefaultCommand(async (o) =>
            {
                await RenameAsync();
            },
            (o) => true);

            OpenFolderPickerCommand = new DefaultCommand((o) =>
            {
                var fileDialog = new OpenFileDialog();
                var result = fileDialog.ShowDialog();
                switch (result)
                {
                    case true:
                        FolderPath = GetFolderNameFromFileName(fileDialog.FileName);
                        break;
                    case false:
                    default:
                        FolderPath = string.Empty;
                        break;
                }
            }, (o) => true);
        }

        private Task RenameAsync()
        {
            return Task.Run(() =>
            {
                var process = _solutionRenamerservice.Initialize(OldName, NewName, FolderPath);
                if (process.Status == RenamingStatus.Error)
                {
                    ProcessStatus = process.Error;
                }
                else
                {
                    foreach (var renamingProcessStatuse in _solutionRenamerservice.Rename())
                    {
                        TotalFilesToRename = renamingProcessStatuse.InContentReplacementStatus.Total;
                        CurrentlyProcessedFiles = renamingProcessStatuse.InContentReplacementStatus.Current;
                        ProcessStatus = renamingProcessStatuse.RenamingProcessStep.ToString();
                    }
                }
            });
        }

        private string GetFolderNameFromFileName(string fileName)
        {
            return Path.GetDirectoryName(fileName);
        }
    }
}
