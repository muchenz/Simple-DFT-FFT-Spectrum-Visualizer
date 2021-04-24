/*
 * @(#)BooleanAlgebra.cs        4.3.0   2018-12-12
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
	 * BooleanAlgebra - class for boolean operators.
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
	public sealed class BooleanAlgebra {
		/**
		 * False as integer
		 */
		public const int FALSE = 0;
		/**
		 * True as integer
		 */
		public const int TRUE = 1;
		/**
		 * Null as integer
		 */
		public const int NULL = 2;
		/**
		 * False as double
		 */
		public const double F = 0;
		/**
		 * True as double
		 */
		public const double T = 1;
		/**
		 * Null as double
		 */
		public const double N = Double.NaN;
		/**
		 * AND truth table
		 */
		public static readonly double[,] AND_TRUTH_TABLE = {
			/*		  F  T  N
			/* F */	{ F, F, F} ,
			/* T */	{ F, T, N} ,
			/* N */	{ F, N, N}
		};
		/**
		 * NAND truth table
		 */
		public static readonly double[,] NAND_TRUTH_TABLE = {
			/*		  F  T  N
			/* F */	{ T, T, T} ,
			/* T */	{ T, F, N} ,
			/* N */	{ T, N, N}
		};
		/**
		 * OR truth table
		 */
		public static readonly double[,] OR_TRUTH_TABLE = {
			/*		  F  T  N
			/* F */	{ F, T, N} ,
			/* T */	{ T, T, T} ,
			/* N */	{ N, T, N}
		};
		/**
		 * NOR truth table
		 */
		public static readonly double[,] NOR_TRUTH_TABLE = {
			/*		  F  T  N
			/* F */	{ T, F, N} ,
			/* T */	{ F, F, F} ,
			/* N */	{ N, F, N}
		};
		/**
		 * XOR truth table
		 */
		public static readonly double[,] XOR_TRUTH_TABLE = {
			/*		  F  T  N
			/* F */	{ F, T, N} ,
			/* T */	{ T, F, N} ,
			/* N */	{ N, N, N}
		};
		/**
		 * XNOR truth table
		 */
		public static readonly double[,] XNOR_TRUTH_TABLE = {
			/*		  F  T  N
			/* F */	{ T, F, N} ,
			/* T */	{ F, T, N} ,
			/* N */	{ N, N, N}
		};
		/**
		 * IMP truth table
		 */
		public static readonly double[,] IMP_TRUTH_TABLE = {
			/*		  F  T  N
			/* F */	{ T, T, T} ,
			/* T */	{ F, T, N} ,
			/* N */	{ N, T, N}
		};
		/**
		 * CIMP truth table
		 */
		public static readonly double[,] CIMP_TRUTH_TABLE = {
			/*		  F  T  N
			/* F */	{ T, F, N} ,
			/* T */	{ T, T, T} ,
			/* N */	{ T, N, N}
		};
		/**
		 * EQV truth table
		 */
		public static readonly double[,] EQV_TRUTH_TABLE = {
			/*		  F  T  N
			/* F */	{ T, F, N} ,
			/* T */	{ F, T, N} ,
			/* N */	{ N, N, N}
		};
		/**
		 * NIMP truth table
		 */
		public static readonly double[,] NIMP_TRUTH_TABLE = {
			/*		  F  T  N
			/* F */	{ F, F, F} ,
			/* T */	{ T, F, N} ,
			/* N */	{ N, F, N}
		};
		/**
		 * CNIMP truth table
		 */
		public static readonly double[,] CNIMP_TRUTH_TABLE = {
			/*		  F  T  N
			/* F */	{ F, T, N} ,
			/* T */	{ F, F, F} ,
			/* N */	{ F, N, N}
		};
		/**
		 * NOT truth table
		 */
		public static readonly double[] NOT_TRUTH_TABLE = {
				   /* F  T  N	*/
					  T, F, N
		};
		/**
		 * Double to integer boolean translation
		 *
		 * @param      a                   the double number
		 *
		 * @return     If a = Double.NaN return NULL,
		 *             else if a <> 0 return TRUE,
		 *             else return FALSE.
		 */
		public static int double2IntBoolean(double a) {
			if (Double.IsNaN(a))
				return NULL;
			if (BinaryRelations.epsilonComparison) {
				/* Epsilon comparison mode */
				if (MathFunctions.abs(a) > BinaryRelations.epsilon)
					return TRUE;
				else
					return FALSE;
			}
			else {
				/* Exact comparison mode */
				if (a != 0)
					return TRUE;
				else
					return FALSE;
			}
		}
		/**
		 * Boolean AND
		 *
		 * @param      a                   the a number (a AND b)
		 * @param      b                   the b number (a AND b)
		 *
		 * @return     Truth table element AND[A][B] where A = double2IntBoolean(a), B = double2IntBoolean(b)
		 */
		public static double and(double a, double b) {
			int A = double2IntBoolean(a);
			int B = double2IntBoolean(b);
			return AND_TRUTH_TABLE[A, B];
		}
		/**
		 * Boolean OR
		 *
		 * @param      a                   the a number (a OR b)
		 * @param      b                   the b number (a OR b)
		 *
		 * @return     Truth table element OR[A][B]
		 *             where A = double2IntBoolean(a), B = double2IntBoolean(b)
		 */
		public static double or(double a, double b) {
			int A = double2IntBoolean(a);
			int B = double2IntBoolean(b);
			return OR_TRUTH_TABLE[A, B];
		}
		/**
		 * Boolean XOR
		 *
		 * @param      a                   the a number (a XOR b)
		 * @param      b                   the b number (a XOR b)
		 *
		 * @return     Truth table element XOR[A][B]
		 *             where A = double2IntBoolean(a), B = double2IntBoolean(b)
		 */
		public static double xor(double a, double b) {
			int A = double2IntBoolean(a);
			int B = double2IntBoolean(b);
			return XOR_TRUTH_TABLE[A, B];
		}
		/**
		 * Boolean NAND
		 *
		 * @param      a                   the a number (a NAND b)
		 * @param      b                   the b number (a NAND b)
		 *
		 * @return     Truth table element NAND[A][B]
		 *             where A = double2IntBoolean(a), B = double2IntBoolean(b)
		 */
		public static double nand(double a, double b) {
			int A = double2IntBoolean(a);
			int B = double2IntBoolean(b);
			return NAND_TRUTH_TABLE[A, B];
		}
		/**
		 * Boolean NOR
		 *
		 * @param      a                   the a number (a NOR b)
		 * @param      b                   the b number (a NOR b)
		 *
		 * @return     Truth table element NOR[A][B]
		 *             where A = double2IntBoolean(a), B = double2IntBoolean(b)
		 */
		public static double nor(double a, double b) {
			int A = double2IntBoolean(a);
			int B = double2IntBoolean(b);
			return NOR_TRUTH_TABLE[A, B];
		}
		/**
		 * Boolean XNOR
		 *
		 * @param      a                   the a number (a XNOR b)
		 * @param      b                   the b number (a XNOR b)
		 *
		 * @return     Truth table element XNOR[A][B]
		 *             where A = double2IntBoolean(a), B = double2IntBoolean(b)
		 */
		public static double xnor(double a, double b) {
			int A = double2IntBoolean(a);
			int B = double2IntBoolean(b);
			return XNOR_TRUTH_TABLE[A, B];
		}
		/**
		 * Boolean IMP
		 *
		 * @param      a                   the a number (a IMP b)
		 * @param      b                   the b number (a IMP b)
		 *
		 * @return     Truth table element IMP[A][B]
		 *             where A = double2IntBoolean(a), B = double2IntBoolean(b)
		 */
		public static double imp(double a, double b) {
			int A = double2IntBoolean(a);
			int B = double2IntBoolean(b);
			return IMP_TRUTH_TABLE[A, B];
		}
		/**
		 * Boolean EQV
		 *
		 * @param      a                   the a number (a EQV b)
		 * @param      b                   the b number (a EQV b)
		 *
		 * @return     Truth table element EQV[A][B]
		 *             where A = double2IntBoolean(a), B = double2IntBoolean(b)
		 */
		public static double eqv(double a, double b) {
			int A = double2IntBoolean(a);
			int B = double2IntBoolean(b);
			return EQV_TRUTH_TABLE[A, B];
		}
		/**
		 * Boolean NOT
		 *
		 * @param      a                   the a number (NOT a)
		 *
		 * @return     Truth table element NOT[A]
		 *             where A = double2IntBoolean(a)
		 */
		public static double not(double a) {
			int A = double2IntBoolean(a);
			return NOT_TRUTH_TABLE[A];
		}
		/**
		 * Boolean CIMP
		 *
		 * @param      a                   the a number (a CIMP b)
		 * @param      b                   the b number (a CIMP b)
		 *
		 * @return     Truth table element CIMP[A][B]
		 *             where A = double2IntBoolean(a), B = double2IntBoolean(b)
		 */
		public static double cimp(double a, double b) {
			int A = double2IntBoolean(a);
			int B = double2IntBoolean(b);
			return CIMP_TRUTH_TABLE[A, B];
		}
		/**
		 * Boolean NIMP
		 *
		 * @param      a                   the a number (a NIMP b)
		 * @param      b                   the b number (a NIMP b)
		 *
		 * @return     Truth table element NIMP[A][B]
		 *             where A = double2IntBoolean(a), B = double2IntBoolean(b)
		 */
		public static double nimp(double a, double b) {
			int A = double2IntBoolean(a);
			int B = double2IntBoolean(b);
			return NIMP_TRUTH_TABLE[A, B];
		}
		/**
		 * Boolean CNIMP
		 *
		 * @param      a                   the a number (a CNIMP b)
		 * @param      b                   the b number (a CNIMP b)
		 *
		 * @return     Truth table element CNIMP[A][B]
		 *             where A = double2IntBoolean(a), B = double2IntBoolean(b)
		 */
		public static double cnimp(double a, double b)
		{
			int A = double2IntBoolean(a);
			int B = double2IntBoolean(b);
			return CNIMP_TRUTH_TABLE[A, B];
		}
		/**
		 * Boolean AND variadic
		 *
		 * @param values   List of values
		 * @return   Returns BooleanAlgebra.TRUE if all values on the list are BooleanAlgebra.TURE,
		 *           otherwise returns BooleanAlgebra.FALSE
		 */
		public static double andVariadic(params double[] values) {
			if (values == null) return Double.NaN;
			if (values.Length == 0) return Double.NaN;
			int cntTrue = 0;
			int bv;
			foreach (double v in values) {
				bv = double2IntBoolean(v);
				if (bv == FALSE) return FALSE;
				if (bv == TRUE) cntTrue++;
				if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
			}
			if (cntTrue == values.Length) return TRUE;
			else return Double.NaN;
		}
		/**
		 * Boolean OR variadic
		 *
		 * @param values   List of values
		 * @return   Returns BooleanAlgebra.TRUE if at least one value on the list is BooleanAlgebra.TURE,
		 *           otherwise returns BooleanAlgebra.FALSE
		 */
		public static double orVariadic(params double[] values) {
			if (values == null) return Double.NaN;
			if (values.Length == 0) return Double.NaN;
			int cntFalse = 0;
			int bv;
			foreach (double v in values) {
				bv = double2IntBoolean(v);
				if (bv == TRUE) return TRUE;
				if (bv == FALSE) cntFalse++;
				if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
			}
			if (cntFalse == values.Length) return FALSE;
			else return Double.NaN;
		}
		/**
		 * Boolean XOR variadic
		 *
		 * @param values   List of values
		 * @return   Returns BooleanAlgebra.TRUE if exactly one value on the list is BooleanAlgebra.TURE,
		 *           otherwise returns BooleanAlgebra.FALSE
		 */
		public static double xorVariadic(params double[] values) {
			if (values == null) return Double.NaN;
			if (values.Length == 0) return Double.NaN;
			int cntTrue = 0;
			int bv;
			foreach (double v in values) {
				bv = double2IntBoolean(v);
				if (bv == TRUE) {
					cntTrue++;
					if (cntTrue > 1) return FALSE;
				}
				if (bv == NULL) return Double.NaN;
				if (mXparser.isCurrentCalculationCancelled()) return Double.NaN;
			}
			if (cntTrue == 1) return TRUE;
			else return FALSE;
		}
	}
}