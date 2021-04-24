using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_FFT.MVVM.Base
{
    public interface IDialog
    {
        object DataContext { get; set; }
        bool? DialogResult { get; set; }
        //Window Owner { get; set; }
        void Close();
        bool? ShowDialog();
        void Hide();

    }
}
