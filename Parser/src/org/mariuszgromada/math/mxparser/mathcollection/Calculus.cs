/*
 * @(#)Calculus.cs        4.3.0   2018-12-12
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
	 * Calculus - numerical integration, differentiation, etc...
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
	public sealed class Calculus {
		/**
		 * Derivative type specification
		 */
		public const int LEFT_DERIVATIVE = 1;
		public const int RIGHT_DERIVATIVE = 2;
		public const int GENERAL_DERIVATIVE = 3;
		/**
		 * Trapezoid numerical integration
		 *
		 * @param      f                   the expression
		 * @param      x                   the argument
		 * @param      a                   form a ...
		 * @param      b                   ... to b
		 * @param      eps                 the epsilon (error)
		 * @param      maxSteps            the maximum number of steps
		 *
		 * @return     Integral value as double.
		 *
		 * @see        Expression
		 */
		public static double integralTrapezoid(Expression f, Argument x, double a, double b,
				double eps, int maxSteps) {
			double h = 0.5 * (b-a);
			double fa = mXparser.getFunctionValue(f, x, a);
			double fb = mXparser.getFunctionValue(f, x, b);
			double fah = mXparser.getFunctionValue(f, x, a + h);
			double ft;
			double s = fa + fb + 2 * fah;
			double intF = s * h * 0.5;
			double intFprev = 0;
			double t = a;
			int i, j;
			int n = 1;
			for (i = 1; i <= maxSteps; i++) {
				n += n;
				t = a + 0.5*h;
				intFprev = intF;
				for (j = 1; j <= n; j++) {
					if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
					ft = mXparser.getFunctionValue(f, x, t);
					s += 2 * ft;
					t += h;
				}
				h *= 0.5;
				intF = s * h * 0.5;
				if (Math.Abs(intF - intFprev) <= eps)
					return intF;
				if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
			}
			return intF;
		}
		/**
		 * Numerical derivative at x = x0
		 *
		 * @param      f                   the expression
		 * @param      x                   the argument
		 * @param      x0                  at point x = x0
		 * @param      derType             derivative type (LEFT_DERIVATIVE, RIGHT_DERIVATIVE,
		 *                                 GENERAL_DERIVATIVE
		 * @param      eps                 the epsilon (error)
		 * @param      maxSteps            the maximum number of steps
		 *
		 * @return     Derivative value as double.
		 *
		 * @see        Expression
		 */
		public static double derivative(Expression f, Argument x, double x0,
				int derType, double eps, int maxSteps) {
			const double START_DX = 0.1;
			int step = 0;
			double error = 2.0*eps;
			double y0 = 0.0;
			double derF = 0.0;
			double derFprev = 0.0;
			double dx = 0.0;
			if (derType == LEFT_DERIVATIVE)
				dx = -START_DX;
			else
				dx = START_DX;
			double dy = 0.0;
			if ( (derType == LEFT_DERIVATIVE) || (derType == RIGHT_DERIVATIVE) ) {
				y0 = mXparser.getFunctionValue(f, x, x0);
				dy = mXparser.getFunctionValue(f, x, x0+dx) - y0;
				derF = dy/dx;
			} else
				derF = ( mXparser.getFunctionValue(f, x, x0+dx) - mXparser.getFunctionValue(f, x, x0-dx) ) / (2.0*dx);
			do {
				derFprev = derF;
				dx = dx/2.0;
				if ( (derType == LEFT_DERIVATIVE) || (derType == RIGHT_DERIVATIVE) ) {
					dy = mXparser.getFunctionValue(f, x, x0+dx) - y0;
					derF = dy/dx;
				} else
					derF = ( mXparser.getFunctionValue(f, x, x0+dx) - mXparser.getFunctionValue(f, x, x0-dx) ) / (2.0*dx);
				error = Math.Abs(derF - derFprev);
				step++;
				if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
			} while ( (step < maxSteps) && ( (error > eps) || Double.IsNaN(derF) ));
			return derF;
		}
		/**
		 * Numerical n-th derivative at x = x0 (you should avoid calculation
		 * of derivatives with order higher than 2).
		 *
		 * @param      f                   the expression
		 * @param      n                   the deriviative order
		 * @param      x                   the argument
		 * @param      x0                  at point x = x0
		 * @param      derType             derivative type (LEFT_DERIVATIVE, RIGHT_DERIVATIVE,
		 *                                 GENERAL_DERIVATIVE
		 * @param      eps                 the epsilon (error)
		 * @param      maxSteps            the maximum number of steps
		 *
		 * @return     Derivative value as double.
		 *
		 * @see        Expression
		 */
		public static double derivativeNth(Expression f, double n, Argument x,
				double x0, int derType, double eps, int maxSteps) {
			n = Math.Round(n);
			int step = 0;
			double error = 2*eps;
			double derFprev = 0;
			double dx = 0.01;
			double derF = 0;
			if (derType == RIGHT_DERIVATIVE)
				for (int i = 1; i <= n; i++)
					derF += MathFunctions.binomCoeff(-1,n-i) * MathFunctions.binomCoeff(n,i) * mXparser.getFunctionValue(f,x,x0+i*dx);
			else
				for (int i = 1; i <= n; i++)
					derF += MathFunctions.binomCoeff(-1,i)*MathFunctions.binomCoeff(n,i) * mXparser.getFunctionValue(f,x,x0-i*dx);
			derF = derF / Math.Pow(dx, n);
			do {
				derFprev = derF;
				dx = dx/2.0;
				derF = 0;
				if (derType == RIGHT_DERIVATIVE)
					for (int i = 1; i <= n; i++)
						derF += MathFunctions.binomCoeff(-1,n-i) * MathFunctions.binomCoeff(n,i) * mXparser.getFunctionValue(f,x,x0+i*dx);
				else
					for (int i = 1; i <= n; i++)
						derF += MathFunctions.binomCoeff(-1,i)*MathFunctions.binomCoeff(n,i) * mXparser.getFunctionValue(f,x,x0-i*dx);
				derF = derF / Math.Pow(dx, n);
				error = Math.Abs(derF - derFprev);
				step++;
				if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
			} while ( (step < maxSteps) && ( (error > eps) || Double.IsNaN(derF) ));
			return derF;
		}
		/**
		 * Forward difference(1) operator (at x = x0)
		 *
		 * @param      f                   the expression
		 * @param      x                   the argument name
		 * @param      x0                  x = x0
		 *
		 * @return     Forward difference(1) value calculated at x0.
		 *
		 * @see        Expression
		 * @see        Argument
		 */
		public static double forwardDifference(Expression f, Argument x, double x0) {
			if (Double.IsNaN(x0))
				return Double.NaN;
			double xb = x.getArgumentValue();
			double delta = mXparser.getFunctionValue(f, x, x0 + 1) - mXparser.getFunctionValue(f, x, x0);
			x.setArgumentValue(xb);
			return delta;
		}
		/**
		 * Forward difference(1) operator (at current value of argument x)
		 *
		 * @param      f                   the expression
		 * @param      x                   the argument name
		 *
		 * @return     Forward difference(1) value calculated at the current value of argument x.
		 *
		 * @see        Expression
		 * @see        Argument
		 */
		public static double forwardDifference(Expression f, Argument x) {
			double xb = x.getArgumentValue();
			if (Double.IsNaN(xb))
				return Double.NaN;
			double fv = f.calculate();
			x.setArgumentValue(xb + 1);
			double delta = f.calculate() - fv;
			x.setArgumentValue(xb);
			return delta;
		}
		/**
		 * Backward difference(1) operator (at x = x0).
		 *
		 * @param      f                   the expression
		 * @param      x                   the argument name
		 * @param      x0                  x = x0
		 *
		 * @return     Backward difference value calculated at x0.
		 *
		 * @see        Expression
		 * @see        Argument
		 */
		public static double backwardDifference(Expression f, Argument x, double x0) {
			if (Double.IsNaN(x0))
				return Double.NaN;
			double xb = x.getArgumentValue();
			double delta = mXparser.getFunctionValue(f, x, x0) - mXparser.getFunctionValue(f, x, x0 - 1);
			x.setArgumentValue(xb);
			return delta;
		}
		/**
		 * Backward difference(1) operator (at current value of argument x)
		 *
		 * @param      f                   the expression
		 * @param      x                   the argument name
		 *
		 * @return     Backward difference(1) value calculated at the current value of argument x.
		 *
		 * @see        Expression
		 * @see        Argument
		 */
		public static double backwardDifference(Expression f, Argument x) {
			double xb = x.getArgumentValue();
			if (Double.IsNaN(xb))
				return Double.NaN;
			double fv = f.calculate();
			x.setArgumentValue(xb - 1);
			double delta = fv - f.calculate();
			x.setArgumentValue(xb);
			return delta;
		}
		/**
		 * Forward difference(h) operator (at x = x0)
		 *
		 * @param      f                   the expression
		 * @param      h                   the difference
		 * @param      x                   the argument name
		 * @param      x0                  x = x0
		 *
		 * @return     Forward difference(h) value calculated at x0.
		 *
		 * @see        Expression
		 * @see        Argument
		 */
		public static double forwardDifference(Expression f, double h, Argument x, double x0) {
			if (Double.IsNaN(x0))
				return Double.NaN;
			double xb = x.getArgumentValue();
			double delta = mXparser.getFunctionValue(f, x, x0 + h) - mXparser.getFunctionValue(f, x, x0);
			x.setArgumentValue(xb);
			return delta;
		}
		/**
		 * Forward difference(h) operator (at the current value of the argument x)
		 *
		 * @param      f                   the expression
		 * @param      h                   the difference
		 * @param      x                   the argument name
		 *
		 * @return     Forward difference(h) value calculated at at the current value of the argument x.
		 *
		 * @see        Expression
		 * @see        Argument
		 */
		public static double forwardDifference(Expression f, double h, Argument x) {
			double xb = x.getArgumentValue();
			if (Double.IsNaN(xb))
				return Double.NaN;
			double fv = f.calculate();
			x.setArgumentValue(xb + h);
			double delta = f.calculate() - fv;
			x.setArgumentValue(xb);
			return delta;
		}
		/**
		 * Backward difference(h) operator (at x = x0)
		 *
		 * @param      f                   the expression
		 * @param      h                   the difference
		 * @param      x                   the argument name
		 * @param      x0                  x = x0
		 *
		 * @return     Backward difference(h) value calculated at x0.
		 *
		 * @see        Expression
		 * @see        Argument
		 */
		public static double backwardDifference(Expression f, double h, Argument x, double x0) {
			if (Double.IsNaN(x0))
				return Double.NaN;
			double xb = x.getArgumentValue();
			double delta = mXparser.getFunctionValue(f, x, x0) - mXparser.getFunctionValue(f, x, x0 - h);
			x.setArgumentValue(xb);
			return delta;
		}
		/**
		 * Backward difference(h) operator (at the current value of the argument x)
		 *
		 * @param      f                   the expression
		 * @param      h                   the difference
		 * @param      x                   the argument name
		 *
		 * @return     Backward difference(h) value calculated at at the current value of the argument x.
		 *
		 * @see        Expression
		 * @see        Argument
		 */
		public static double backwardDifference(Expression f, double h, Argument x) {
			double xb = x.getArgumentValue();
			if (Double.IsNaN(xb))
				return Double.NaN;
			double fv = f.calculate();
			x.setArgumentValue(xb - h);
			double delta = fv - f.calculate();
			x.setArgumentValue(xb);
			return delta;
		}
		/**
		 * Brent solver (Brent root finder)
		 *
		 * @param f  Function given in the Expression form
		 * @param x  Argument
		 * @param a  Left limit
		 * @param b  Right limit
		 * @return   Function root - if found, otherwise Double.NaN.
		 */
		public static double solveBrent(Expression f, Argument x, double a, double b, double eps, double maxSteps) {
			double fa, fb, fc, fs, c, c0, c1, c2;
			double tmp, d, s;
			bool mflag;
			int iter;
			/*
			 * If b lower than b then swap
			 */
			if (b < a) {
				tmp = a;
				a = b;
				b = tmp;
			}
			fa = mXparser.getFunctionValue(f, x, a);
			fb = mXparser.getFunctionValue(f, x, b);
			/*
			 * If already root then no need to solve
			 */
			if (MathFunctions.abs(fa) <= eps) return a;
			if (MathFunctions.abs(fb) <= eps) return b;
			if (b == a) return Double.NaN;
			/*
			 * If root not bracketed the perform random search
			 */
			if (fa * fb > 0) {
				bool rndflag = false;
				double ap, bp;
				for (int i = 0; i < maxSteps; i++) {
					ap = ProbabilityDistributions.rndUniformContinuous(a, b);
					bp = ProbabilityDistributions.rndUniformContinuous(a, b);
					if (bp < ap) {
						tmp = ap;
						ap = bp;
						bp = tmp;
					}
					fa = mXparser.getFunctionValue(f, x, ap);
					fb = mXparser.getFunctionValue(f, x, bp);
					if (MathFunctions.abs(fa) <= eps) return ap;
					if (MathFunctions.abs(fb) <= eps) return bp;
					if (fa * fb < 0) {
						rndflag = true;
						a = ap;
						b = bp;
						break;
					}
					if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
				}
				if (rndflag == false) return Double.NaN;
			}
			c = a;
			d = c;
			fc = mXparser.getFunctionValue(f, x, c);
			if (MathFunctions.abs(fa) < MathFunctions.abs(fb)) {
				tmp = a;
				a = b;
				b = tmp;
				tmp = fa;
				fa = fb;
				fb = tmp;
			}
			mflag = true;
			iter = 0;
			/*
			 * Perform actual Brent algorithm
			 */
			while ((MathFunctions.abs(fb) > eps) && (MathFunctions.abs(b - a) > eps) && (iter < maxSteps)) {
				if ((fa != fc) && (fb != fc)) {
					c0 = (a * fb * fc) / ((fa - fb) * (fa - fc));
					c1 = (b * fa * fc) / ((fb - fa) * (fb - fc));
					c2 = (c * fa * fb) / ((fc - fa) * (fc - fb));
					s = c0 + c1 + c2;
				} else
					s = b - (fb * (b - a)) / (fb - fa);
				if ((s < (3 * (a + b) / 4) || s > b) ||
						((mflag == true) && MathFunctions.abs(s - b) >= (MathFunctions.abs(b - c) / 2)) ||
						((mflag == false) && MathFunctions.abs(s - b) >= (MathFunctions.abs(c - d) / 2))) {
					s = (a + b) / 2;
					mflag = true;
				} else
					mflag = true;
				fs = mXparser.getFunctionValue(f, x, s);
				d = c;
				c = b;
				fc = fb;
				if ((fa * fs) < 0)
					b = s;
				else
					a = s;
				if (MathFunctions.abs(fa) < MathFunctions.abs(fb)) {
					tmp = a;
					a = b;
					b = tmp;
					tmp = fa;
					fa = fb;
					fb = tmp;
				}
				iter++;
				if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
			}
			return MathFunctions.round(b, MathFunctions.decimalDigitsBefore(eps) - 1);
		}
	}
}