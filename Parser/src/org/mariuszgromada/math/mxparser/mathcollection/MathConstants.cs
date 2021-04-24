/*
 * @(#)MathConstants.cs        4.3.4    2019-12-2
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
	 * MathConstants - class representing the most important math constants.
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
	 * @version        4.3.4
	 */
	[CLSCompliant(true)]
	public sealed class MathConstants {
		/**
		 * Pi, Archimedes' constant or Ludolph's number
		 */
		public const double PI = 3.14159265358979323846264338327950288;
		/**
		 * Pi/2
		 */
		public static readonly double PIBY2 = PI / 2.0;
		/**
		 * Napier's constant, or Euler's number, base of Natural logarithm
		 */
		public const double E = 2.71828182845904523536028747135266249;
		/**
		 * Euler-Mascheroni constant
		 */
		public const double EULER_MASCHERONI = 0.57721566490153286060651209008240243;
		/**
		 * Golden ratio
		 */
		public const double GOLDEN_RATIO = 1.61803398874989484820458683436563811;
		/**
		 * Plastic constant
		 */
		public const double PLASTIC = 1.32471795724474602596090885447809734;
		/**
		 * Embree-Trefethen constant
		 */
		public const double EMBREE_TREFETHEN = 0.70258;
		/**
		 * Feigenbaum constant
		 */
		public const double FEIGENBAUM_DELTA = 4.66920160910299067185320382046620161;
		/**
		 * Feigenbaum constant
		 */
		public const double FEIGENBAUM_ALFA = 2.50290787509589282228390287321821578;
		/**
		 * Feigenbaum constant
		 */
		public const double TWIN_PRIME = 0.66016181584686957392781211001455577;
		/**
		 * Meissel-Mertens constant
		 */
		public const double MEISSEL_MERTEENS = 0.26149721284764278375542683860869585;
		/**
		 * Brun's constant for twin primes
		 */
		public const double BRAUN_TWIN_PRIME = 1.9021605823;
		/**
		 * Brun's constant for prime quadruplets
		 */
		public const double BRAUN_PRIME_QUADR = 0.8705883800;
		/**
		 * de Bruijn-Newman constant
		 */
		public const double BRUIJN_NEWMAN = -2.7E-9;
		/**
		 * Catalan's constant
		 */
		public const double CATALAN = 0.91596559417721901505460351493238411;
		/**
		 * Landau-Ramanujan constant
		 */
		public const double LANDAU_RAMANUJAN = 0.76422365358922066299069873125009232;
		/**
		 * Viswanath's constant
		 */
		public const double VISWANATH = 1.13198824;
		/**
		 * Legendre's constant
		 */
		public const double LEGENDRE = 1.0;
		/**
		 * Ramanujan-Soldner constant
		 */
		public const double RAMANUJAN_SOLDNER = 1.45136923488338105028396848589202744;
		/**
		 * Erdos-Borwein constant
		 */
		public const double ERDOS_BORWEIN = 1.60669515241529176378330152319092458;
		/**
		 * Bernstein's constant
		 */
		public const double BERNSTEIN = 0.28016949902386913303;
		/**
		 * Gauss-Kuzmin-Wirsing constant
		 */
		public const double GAUSS_KUZMIN_WIRSING = 0.30366300289873265859744812190155623;
		/**
		 * Hafner-Sarnak-McCurley constant
		 */
		public const double HAFNER_SARNAK_MCCURLEY = 0.35323637185499598454;
		/**
		 * Golomb-Dickman constant
		 */
		public const double GOLOMB_DICKMAN = 0.62432998854355087099293638310083724;
		/**
		 * Cahen's constant
		 */
		public const double CAHEN = 0.6434105463;
		/**
		 * Laplace limit
		 */
		public const double LAPLACE_LIMIT = 0.66274341934918158097474209710925290;
		/**
		 * Alladi-Grinstead constant
		 */
		public const double ALLADI_GRINSTEAD = 0.8093940205;
		/**
		 * Lengyel's constant
		 */
		public const double LENGYEL = 1.0986858055;
		/**
		 * Levy's constant
		 */
		public const double LEVY = 3.27582291872181115978768188245384386;
		/**
		 * Apery's constant
		 */
		public const double APERY = 1.20205690315959428539973816151144999;
		/**
		 * Mills' constant
		 */
		public const double MILLS = 1.30637788386308069046861449260260571;
		/**
		 * Backhouse's constant
		 */
		public const double BACKHOUSE = 1.45607494858268967139959535111654356;
		/**
		 * Porter's constant
		 */
		public const double PORTER = 1.4670780794;
		/**
		 * Porter's constant
		 */
		public const double LIEB_QUARE_ICE = 1.5396007178;
		/**
		 * Niven's constant
		 */
		public const double NIVEN = 1.70521114010536776428855145343450816;
		/**
		 * Sierpiński's constant
		 */
		public const double SIERPINSKI = 2.58498175957925321706589358738317116;
		/**
		 * Khinchin's constant
		 */
		public const double KHINCHIN = 2.68545200106530644530971483548179569;
		/**
		 * Fransén-Robinson constant
		 */
		public const double FRANSEN_ROBINSON = 2.80777024202851936522150118655777293;
		/**
		 * Landau's constant
		 */
		public const double LANDAU = 0.5;
		/**
		 * Parabolic constant
		 */
		public const double PARABOLIC = 2.29558714939263807403429804918949039;
		/**
		 * Omega constant
		 */
		public const double OMEGA = 0.56714329040978387299996866221035555;
		/**
		 * MRB constant
		 */
		public const double MRB = 0.187859;
		/**
		 * A069284 - Logarithmic integral function li(2)
		 */
		public const double LI2 = 1.045163780117492784844588889194613136522615578151;
		/**
		 * Gompertz Constant OEIS A073003
		 */
		public const double GOMPERTZ = 0.596347362323194074341078499369279376074;
		/**
		 * Square root of 2
		 */
		public static readonly double SQRT2 = Math.Sqrt(2.0);
		/**
		 * Square root of pi
		 */
		public const double SQRTPi = 1.772453850905516027298167483341145182797549456122387128213d;
		/**
		 * Square root of 2*pi
		 */
		public const double SQRT2Pi = 2.5066282746310005024157652848110452530069867406099d;
		/**
		 * Natural logarithm of pi
		 */
		public static readonly double LNPI = MathFunctions.ln(PI);
		/**
		 * Tetration left convergence limit
		 */
		public static readonly double EXP_MINUS_E = Math.Pow(E, -E);
		/**
		 * Tetration right convergence limit
		 */
		public static readonly double EXP_1_OVER_E = Math.Pow(E, 1.0 / E);
		/**
		 * 1 over e
		 */
		public const double EXP_MINUS_1 = 1.0 / Math.E;
		/**
		 * Natural logarithm of sqrt(2)
		 */
		public static readonly double LN_SQRT2 = MathFunctions.ln(SQRT2);
		/**
		 * SQRT2BY2
		 */
		public static readonly double SQRT2BY2 = SQRT2 / 2.0;
		/**
		 * SQRT3
		 */
		public static readonly double SQRT3 = Math.Sqrt(3.0);
		/**
		 * SQRT3BY2
		 */
		public static readonly double SQRT3BY2 = SQRT3 / 2.0;
		/**
		 * D2BYSQRT3
		 */
		public static readonly double D2BYSQRT3 = 2.0 / SQRT3;
		/**
		 * SQRT3BY3
		 */
		public static readonly double SQRT3BY3 = SQRT3 / 3.0;
		/**
		 * Not-a-Number
		 */
		public const double NOT_A_NUMBER = Double.NaN;
	}
}