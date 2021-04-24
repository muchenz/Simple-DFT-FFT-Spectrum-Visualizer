using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Wpf_FFT.MVVM;
using Wpf_FFT.Service;
using Wpf_FFT.View;

namespace Wpf_FFT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UnityContainer Container { get; set; }
        public App()
        {
            InitContainer();
        }

        private void InitContainer()
        {
            App.Container = new UnityContainer();

            Container.RegisterType<MainWindow>();
            Container.RegisterType<MainWindowVM>();


            var dialogService = new DialogService();
            dialogService.RegisterDialog<DialogOkVM, DialogOk>();
            Container.RegisterInstance<IDialogService>(dialogService);

            var mainWindow = Container.Resolve<MainWindow>();
            
            mainWindow.Show();
        }
    }
}
