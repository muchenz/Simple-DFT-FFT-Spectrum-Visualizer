/*
 * @(#)Statistics.cs        4.3.0   2018-12-12
 *
 * You may use this software under the condition of "Simplified BSD License"
 *
 * Copyright 2010-2019 MARIUSZ GROMADA. All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 *
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 *
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY <MARIUSZ GROMADA> ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of MARIUSZ GROMADA.
 *
 * If you have any questions/bugs feel free to contact:
 *
 *     Mariusz Gromada
 *     mariuszgromada.org@gmail.com
 *     http://mathparser.org
 *     http://mathspace.pl
 *     http://janetsudoku.mariuszgromada.org
 *     http://github.com/mariuszgromada/MathParser.org-mXparser
 *     http://mariuszgromada.github.io/MathParser.org-mXparser
 *     http://mxparser.sourceforge.net
 *     http://bitbucket.org/mariuszgromada/mxparser
 *     http://mxparser.codeplex.com
 *     http://github.com/mariuszgromada/Janet-Sudoku
 *     http://janetsudoku.codeplex.com
 *     http://sourceforge.net/projects/janetsudoku
 *     http://bitbucket.org/mariuszgromada/janet-sudoku
 *     http://github.com/mariuszgromada/MathParser.org-mXparser
 *     http://scalarmath.org/
 *     https://play.google.com/store/apps/details?id=org.mathparser.scalar.lite
 *     https://play.google.com/store/apps/details?id=org.mathparser.scalar.pro
 *
 *                              Asked if he believes in one God, a mathematician answered:
 *                              "Yes, up to isomorphism."
 */
using System;

namespace org.mariuszgromada.math.mxparser.mathcollection {
	/**
	 * Statistics - i.e.: mean, variance, standard deviation, etc.
	 *
	 * @author         <b>Mariusz Gromada</b><br>
	 *                 <a href="mailto:mariuszgromada.org@gmail.com">mariuszgromada.org@gmail.com</a><br>
	 *                 <a href="http://mathspace.pl" target="_blank">MathSpace.pl</a><br>
	 *                 <a href="http://mathparser.org" target="_blank">MathParser.org - mXparser project page</a><br>
	 *                 <a href="http://github.com/mariuszgromada/MathParser.org-mXparser" target="_blank">mXparser on GitHub</a><br>
	 *                 <a href="http://mxparser.sourceforge.net" target="_blank">mXparser on SourceForge</a><br>
	 *                 <a href="http://bitbucket.org/mariuszgromada/mxparser" target="_blank">mXparser on Bitbucket</a><br>
	 *                 <a href="http://mxparser.codeplex.com" target="_blank">mXparser on CodePlex</a><br>
	 *                 <a href="http://janetsudoku.mariuszgromada.org" target="_blank">Janet Sudoku - project web page</a><br>
	 *                 <a href="http://github.com/mariuszgromada/Janet-Sudoku" target="_blank">Janet Sudoku on GitHub</a><br>
	 *                 <a href="http://janetsudoku.codeplex.com" target="_blank">Janet Sudoku on CodePlex</a><br>
	 *                 <a href="http://sourceforge.net/projects/janetsudoku" target="_blank">Janet Sudoku on SourceForge</a><br>
	 *                 <a href="http://bitbucket.org/mariuszgromada/janet-sudoku" target="_blank">Janet Sudoku on BitBucket</a><br>
	 *                 <a href="https://play.google.com/store/apps/details?id=org.mathparser.scalar.lite" target="_blank">Scalar Free</a><br>
	 *                 <a href="https://play.google.com/store/apps/details?id=org.mathparser.scalar.pro" target="_blank">Scalar Pro</a><br>
	 *                 <a href="http://scalarmath.org/" target="_blank">ScalarMath.org</a><br>
	 *
	 * @version        4.3.0
	 */
	[CLSCompliant(true)]
	public sealed class Statistics {
		/**
		 * Average from sample function values - iterative operator.
		 *
		 * @param      f                   the expression
		 * @param      index               the name of index argument
		 * @param      from                FROM index = form
		 * @param      to                  TO index = to
		 * @param      delta               BY delta
		 *
		 * @return     product operation (for empty product operations returns 1).
		 *
		 * @see        Expression
		 * @see        Argument
		 */
		public static double avg(Expression f, Argument index, double from, double to, double delta) {
			if ((Double.IsNaN(delta)) || (Double.IsNaN(from)) || (Double.IsNaN(to)) || (delta == 0))
				return Double.NaN;
			double sum = 0;
			int n = 0;
			if ( (to >= from) && (delta > 0) ) {
				double i;
				for (i = from; i < to; i+=delta) {
					if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
					sum += mXparser.getFunctionValue(f, index, i);
					n++;
				}
				if ( delta - (i - to) > 0.5 * delta) {
					if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
					sum += mXparser.getFunctionValue(f, index, to);
					n++;
				}
			} else if ( (to <= from) && (delta < 0) ) {
				double i;
				for (i = from; i > to; i+=delta) {
					if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
					sum += mXparser.getFunctionValue(f, index, i);
					n++;
				}
				if ( -delta - (to - i) > -0.5 * delta) {
					if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
					sum += mXparser.getFunctionValue(f, index, to);
					n++;
				}
			} else if (from == to)
				return mXparser.getFunctionValue(f, index, from);
			return sum / n;
		}
		/**
		 * Bias-corrected variance from sample function values - iterative operator.
		 *
		 * @param      f                   the expression
		 * @param      index               the name of index argument
		 * @param      from                FROM index = form
		 * @param      to                  TO index = to
		 * @param      delta               BY delta
		 *
		 * @return     product operation (for empty product operations returns 1).
		 *
		 * @see        Expression
		 * @see        Argument
		 */
		public static double var(Expression f, Argument index, double from, double to, double delta) {
			if ((Double.IsNaN(delta)) || (Double.IsNaN(from)) || (Double.IsNaN(to)) || (delta == 0))
				return Double.NaN;
			return var(mXparser.getFunctionValues(f, index, from, to, delta));
		}
		/**
		 * Bias-corrected standard deviation from sample function values - iterative operator.
		 *
		 * @param      f                   the expression
		 * @param      index               the name of index argument
		 * @param      from                FROM index = form
		 * @param      to                  TO index = to
		 * @param      delta               BY delta
		 *
		 * @return     product operation (for empty product operations returns 1).
		 *
		 * @see        Expression
		 * @see        Argument
		 */
		public static double std(Expression f, Argument index, double from, double to, double delta) {
			if ((Double.IsNaN(delta)) || (Double.IsNaN(from)) || (Double.IsNaN(to)) || (delta == 0))
				return Double.NaN;
			return std(mXparser.getFunctionValues(f, index, from, to, delta));
		}
		/**
		 * Sample average.
		 *
		 * @param      numbers             the numbers
		 *
		 * @return     if each number from numbers <> Double.NaN returns
		 *             avg(a_1,...,a_n) a_1,...,a_n in numbers,
		 *             otherwise returns Double.NaN.
		 */
		public static double avg(params double[] numbers) {
			if (numbers == null) return Double.NaN;
			if (numbers.Length == 0) return Double.NaN;
			if (numbers.Length == 1) return numbers[0];
			double sum = 0;
			foreach (double xi in numbers) {
				if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
				if (Double.IsNaN(xi))
					return Double.NaN;
				sum += xi;
			}
			return sum / numbers.Length;
		}

