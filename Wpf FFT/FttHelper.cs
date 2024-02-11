using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf_FFT.MVVM;

namespace Wpf_FFT
{
    public  class FttHelper
    {
        public static  void GetExpressionFromString(string FunctionToFFT, out Argument time, out Argument frequency,
            out Expression f)
        {
            time = new("t", 1);
            frequency = new("f", 2);
            f = new(FunctionToFFT, time, frequency);
        }


        public static double[] GetInputData(double frequency, uint length, double samplingRate, Argument timeArg, 
            Argument frequencyArg, Expression expression)
        {
            return DSP.Generate.ToneSampling2((time, freq) =>
            {

                timeArg.setArgumentValue(time);
                frequencyArg.setArgumentValue(freq);


                var result = expression.calculate();


                if (double.IsNaN(result) || double.IsInfinity(result))
                {
                    throw new CalculateException("During calculate value 'NaN' or 'Infinite' occurs");
                }
                return result;


            }, frequency, samplingRate, length);
        }
    }
}
