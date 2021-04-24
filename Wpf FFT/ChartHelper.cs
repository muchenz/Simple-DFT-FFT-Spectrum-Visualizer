using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf_FFT.MVVM;

namespace Wpf_FFT
{
    public class ChartHelper
    {

        public static List<Axis> GetXAxisLabelForFrequencyCart(double [] freqSpan)

            {
            var listOfLabels = freqSpan.ToList().Select(a => a.ToString("#.#")).ToList();

            return (new List<Axis>
            {
                new Axis
                {
                    //TextSize = 20,

                    //// TextBrush = null will not draw the axis labels.
                    //TextBrush = new SolidColorPaintTask { Color = SKColors.CornflowerBlue },

                    //// SeparatorsBrush = null will not draw the separator lines
                    //SeparatorsBrush = new SolidColorPaintTask { Color = SKColors.LightBlue, StrokeThickness = 3 },

                    Labels = listOfLabels,

                    MinStep=(freqSpan[1]-freqSpan[0]),

                }
            });
        }

        static List<DataModel> GetListOfFrequecyPoints(double[] lmSpectrum, double [] freqSpan, bool isShift)
        {

            List<DataModel> listOfFrequecyPoints = new();

            for (int i = 0; i < lmSpectrum.Length; i++)
            {
                if (!isShift)
                {
                    listOfFrequecyPoints.Add(new DataModel { Second = (float)freqSpan[i], Value = lmSpectrum[i] });
                }else
                {
                    if (i < lmSpectrum.Length / 2)
                        listOfFrequecyPoints.Add(new DataModel { Second = -(float)(freqSpan[lmSpectrum.Length/2 - i]), Value = lmSpectrum[i] });
                    else
                        listOfFrequecyPoints.Add(new DataModel { Second = (float)freqSpan[i - (lmSpectrum.Length/2) ], Value= lmSpectrum[i] });


                }
            }

            return listOfFrequecyPoints;

        }

        public static List<ISeries> GetSeriesFrequency(double[] lmSpectrum, double[] freqSpan, bool isShift)
        {

            var listOfFrequecyPoints = GetListOfFrequecyPoints(lmSpectrum, freqSpan, isShift);

            return new List<ISeries>{
                  new LineSeries<DataModel>()
                  {
                        Values = listOfFrequecyPoints,
                        Fill = null,
                        GeometrySize = 2,
                        Stroke = new SolidColorPaintTask(SKColors.DarkOliveGreen, 1),
                        LineSmoothness = 0,
                        Name="Val:",
                  }
            };
        }

        static List<DataModel> GetListOfTimePoints(double[] inputSignal, double samplingRate)
        {

            List<DataModel> listOfTimePoints = new();

            for (int i = 0; i < inputSignal.Length ; i++)
            {
                listOfTimePoints.Add(

                    new DataModel { Second = (float)(i / samplingRate), Value = inputSignal[i] }

                    );
            }

            return listOfTimePoints;
        }

        public static List<ISeries> GetSeriesTime(double[] inputSignal, double samplingRate)
        {

            var listOfTimePoints = GetListOfTimePoints(inputSignal, samplingRate);

            return new List<ISeries>{
                  new LineSeries<DataModel>()
                  {
                        Values = listOfTimePoints,
                        Fill = null,
                        GeometrySize = 2,
                        Stroke = new SolidColorPaintTask(SKColors.DarkOliveGreen, 1),
                        LineSmoothness = 0,
                        Name="Val: ",

                  }
            };
        }
    }
}
