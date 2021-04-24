/*
 * @(#)Constant.cs        4.3.0    2019-01-18
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
using org.mariuszgromada.math.mxparser.parsertokens;
using System;
using System.Collections.Generic;

namespace org.mariuszgromada.math.mxparser {
	/**
	 * Constant class provides ability to declare constants.
	 * Constants can be used in further processing by any expression,
	 * dependent or recursive argument, function, etc...
	 * <p>
	 * When creating a constant you should avoid names reserved as
	 * parser keywords, in general words known in mathematical language
	 * as function names, operators (for example:
	 * sin, cos, +, -, pi, e, etc...). Please be informed that after associating
	 * the constant with the expression, function or dependent/recursive argument
	 * its name will be recognized by the parser as reserved key word.
	 * It means that it could not be the same as any other key word known
	 * by the parser for this particular expression.
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
	 *
	 * @see RecursiveArgument
	 * @see Expression
	 * @see Function
	 * @see Argument
	 *
	 */
	[CLSCompliant(true)]
	public class Constant : PrimitiveElement {
		/**
		 * When constant could not be found
		 */
		public const int NOT_FOUND = Expression.NOT_FOUND;
		/**
		 * Type identifier for constants
		 */
		public const int TYPE_ID			= 104;
		public const String TYPE_DESC		= "User defined constant";
		/**
		 * Status of the Expression syntax
		 */
		public const bool NO_SYNTAX_ERRORS = Expression.NO_SYNTAX_ERRORS;
		public const bool SYNTAX_ERROR_OR_STATUS_UNKNOWN = Expression.SYNTAX_ERROR_OR_STATUS_UNKNOWN;
		private const String NO_SYNTAX_ERROR_MSG = "Constant - no syntax errors.";
		/**
		 * Name of the constant
		 */
		private String constantName;
		/**
		 * COnstant value
		 */
		private double constantValue;
		/**
		 * Constant description
		 */
		private String description;
		/**
		 * Dependent expression list
		 */
		private List<Expression> relatedExpressionsList;
		/**
		 * Status of the expression syntax
		 *
		 * Please referet to the:
		 *    - NO_SYNTAX_ERRORS
		 *    - SYNTAX_ERROR_OR_STATUS_UNKNOWN
		 */
		private bool syntaxStatus;
		/**
		 * Message after checking the syntax
		 */
		private String errorMessage;
		/**
		 * Constructor - creates constant with a given name and given value
		 *
		 *
		 * @param      constantName        the constant name
		 * @param      constantValue       the constant value
		 */
		public Constant(String constantName, double constantValue) : base(Constant.TYPE_ID) {
			relatedExpressionsList = new List<Expression>();
			if (mXparser.regexMatch(constantName, ParserSymbol.nameOnlyTokenOptBracketsRegExp)) {
				this.constantName = constantName;
				this.constantValue = constantValue;
				description = "";
				syntaxStatus = NO_SYNTAX_ERRORS;
				errorMessage = NO_SYNTAX_ERROR_MSG;
			} else {
				syntaxStatus = SYNTAX_ERROR_OR_STATUS_UNKNOWN;
				errorMessage = "[" + constantName + "] " + "--> invalid constant name, pattern not mathes: " + ParserSymbol.nameTokenRegExp; ;
			}
		}
		/**
		 * Constructor - creates constant with a given name and given value.
		 * Additionally description is being set.
		 *
		 * @param      constantName        the constant name
		 * @param      constantValue       the constant value
		 * @param      description         the constant description
		 */
		public Constant(String constantName, double constantValue, String description) : base(Constant.TYPE_ID) {
			relatedExpressionsList = new List<Expression>();
			if (mXparser.regexMatch(constantName, ParserSymbol.nameOnlyTokenOptBracketsRegExp)) {
				this.constantName = constantName;
				this.constantValue = constantValue;
				this.description = description;
				syntaxStatus = NO_SYNTAX_ERRORS;
				errorMessage = NO_SYNTAX_ERROR_MSG;
			}
			else {
				syntaxStatus = SYNTAX_ERROR_OR_STATUS_UNKNOWN;
				errorMessage = "[" + constantName + "] " + "--> invalid constant name, pattern not mathes: " + ParserSymbol.nameTokenRegExp; ;
			}
		}
		/**
		 * Constructor for function definition in natural math language,
		 * for instance providing on string "f(x,y) = sin(x) + cos(x)"
		 * is enough to define function "f" with parameters "x and y"
		 * and function body "sin(x) + cos(x)".
		 *
		 * @param constantDefinitionString      Constant definition in the form
		 *                                      of one String, ie "c = 2" or "c = 2*sin(pi/3)"
		 * @param      elements   Optional parameters (comma separated) such as Arguments, Constants, Functions
		 */
		public Constant(String constantDefinitionString, params PrimitiveElement[] elements) : base(Constant.TYPE_ID) {
			description = "";
			syntaxStatus = SYNTAX_ERROR_OR_STATUS_UNKNOWN;
			relatedExpressionsList = new List<Expression>();
			if (mXparser.regexMatch(constantDefinitionString, ParserSymbol.constUnitgDefStrRegExp))
			{
				HeadEqBody headEqBody = new HeadEqBody(constantDefinitionString);
				constantName = headEqBody.headTokens[0].tokenStr;
				Expression bodyExpression = new Expression(headEqBody.bodyStr, elements);
				constantValue = bodyExpression.calculate();
				syntaxStatus = bodyExpression.getSyntaxStatus();
				errorMessage = bodyExpression.getErrorMessage();
			}
			else errorMessage = "[" + constantDefinitionString + "] " + "--> pattern not mathes: " + ParserSymbol.constArgDefStrRegExp;
		}
		/**
		 * Gets constant name
		 *
		 * @return     the constant name as string.
		 */
		public String getConstantName() {
			return constantName;
		}
		/**
		 * Sets constant name. If constant is associated with any expression
		 * then this operation will set modified flag to each related expression.
		 *
		 * @param      constantName        the constant name
		 */
		public void setConstantName(String constantName) {
			if (mXparser.regexMatch(constantName, ParserSymbol.nameOnlyTokenOptBracketsRegExp)) {
				this.constantName = constantName;
				setExpressionModifiedFlags();
			}
			else {
				syntaxStatus = SYNTAX_ERROR_OR_STATUS_UNKNOWN;
				errorMessage = "[" + constantName + "] " + "--> invalid constant name, pattern not mathes: " + ParserSymbol.nameTokenRegExp; ;
			}
		}
		/**
		 * Sets constant value
		 * @param value   constant value
		 */
		public void setConstantValue(double constantValue) {
			this.constantValue = constantValue;
		}
		/**
		 * Gets constant value.
		 *
		 * @return     constant value as double
		 */
		public double getConstantValue() {
			return constantValue;
		}
		/**
		 * Gets constant description.
		 *
		 * @return     constant description as string.
		 */
		public String getDescription() {
			return description;
		}
		/**
		 * Sets constant description.
		 *
		 * @param      description         the constant description
		 */
		public void setDescription(String description) {
			this.description = description;
		}
		/**
		 * Method return error message after
		 *
		 * @return     Error message as string.
		 */
		public String getErrorMessage() {
			return errorMessage;
		}
		/**
		 * Gets syntax status of the expression.
		 *
		 * @return     Constant.NO_SYNTAX_ERRORS if there are no syntax errors,
		 *             ConstantValue.SYNTAX_ERROR_OR_STATUS_UNKNOWN when syntax error was found or
		 *             syntax status is unknown
		 */
		public bool getSyntaxStatus() {
			return this.syntaxStatus;
		}
		/**
		 * Adds related expression.
		 *
		 * @param      expression          the related expression.
		 */
		internal void addRelatedExpression(Expression expression) {
			if (expression != null)
				if ( !relatedExpressionsList.Contains(expression) )
					relatedExpressionsList.Add(expression);
		}
		/**
		 * Removes related expression.
		 *
		 * @param      expression          the related expression.
		 */
		internal void removeRelatedExpression(Expression expression) {
			if (expression != null)
				relatedExpressionsList.Remove(expression);
		}
		/**
		 * Sets expression modified flag to each related expression.
		 */
		void setExpressionModifiedFlags() {
			foreach (Expression e in relatedExpressionsList)
				e.setExpressionModifiedFlag();
		}
	}
}