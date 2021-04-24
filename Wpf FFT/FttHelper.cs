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
        public static  void GetExpressionFromString(string FunctionToFFT, out Argument pierwszy, out Argument drugi, out org.mariuszgromada.math.mxparser.Expression f)
        {
            pierwszy = new("t", 1);
            drugi = new("f", 2);
            f = new(FunctionToFFT, pierwszy, drugi);
        }

        public static double[] GetInputData(double frequency, uint length, double samplingRate, Argument timeArg, Argument frequencyArg, org.mariuszgromada.math.mxparser.Expression expression)
        {
            return DSP.Generate.ToneSampling2((t, freq) =>
            {

                timeArg.setArgumentValue(t);
                frequencyArg.setArgumentValue(freq);


                var w = expression.calculate();


                if (double.IsNaN(w) || double.IsInfinity(w))
                {
                    throw new CalculateException("During calculate value 'NaN' or 'Infinite' occurs");
                }
                return w;


            }, frequency, samplingRate, length);
        }
    }
}
