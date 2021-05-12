using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using GI.Screenshot;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using ScreenCapturerNS;
using UAS_Utility.Views;
using WK.Libraries.HotkeyListenerNS;

namespace UAS_Utility.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private DelegateCommand _captureScreenCommand;
        private string _title = "UAS Utility";
        private readonly HotkeyListener _hkl = new HotkeyListener();
        private readonly Hotkey _hotkey1;
        public ShellViewModel(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(Coordinates));

            _hotkey1 = new Hotkey(Keys.Control | Keys.Shift, Keys.Q);
            _hkl.Add(_hotkey1);
            _hkl.HotkeyPressed += _hkl_HotkeyPressed;
        }

        private void _hkl_HotkeyPressed(object sender, HotkeyEventArgs e)
        {
            if (e.Hotkey == _hotkey1)
            {
                CaptureScreen();
            }
        }

        public DelegateCommand CaptureScreenCommand => _captureScreenCommand ??= new DelegateCommand(CaptureScreen);

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public void CaptureScreen()
        {
            var capture = Screenshot.CaptureAllScreens();
            var filename = Path.Combine("Captures", $"{DateTime.Now:G}.bmp".Replace("/", "-").Replace(":", "-"));
            using var fileStream = new FileStream(filename, FileMode.Create);
            var encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Frames.Add(BitmapFrame.Create(capture));
            encoder.Save(fileStream);
        }
    }
}