using System;
using System.Diagnostics;
using System.IO;
using Prism.Commands;
using Prism.Mvvm;

namespace UAS_Utility.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private string _captureFolder;
        private string _statusMessage;
        private DelegateCommand _openCaptureFolderCommand;
        private DelegateCommand _resetCommand;

        public SettingsViewModel()
        {
            CaptureFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Captures");
            Hotkey = "Ctrl + Shift + Q";
            Theme = "Light / Orange";
        }

        public string CaptureFolder
        {
            get => _captureFolder;
            private set => SetProperty(ref _captureFolder, value);
        }

        public string Hotkey { get; }

        public string Theme { get; }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public DelegateCommand OpenCaptureFolderCommand =>
            _openCaptureFolderCommand ??= new DelegateCommand(OpenCaptureFolder);

        public DelegateCommand ResetCommand =>
            _resetCommand ??= new DelegateCommand(ResetSettings);

        private void OpenCaptureFolder()
        {
            Directory.CreateDirectory(CaptureFolder);
            Process.Start("explorer.exe", CaptureFolder);
            StatusMessage = "Capture folder opened.";
        }

        private void ResetSettings()
        {
            CaptureFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Captures");
            StatusMessage = "Settings restored for this session.";
        }
    }
}