		/**
		 * Sample variance (biased-corrected).
		 *
		 * @param      numbers             the numbers
		 *
		 * @return     if each number from numbers <> Double.NaN returns
		 *             Var(a_1,...,a_n) a_1,...,a_n in numbers,
		 *             otherwise returns Double.NaN.
		 */
		public static double var(params double[] numbers) {
			if (numbers == null) return Double.NaN;
			if (numbers.Length == 0) return Double.NaN;
			if (numbers.Length == 1) {
				if (Double.IsNaN(numbers[0])) return Double.NaN;
				return 0;
			}
			double m = avg(numbers);
			double sum = 0;
			foreach (double xi in numbers) {
				if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
				if (Double.IsNaN(xi))
					return Double.NaN;
				sum += (xi - m) * (xi - m);
			}
			return sum / (numbers.Length - 1);
		}
		/**
		 * Sample standard deviation (biased-corrected).
		 *
		 * @param      numbers             the numbers
		 *
		 * @return     if each number from numbers <> Double.NaN returns
		 *             Std(a_1,...,a_n) a_1,...,a_n in numbers,
		 *             otherwise returns Double.NaN.
		 */
		public static double std(params double[] numbers) {
			if (numbers == null) return Double.NaN;
			if (numbers.Length == 0) return Double.NaN;
			if (numbers.Length == 1) {
				if (Double.IsNaN(numbers[0])) return Double.NaN;
				return 0;
			}
			return MathFunctions.sqrt(var(numbers));
		}
		/**
		 * Sample median
		 * @param numbers   List of number
		 * @return          Sample median, if table was empty or null then Double.NaN is returned.
		 */
		public static double median(params double[] numbers) {
			if (numbers == null) return Double.NaN;
			if (numbers.Length == 0) return Double.NaN;
			if (numbers.Length == 1) return numbers[0];
			if (numbers.Length == 2) return (numbers[0] + numbers[1]) / 2.0;
			foreach (double v in numbers) {
				if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
				if (Double.IsNaN(v)) return Double.NaN;
			}
			NumberTheory.sortAsc(numbers);
			if ((numbers.Length % 2) == 1) {
				int i = (numbers.Length - 1) / 2;
				return numbers[i];
			}
			else {
				int i = (numbers.Length / 2) - 1;
				return (numbers[i] + numbers[i + 1]) / 2.0;
			}
		}
		/**
		 * Sample mode
		 * @param numbers   List of number
		 * @return          Sample median, if table was empty or null then Double.NaN is returned.
		 */
		public static double mode(params double[] numbers) {
			if (numbers == null) return Double.NaN;
			if (numbers.Length == 0) return Double.NaN;
			if (numbers.Length == 1) return numbers[0];
			foreach (double v in numbers) {
				if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
				if (Double.IsNaN(v)) return Double.NaN;
			}
			double[,] dist = NumberTheory.getDistValues(numbers, true);
			return dist[0, 0];
		}
	}
}