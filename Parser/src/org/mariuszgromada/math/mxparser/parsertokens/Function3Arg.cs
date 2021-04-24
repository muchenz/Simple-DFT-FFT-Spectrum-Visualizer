/*
 * @(#)Function3Arg.cs        4.2.0    2018-05-29
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

namespace org.mariuszgromada.math.mxparser.parsertokens {
	/**
	 * Functions with 3 arguments - mXparser tokens definition.
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
	 * @version        4.2.0
	 */
	[CLSCompliant(true)]
	public sealed class Function3Arg {
		/*
		 * 3-args Function - token type id.
		 */
		public const int TYPE_ID						= 6;
		public const String TYPE_DESC					= "3-args Function";
		/*
		 * 3-args Function - tokens id.
		 */
		public const int IF_CONDITION_ID				= 1;
		public const int IF_ID							= 2;
		public const int CHI_ID							= 3;
		public const int CHI_LR_ID						= 4;
		public const int CHI_L_ID						= 5;
		public const int CHI_R_ID						= 6;
		public const int PDF_UNIFORM_CONT_ID			= 7;
		public const int CDF_UNIFORM_CONT_ID			= 8;
		public const int QNT_UNIFORM_CONT_ID			= 9;
		public const int PDF_NORMAL_ID					= 10;
		public const int CDF_NORMAL_ID					= 11;
		public const int QNT_NORMAL_ID					= 12;
		public const int DIGIT_ID						= 13;
		public const int INC_BETA_ID					= 14;
		public const int REG_BETA_ID					= 15;
		/*
		 * 3-args Function - tokens key words.
		 */
		public const String IF_STR 						= "if";
		public const String CHI_STR						= "chi";
		public const String CHI_LR_STR					= "CHi";
		public const String CHI_L_STR					= "Chi";
		public const String CHI_R_STR					= "cHi";
		public const String PDF_UNIFORM_CONT_STR		= "pUni";
		public const String CDF_UNIFORM_CONT_STR		= "cUni";
		public const String QNT_UNIFORM_CONT_STR		= "qUni";
		public const String PDF_NORMAL_STR				= "pNor";
		public const String CDF_NORMAL_STR				= "cNor";
		public const String QNT_NORMAL_STR				= "qNor";
		public const String DIGIT_STR					= "dig";
		public const String INC_BETA_STR				= "BetaInc";
		public const String REG_BETA_STR				= "BetaReg";
		public const String REG_BETA_I_STR				= "BetaI";
		/*
		 * 3-args Function - syntax.
		 */
		public const String IF_SYN 						= "if( cond, expr-if-true, expr-if-false )";
		public const String CHI_SYN						= "chi(x, a, b)";
		public const String CHI_LR_SYN					= "CHi(x, a, b)";
		public const String CHI_L_SYN					= "Chi(x, a, b)";
		public const String CHI_R_SYN					= "cHi(x, a, b)";
		public const String PDF_UNIFORM_CONT_SYN		= "pUni(x, a, b)";
		public const String CDF_UNIFORM_CONT_SYN		= "cUni(x, a, b)";
		public const String QNT_UNIFORM_CONT_SYN		= "qUni(q, a, b)";
		public const String PDF_NORMAL_SYN				= "pNor(x, mean, stdv)";
		public const String CDF_NORMAL_SYN				= "cNor(x, mean, stdv)";
		public const String QNT_NORMAL_SYN				= "qNor(q, mean, stdv)";
		public const String DIGIT_SYN					= "dig(num, pos, base)";
		public const String INC_BETA_SYN				= INC_BETA_STR + "(x,a,b)";
		public const String REG_BETA_SYN				= REG_BETA_STR + "(x,a,b)";
		public const String REG_BETA_I_SYN				= REG_BETA_I_STR + "(x,a,b)";
		/*
		 * 3-args Function - tokens description.
		 */
		public const String IF_DESC 					= "If function";
		public const String CHI_DESC					= "Characteristic function for x in (a,b)";
		public const String CHI_LR_DESC					= "Characteristic function for x in [a,b]";
		public const String CHI_L_DESC					= "Characteristic function for x in [a,b)";
		public const String CHI_R_DESC					= "Characteristic function for x in (a,b]";
		public const String PDF_UNIFORM_CONT_DESC		= "Probability distribution function - Uniform continuous distribution U(a,b)";
		public const String CDF_UNIFORM_CONT_DESC		= "Cumulative distribution function - Uniform continuous distribution U(a,b)";
		public const String QNT_UNIFORM_CONT_DESC		= "Quantile function (inverse cumulative distribution function) - Uniform continuous distribution U(a,b)";
		public const String PDF_NORMAL_DESC				= "Probability distribution function - Normal distribution N(m,s)";
		public const String CDF_NORMAL_DESC				= "Cumulative distribution function - Normal distribution N(m,s)";
		public const String QNT_NORMAL_DESC				= "Quantile function (inverse cumulative distribution function)";
		public const String DIGIT_DESC					= "Digit at position 1 ... n (left -> right) or 0 ... -(n-1) (right -> left) - numeral system with given base";
		public const String INC_BETA_DESC				= "The incomplete beta special function B(x; a, b), also called the incomplete Euler integral of the first kind";
		public const String REG_BETA_DESC				= "The regularized incomplete beta (or regularized beta) special function I(x; a, b), also called the regularized incomplete Euler integral of the first kind";
		/*
		 * 3-args Function - since.
		 */
		public const String IF_SINCE 					= mXparser.NAMEv10;
		public const String CHI_SINCE					= mXparser.NAMEv10;
		public const String CHI_LR_SINCE				= mXparser.NAMEv10;
		public const String CHI_L_SINCE					= mXparser.NAMEv10;
		public const String CHI_R_SINCE					= mXparser.NAMEv10;
		public const String PDF_UNIFORM_CONT_SINCE		= mXparser.NAMEv30;
		public const String CDF_UNIFORM_CONT_SINCE		= mXparser.NAMEv30;
		public const String QNT_UNIFORM_CONT_SINCE		= mXparser.NAMEv30;
		public const String PDF_NORMAL_SINCE			= mXparser.NAMEv30;
		public const String CDF_NORMAL_SINCE			= mXparser.NAMEv30;
		public const String QNT_NORMAL_SINCE			= mXparser.NAMEv30;
		public const String DIGIT_SINCE					= mXparser.NAMEv41;
		public const String INC_BETA_SINCE				= mXparser.NAMEv42;
		public const String REG_BETA_SINCE				= mXparser.NAMEv42;
		public const String REG_BETA_I_SINCE			= mXparser.NAMEv42;
	}
}
