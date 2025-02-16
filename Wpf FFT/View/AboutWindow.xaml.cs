using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf_FFT.MVVM;
using Wpf_FFT.MVVM.Base;

namespace Wpf_FFT.View
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window, IDialog
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var pi = new ProcessStartInfo("https://github.com/muchenz/Simple-DFT-FFT-Spectrum-Visualizer")
            {
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(pi);
        }

        private void HyperlinkWiki_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/muchenz/RefreshRateWpfApp/wiki");

        }

        private void okButton_Click(object sender, RoutedEventArgs e) =>
          DialogResult = true;

    }
}
