using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf_FFT.MVVM.Base;

namespace Wpf_FFT.View
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class DialogOk : Window, IDialog
    {
        public DialogOk()
        {
            InitializeComponent();
        }

      
    }
}
