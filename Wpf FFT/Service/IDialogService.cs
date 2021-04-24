using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf_FFT.MVVM.Base;

namespace Wpf_FFT.Service
{
    public interface IDialogService
    {
        void RegisterDialog<TViewModel, TView>()
            where TViewModel : IDialogVM
            where TView : IDialog;
        string ShowDialog<TViewModel>() where TViewModel : IDialogVM;
        string ShowDialog<TViewModel>(string message) where TViewModel : IDialogVM;
    }
}
