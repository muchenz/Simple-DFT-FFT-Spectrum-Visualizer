using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf_FFT.MVVM;

namespace Wpf_FFT.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
     
        public MainWindow(MainWindowVM mainWindowVM)
        {
            InitializeComponent();
            DataContext = mainWindowVM;
        }


        //hook for scaling factor 

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);

            var xDPI = GetDeviceCaps(hdc, LOGPIXELSX);

            //var yDPI = GetDeviceCaps(hdc, LOGPIXELSY);


            var windowsScalingFactor = xDPI / 96.0;

            var fixingFactor = 1.01 + ((windowsScalingFactor-1) * 0.78);

            ColumnMainWindow1.Width = new GridLength(sizeInfo.NewSize.Width * fixingFactor - ColumnMainWindow0.Width.Value);

        }
    }
}