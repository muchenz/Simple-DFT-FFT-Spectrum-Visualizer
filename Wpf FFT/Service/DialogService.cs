using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf_FFT.MVVM.Base;

namespace Wpf_FFT.Service
{
    class DialogService : IDialogService
    {
        Dictionary<Type, Type> mapping;

        public DialogService()
        {
            mapping = new Dictionary<Type, Type>();
        }
        public void RegisterDialog<TViewModel, TView>() where TViewModel : IDialogVM where TView : IDialog
        {
            mapping.Add(typeof(TViewModel), typeof(TView));
        }

        public string ShowDialog<TViewModel>() where TViewModel : IDialogVM
        {

            var dial = (IDialog)Activator.CreateInstance(mapping[typeof(TViewModel)]);
            var dialMVVM = (TViewModel)Activator.CreateInstance(typeof(TViewModel), dial);

            dial.DataContext = dialMVVM;

            dialMVVM.Show();

            return dialMVVM.Results;
        }

        public string ShowDialog<TViewModel>(string message) where TViewModel : IDialogVM
        {

            var dial = (IDialog)Activator.CreateInstance(mapping[typeof(TViewModel)]);
            var dialMVVM = (TViewModel)Activator.CreateInstance(typeof(TViewModel), dial, message);

            dial.DataContext = dialMVVM;

            dialMVVM.Show();

            return dialMVVM.Results;
        }
    }
}
