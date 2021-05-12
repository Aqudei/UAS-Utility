using System;
using System.IO;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Directory.CreateDirectory("Captures");
        }
    }
}
