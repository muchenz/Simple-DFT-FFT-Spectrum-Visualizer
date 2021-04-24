using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_FFT.MVVM.Base
{
    public interface IDialogVM
    {
        string Results { get; set; }

        void Close();
        void Show();
    }
}
