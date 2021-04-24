using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf_FFT.View
{
    /// <summary>
    /// Interaction logic for ChartControl.xaml
    /// </summary>
    public partial class ChartControl : UserControl
    {
        public ChartControl()
        {
            InitializeComponent();

            Series = new List<ISeries> {
                new LineSeries<double>
                 {
                     Values = new List<double> { },
                     Fill = null
                 }
            };

            XAxe = (new List<IAxis> { new Axis { } }).Cast<IAxis>().ToList();
        }


        public IEnumerable<IAxis> XAxe
        {
            get { return (IEnumerable<IAxis>)GetValue(XAxeProperty); }
            set { SetValue(XAxeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XAxe.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XAxeProperty =
            DependencyProperty.Register("XAxe", typeof(IEnumerable<IAxis>), typeof(ChartControl), new PropertyMetadata(default(IEnumerable<IAxis>)));


        public IEnumerable<ISeries> Series
        {
            get { return (IEnumerable<ISeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(IEnumerable<ISeries>), typeof(ChartControl), new PropertyMetadata(default(IEnumerable<ISeries>)));



        public string XTipName
        {
            get { return (string)GetValue(XTipNameProperty); }
            set { SetValue(XTipNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XTipName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XTipNameProperty =
            DependencyProperty.Register("XTipName", typeof(string), typeof(ChartControl), new PropertyMetadata(string.Empty));


    }
}
