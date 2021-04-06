using UAS_Utility.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace UAS_Utility
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}
