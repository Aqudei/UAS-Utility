using Prism.Mvvm;
using Prism.Regions;
using UAS_Utility.Views;
using Unity;

namespace UAS_Utility.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private string _title = "UAS Utility";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ShellViewModel(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(Coordinates));
        }
    }
}
