using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf_FFT.MVVM;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace Wpf_FFT
{
    public class ChartHelper
    {

        public static List<Axis> GetXAxisLabelForFrequencyCart(double[] freqSpan)

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

        static List<DataModel> GetListOfFrequecyPoints(double[] lmSpectrum, double[] freqSpan, bool isShift)
        {

            //if (lmSpectrum.Length % 2 != 0)
            //{
            //    var list = lmSpectrum.ToList();
            //    list.RemoveAt(lmSpectrum.Length / 2);
            //    lmSpectrum = list.ToArray();
            //}

            List<DataModel> listOfFrequecyPoints = new();

            for (int i = 0; i < lmSpectrum.Length; i++)
            {
                if (!isShift)
                {
                    listOfFrequecyPoints.Add(new DataModel { Second = (float)freqSpan[i], Value = lmSpectrum[i] });
                }
                else
                {
                    if (lmSpectrum.Length % 2 == 0)
                    {

                        if (i < lmSpectrum.Length / 2 - 1)
                            listOfFrequecyPoints.Add(new DataModel { Second = -(float)(freqSpan[lmSpectrum.Length / 2 - i - 1]), Value = lmSpectrum[i] });
                        else
                            listOfFrequecyPoints.Add(new DataModel { Second = (float)freqSpan[i - (lmSpectrum.Length / 2) + 1], Value = lmSpectrum[i] });
                    }
                    else
                    {
                        if (i < lmSpectrum.Length / 2)
                            listOfFrequecyPoints.Add(new DataModel { Second = -(float)(freqSpan[lmSpectrum.Length / 2 - i]), Value = lmSpectrum[i] });
                        else
                            listOfFrequecyPoints.Add(new DataModel { Second = (float)freqSpan[i - (lmSpectrum.Length / 2)], Value = lmSpectrum[i] });
                    }
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
                        GeometrySize = 0.2,
                        //Stroke = new SolidColorPaintTask(SKColors.DarkOliveGreen, 1),
                        Stroke = new SolidColorPaint(SKColors.DarkOliveGreen, 1),

                        LineSmoothness = 0,
                       // Name="Val:",
                        //Mapping=(model, i)=>{ }
                        YToolTipLabelFormatter=(chartPoint) => $"Val:\xa0{chartPoint.Model.Value} Frequency:\xa0{chartPoint.Model.Second}",
                  }
            };
        }

        static List<DataModel> GetListOfTimePoints(double[] inputSignal, double samplingRate)
        {

            List<DataModel> listOfTimePoints = new();

            for (int i = 0; i < inputSignal.Length; i++)
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
                        GeometrySize = 0.2,
                        Stroke = new SolidColorPaint(SKColors.DarkOliveGreen, 1),
                        LineSmoothness = 0,
                        //Name="Val: ",
                       YToolTipLabelFormatter=(chartPoint) => $"Val:\xa0{chartPoint.Model.Value} Time:\xa0{chartPoint.Model.Second}",

                  }
            };
        }

        public static List<ISeries> GetBins(double[] lmSpectrum)
        {

            var listOfFrequecyPoints = GetListOfBins(lmSpectrum);

            return new List<ISeries>{
                  new ColumnSeries<DataModel>()
                  {
                        Values = listOfFrequecyPoints,
                        Fill = null,
                        //Stroke = new SolidColorPaintTask(SKColors.DarkOliveGreen, 1),
                        Stroke = new SolidColorPaint(SKColors.DarkOliveGreen, 1),
                        MaxBarWidth = 1,
                       // Name="Val:",
                        //Mapping=(model, i)=>{ }
                        YToolTipLabelFormatter=(chartPoint) => $"Val:\xa0{chartPoint.Model.Value} Bin:\xa0{chartPoint.Model.Second}",
                  }
            };
        }

        static List<DataModel> GetListOfBins(double[] inputSignal)
        {

            List<DataModel> listOfTimePoints = new();

            for (int i = 0; i < inputSignal.Length; i++)
            {
                listOfTimePoints.Add(

                    new DataModel { Second = i, Value = inputSignal[i] }

                    );
            }

            return listOfTimePoints;
        }
    }
}
