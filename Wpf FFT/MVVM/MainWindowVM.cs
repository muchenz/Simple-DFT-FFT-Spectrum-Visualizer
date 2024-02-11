using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using org.mariuszgromada.math.mxparser;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
using Unity;
using Wpf_FFT.Commands;
using Wpf_FFT.Service;
using Wpf_FFT.View;

namespace Wpf_FFT.MVVM
{
    /// <summary>
    /// Purpose:
    /// Created By: muchenz
    /// Created On: 4/17/2021 1:33:30 PM
    /// </summary>
    public class MainWindowVM : ObservableObject, IDataErrorInfo
    {
        private readonly IDialogService _dialogService;
        public MainWindowVM(IDialogService dialogService)
        {
            LiveCharts.Configure(a => a.HasMap<DataModel>((b, c) => { c.PrimaryValue = (float)b.Value; c.SecondaryValue = b.Second; }));
            _dialogService = dialogService;
            Compute.Execute(null);
        }
        public MainWindowVM()
        {
            /*
             *    only for design
             *    
             */
            LiveCharts.Configure(a => a.HasMap<DataModel>((b, c) => { c.PrimaryValue = (float)b.Value; c.SecondaryValue = b.Second; }));

        }



        //private string _functionToFFT2 = "if( sin(2.0 * pi * time * frequency)>0 , 1, -1)";
        private string _functionToFFT = "sin(2.0 * pi* t* f)*e^(t*100)";

        public string FunctionToFFT
        {
            get { return _functionToFFT; }
            set
            {
                _functionToFFT = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand Compute
        {
            get
            {
                var t = new ActionCommand((o) =>
                {
                    try
                    {
                        ComputeTransformation();
                    }
                    catch (CalculateException ex)
                    {
                        //ErrorMessage = "Some errers occurs during calculation. Please modify function or arguments.";
                        _dialogService.ShowDialog<DialogOkVM>("Some errers occurs during calculation. Please modify function or arguments.");

                    }

                }, (o) =>
                {

                    //System.Diagnostics.Trace.WriteLine("a");
                    return string.IsNullOrEmpty(ErrorMessage);
                });
                // PropertyChanged += t.NotifyMaybyExecuteChanged;
                return t;
            }
        }


        List<ISeries> _seriesFrequency { get; set; } = new List<ISeries>
        {
            new LineSeries<DataModel>
            {
                Values =  new List<DataModel> { new DataModel { Value=2, Second = 0f }, new DataModel { Value=2, Second=1f },
                new DataModel { Value=3, Second = 3f }, new DataModel { Value=5, Second=4f },
                new DataModel { Value=3, Second = 5f }, new DataModel { Value=4, Second=6f },
                new DataModel { Value=6, Second = 7f }, new DataModel { Value=2, Second=8f }},
                Fill = null
            }
        };

        public List<ISeries> SeriesFrequency
        {
            get { return _seriesFrequency; }
            set
            {
                _seriesFrequency = value;
                NotifyPropertyChanged();
            }
        }

        List<ISeries> _seriesTime { get; set; } = new List<ISeries>
        {
            new LineSeries<double>
            {
                Values = new List<double> { 2, 1, 3, 5, 3, 4, 6 },
                Fill = null
            }
        };

        public List<ISeries> SeriesTime
        {
            get { return _seriesTime; }
            set
            {
                _seriesTime = value;
                NotifyPropertyChanged();
            }
        }

        public List<Axis> _xAxe = (new List<Axis> { new Axis {
         SeparatorsBrush = new SolidColorPaintTask { Color = new SKColor(245,245,245),
             StrokeThickness = 1 },

        } });
        public List<Axis> XAxe
        {
            get { return _xAxe; }
            set
            {
                _xAxe = value;
                NotifyPropertyChanged();
            }
        }

        public List<Axis> XAxe2 => new List<Axis> { new Axis {
         SeparatorsBrush = new SolidColorPaintTask { Color = new SKColor(245,245,245),
             StrokeThickness = 1 },
          } };

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {

                if (columnName == nameof(FunctionToFFT))
                {
                    org.mariuszgromada.math.mxparser.Argument timeArg = new("t", 1);
                    org.mariuszgromada.math.mxparser.Argument frequencyArg = new("f", 2);

                    org.mariuszgromada.math.mxparser.Expression func = new(FunctionToFFT, timeArg, frequencyArg);

                    if (!func.checkSyntax())
                        return ErrorMessage = "Function Syntax Error";
                }
                else if (columnName == nameof(Frequency))
                {
                    double d;
                    if (!double.TryParse(Frequency, System.Globalization.NumberStyles.Any, cultureUS, out d))
                        return ErrorMessage = "'Frequerency' field error";
                }
                else if (columnName == nameof(Lenght))
                {
                    double d;
                    if (!double.TryParse(Lenght, System.Globalization.NumberStyles.Any, cultureUS, out d))
                        return ErrorMessage = "'Number of pts' field error";

                }
                else if (columnName == nameof(SamplingFrequency))
                {
                    double d;
                    if (!double.TryParse(SamplingFrequency, System.Globalization.NumberStyles.Any, cultureUS, out d))
                        return ErrorMessage = "'Sampling Frequency' field error";
                }
                else if (columnName == nameof(ZerosNumber))
                {
                    uint d;
                    if (!uint.TryParse(ZerosNumber, System.Globalization.NumberStyles.Any, cultureUS, out d))
                        return ErrorMessage = "'Number od zeros' field error";
                }

                if (IsFFT && (columnName == nameof(Lenght) || columnName == nameof(ZerosNumber)))
                {

                    var d = UInt32.Parse(Lenght, cultureUS) + uint.Parse(ZerosNumber, cultureUS);

                    if (!(Math.Log2(d) == ((double)((int)Math.Log2(d)))))
                        return ErrorMessage = "Sum 'Number of pts' and 'Zeros' must be power of 2";
                }

                return ErrorMessage = null;
            }
        }

        private string _frequency = "2000";
        public string Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                NotifyPropertyChanged();
            }
        }

        private string _lenght = "2048";
        public string Lenght
        {
            get { return _lenght; }
            set
            {
                _lenght = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ZerosNumber));
            }
        }
        private string _samplingFrequency = "100000";
        public string SamplingFrequency
        {
            get { return _samplingFrequency; }
            set
            {
                _samplingFrequency = value;
                NotifyPropertyChanged();
            }
        }
        private string _zerosNubmer = "0";
        public string ZerosNumber
        {
            get { return _zerosNubmer; }
            set
            {
                _zerosNubmer = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Lenght));
            }
        }

        public string[] WindowFunctions => Enum.GetNames(typeof(DSP.Window.Type));

        private string _windowFunction = "None";
        public string WindowFunction
        {
            get { return _windowFunction; }
            set
            {
                _windowFunction = value;
                NotifyPropertyChanged();
            }
        }

        bool _isMagnitude = true;
        public bool IsMagnitude
        {
            get { return _isMagnitude; }
            set
            {
                _isMagnitude = value;
                NotifyPropertyChanged();
            }
        }

        bool _isFFT = true;
        public bool IsFFT
        {
            get { return _isFFT; }
            set
            {
                _isFFT = value;
                NotifyPropertyChanged();
                refreshPtsAndZeros();
            }
        }

        void refreshPtsAndZeros()
        {
            NotifyPropertyChanged(nameof(ZerosNumber));
            NotifyPropertyChanged(nameof(Lenght));
        }

        bool _isShift = true;
        public bool IsShift
        {
            get { return _isShift; }
            set
            {
                _isShift = value;
                NotifyPropertyChanged();
            }
        }
        System.IFormatProvider cultureUS = new System.Globalization.CultureInfo("en-US");
        Complex[] _cSpectrum;
        double[] _freqSpan;
        bool _oldShiftValue;
        void ComputeTransformation()
        {
            // Generate a test signal,
            //  1 Vrms at 20,000 Hz
            //  Sampling Rate = 100,000 Hz
            //  DFT Length is 1000 Points
            double frequency = double.Parse(Frequency, cultureUS);
            UInt32 lengthSample = UInt32.Parse(Lenght, cultureUS);
            UInt32 zeros = UInt32.Parse(ZerosNumber, cultureUS);
            double samplingRate = double.Parse(SamplingFrequency);

            Argument timeArg, frequencyArg;
            org.mariuszgromada.math.mxparser.Expression expression;

            FttHelper.GetExpressionFromString(FunctionToFFT, out timeArg, out frequencyArg, out expression);

            double[] inputSignal = FttHelper.GetInputData(frequency, lengthSample, samplingRate, timeArg, frequencyArg, expression);


            // Apply window to the time series data
            double[] wc = DSP.Window.Coefficients((DSP.Window.Type)Enum.Parse(typeof(DSP.Window.Type), WindowFunction), lengthSample);

            double windowScaleFactor = DSP.Window.ScaleFactor.Signal(wc);
            double[] windowedTimeSeries = DSP.Math.Multiply(inputSignal, wc);


            _oldShiftValue = IsShift;
            if (IsFFT)
            {
                FFT fft = new();
                fft.Initialize(lengthSample, zeros, fullFrequencyData: IsShift);
                _cSpectrum = fft.Execute(windowedTimeSeries);
                _cSpectrum = fft.ShiftData(_cSpectrum, IsShift);

                _freqSpan = fft.FrequencySpan(samplingRate);
            }
            else
            {
                // Instantiate a new DFT
                DFT dft = new DFT();
                // Initialize the DFT
                // You only need to do this once or if you change any of the DFT parameters.

                dft.Initialize(lengthSample, zeros, fullFrequency: IsShift);
                // Call the DFT and get the scaled spectrum back
                _cSpectrum = dft.Execute(windowedTimeSeries);
                _cSpectrum = dft.ShiftData(_cSpectrum, IsShift);
                // For plotting on an XY Scatter plot, generate the X Axis frequency Span
                _freqSpan = dft.FrequencySpan(samplingRate);
            }



            // Note: At this point, lmSpectrum is a 501 byte array that 
            // contains a properly scaled Spectrum from 0 - 50,000 Hz (1/2 the Sampling Frequency)


            // Convert the complex spectrum to magnitude

            double[] lmSpectrum = DSP.ConvertComplex.ToMagnitude(_cSpectrum);
            IsMagnitude = true;
            // double[] lmSpectrum = _cSpectrum.ToList().Select(a => a.Magnitude).ToArray();

            // At this point a XY Scatter plot can be generated from,
            // X axis => freqSpan
            // Y axis => lmSpectrum

            // In this example, the maximum value of 1 Vrms is located at bin 200 (20,000 Hz)


            // XAxe = ChartHelper.GetXAxisLabelForFrequencyCart(freqSpan);
            SeriesFrequency = ChartHelper.GetSeriesFrequency(lmSpectrum, _freqSpan, IsShift);

            // ((LineSeries<DataModel>)SeriesFrequency[0]).GeometryFill.StrokeThickness = 0.5f;


            SeriesTime = ChartHelper.GetSeriesTime(windowedTimeSeries, samplingRate);

        }



        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand AmplitudeModulation => new ActionCommand((o) =>
        {
            FunctionToFFT = "(1 + 0.3 * sin(2 * 4*pi *  t*f)) * sin(2 * pi * 40 * t*f)";
            Frequency = "500";
            Lenght = "2048";
            ZerosNumber = "0";
            SamplingFrequency = "100000";

        });

        public ICommand SquareWave => new ActionCommand((o) =>
        {
            FunctionToFFT = "if( sin(2.0 * pi * t * f)>0 , 1, -1)";
            Frequency = "2000";
            Lenght = "2048";
            ZerosNumber = "0";
            SamplingFrequency = "100000";

        }); public ICommand Impulse => new ActionCommand((o) =>
        {
            FunctionToFFT = "if(t>0.004&&t<0.0045, 1, 0)";
            Frequency = "2000";
            Lenght = "2048";
            ZerosNumber = "0";
            SamplingFrequency = "100000";

        });

        public ICommand ImpulseOfSine => new ActionCommand((o) =>
        {
            FunctionToFFT = "if (t > 0.004 && t < 0.0055, sin(2.0 * pi * t * f), 0)";
            Frequency = "2000";
            Lenght = "2048";
            ZerosNumber = "0";
            SamplingFrequency = "100000";

        });
        public ICommand SinePlusNoise => new ActionCommand((o) =>
        {
            FunctionToFFT = "sin(2.0 * pi* t * f)+rUni(0,2)-rUni(0,2)";
            Frequency = "2000";
            Lenght = "2048";
            ZerosNumber = "0";
            SamplingFrequency = "100000";

        });
        public ICommand Tringle => new ActionCommand((o) =>
        {
            FunctionToFFT = "  8*(abs(mod(512*t,1)-0.5)-0.25)  ";
            Frequency = "2000";
            Lenght = "2048";
            ZerosNumber = "0";
            SamplingFrequency = "100000";

        });

        public ICommand FrequerencyModulation => new ActionCommand((o) =>
        {
            FunctionToFFT = "sin(2.0 * pi* t* f*sin(2.0 * pi* t* f/20))";
            Frequency = "2000";
            Lenght = "2048";
            ZerosNumber = "0";
            SamplingFrequency = "100000";

        });

        public ICommand ShiftCommand => new ActionCommand((o) => { }, (o) => true);
        public ICommand MagnitudeCommand => new ActionCommand((o) =>
        {
            if (_cSpectrum == null) return;

            double[] lmSpectrum = DSP.ConvertComplex.ToMagnitude(_cSpectrum);
            SeriesFrequency = ChartHelper.GetSeriesFrequency(lmSpectrum, _freqSpan, _oldShiftValue);

        }, (o) => true);
        public ICommand MagnitudeDBVCommand => new ActionCommand((o) =>
        {
            if (_cSpectrum == null) return;

            double[] lmSpectrum = DSP.ConvertComplex.ToMagnitudeDBV(_cSpectrum);
            SeriesFrequency = ChartHelper.GetSeriesFrequency(lmSpectrum, _freqSpan, _oldShiftValue);

        }, (o) => true);
        public ICommand ImaginaryCommand => new ActionCommand((o) =>
        {
            if (_cSpectrum == null) return;
            double[] lmSpectrum = _cSpectrum.ToList().Select(a => a.Imaginary).ToArray();
            SeriesFrequency = ChartHelper.GetSeriesFrequency(lmSpectrum, _freqSpan, _oldShiftValue);
        }, (o) => true);
        public ICommand RealCommand => new ActionCommand((o) =>
        {
            if (_cSpectrum == null) return;

            double[] lmSpectrum = _cSpectrum.ToList().Select(a => a.Real).ToArray();
            SeriesFrequency = ChartHelper.GetSeriesFrequency(lmSpectrum, _freqSpan, _oldShiftValue);
        }, (o) => true);
        public ICommand PhaseCommand => new ActionCommand((o) =>
        {
            if (_cSpectrum == null) return;

            double[] lmSpectrum = _cSpectrum.ToList().Select(a => a.Phase).ToArray();
            SeriesFrequency = ChartHelper.GetSeriesFrequency(lmSpectrum, _freqSpan, _oldShiftValue);
        }, (o) => true);

        public ICommand AboutCommand => new ActionCommand((o) =>
        {
            _dialogService.ShowDialog<AboutWindowVM>();

        }, (o) => true);

        public ICommand ExitCommand => new ActionCommand((o) =>
        {
            
        }, (o) => true);

    }

    public class DataModel : INotifyPropertyChanged
    {
        private double value;
        public double Value
        {
            get => this.value;
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }

        private float _second;
        public float Second
        {
            get => this._second;
            set
            {
                this._second = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CalculateException : Exception
    {

        public CalculateException(string message) : base(message)
        {

        }

    }
}
