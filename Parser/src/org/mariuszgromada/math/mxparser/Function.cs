/*
 * @(#)Function.cs        4.3.0   2018-12-12
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
	 * Function class provides possibility to define user functions.
	 * Functions can be used in further processing by any expression,
	 * dependent or recursive argument, function, etc... For expamle:
	 *
	 * <ul>
	 * <li>'f(x) = sin(x)'
	 * <li>'g(x,y) = sin(x)+cos(y)'
	 * <li>'h(x,y = f(x)+g(y,x)'
	 * <li>in general 'f(x1,x2,...,xn)' where x1,...,xn are arguments
	 * </ul>
	 *
	 * <p>
	 * When creating a function you should avoid names reserved as
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
	 * @see Argument
	 * @see Constant
	 * @see FunctionExtension
	 *
	 */
	[CLSCompliant(true)]
	public class Function : PrimitiveElement {
		/**
		 * No syntax errors in the function.
		 */
		public const bool NO_SYNTAX_ERRORS = Expression.NO_SYNTAX_ERRORS;
		/**
		 * Syntax error in the function or syntax status unknown.
		 */
		public const bool SYNTAX_ERROR_OR_STATUS_UNKNOWN = Expression.SYNTAX_ERROR_OR_STATUS_UNKNOWN;
		/**
		 * When function was not found
		 */
		public const int NOT_FOUND = Expression.NOT_FOUND;
		/**
		 * Function type id identifier
		 */
		public const int TYPE_ID			= 103;
		public const String TYPE_DESC		= "User defined function";
		/**
		 * Function with body based on the expression string.
		 *
		 * @see Function#getFunctionBodyType()
		 */
		public const int BODY_RUNTIME = 1;
		/**
		 * Function with body based on the extended code.
		 *
		 * @see FunctionExtension
		 * @see Function#getFunctionBodyType()
		 */
		public const int BODY_EXTENDED = 2;
		/**
		 * Function body type.
		 *
		 * @see Function#BODY_RUNTIME
		 * @see Function#BODY_EXTENDED
		 * @see Function#getFunctionBodyType()
		 */
		private int functionBodyType;
		/**
		 * function expression
		 */
		internal Expression functionExpression;
		/**
		 * function name
		 */
		private String functionName;
		/**
		 * function description
		 */
		private String description;
		/**
		 * Indicates whether UDF is variadic
		 */
		internal bool isVariadic;
		/**
		 * The number of function parameters
		 */
		private int parametersNumber;
		/**
		 * Function extension (body based in code)
		 *
		 * @see FunctionExtension
		 * @see FunctionExtensionVariadic
		 * @see Function#Function(String, FunctionExtension)
		 */
		private FunctionExtension functionExtension;
		/**
		 * Function extension variadic (body based in code)
		 *
		 * @see FunctionExtension
		 * @see FunctionExtensionVariadic
		 * @see Function#Function(String, FunctionExtension)
		 */
		private FunctionExtensionVariadic functionExtensionVariadic;
		/*=================================================
		 *
		 * Constructors
		 *
		 *=================================================
		 */
		/**
		 * Constructor - creates function from function name
		 * and function expression string.
		 *
		 * @param      functionName              the function name
		 * @param      functionExpressionString  the function expression string
		 * @param      elements                  Optional elements list (variadic - comma separated) of types: Argument, Constant, Function
		 *
		 * @see        PrimitiveElement
		 * @see        Expression
		 */
		public Function(String functionName
						,String  functionExpressionString, params PrimitiveElement[] elements) : base(Function.TYPE_ID) {
			if (mXparser.regexMatch(functionName, ParserSymbol.nameOnlyTokenRegExp)) {
				this.functionName = functionName;
				functionExpression = new Expression(functionExpressionString, elements);
				functionExpression.setDescription(functionName);
				functionExpression.UDFExpression = true;
				isVariadic = false;
				parametersNumber = 0;
				description = "";
				functionBodyType = BODY_RUNTIME;
				addFunctions(this);
			}
			else {
				parametersNumber = 0;
				description = "";
				functionExpression = new Expression("");
				functionExpression.setSyntaxStatus(SYNTAX_ERROR_OR_STATUS_UNKNOWN, "[" + functionName + "]" + "Invalid function name, pattern not matches: " + ParserSymbol.nameTokenRegExp);
			}
		}
		/**
		 * Constructor - creates function from function name,
		 * function expression string and argument names.
		 *
		 * @param      functionName              the function name
		 * @param      functionExpressionString  the function expression string
		 * @param      argumentsNames            the arguments names (variadic parameters)
		 *                                       comma separated list
		 *
		 * @see        Expression
		 */
		public Function(String functionName
						,String  functionExpressionString
						,params String[] argumentsNames ) : base(Function.TYPE_ID) {
			if (mXparser.regexMatch(functionName, ParserSymbol.nameOnlyTokenRegExp)) {
				this.functionName = functionName;
				functionExpression = new Expression(functionExpressionString);
				functionExpression.setDescription(functionName);
				functionExpression.UDFExpression = true;
				isVariadic = false;
				foreach (String argName in argumentsNames)
					functionExpression.addArguments(new Argument(argName));
				parametersNumber = argumentsNames.Length - countRecursiveArguments();
				description = "";
				functionBodyType = BODY_RUNTIME;
				addFunctions(this);
			}
			else {
				parametersNumber = 0;
				description = "";
				functionExpression = new Expression("");
				functionExpression.setSyntaxStatus(SYNTAX_ERROR_OR_STATUS_UNKNOWN, "[" + functionName + "]" + "Invalid function name, pattern not matches: " + ParserSymbol.nameTokenRegExp);
			}
		}
		/**
		 * Constructor for function definition in natural math language,
		 * for instance providing on string "f(x,y) = sin(x) + cos(x)"
		 * is enough to define function "f" with parameters "x and y"
		 * and function body "sin(x) + cos(x)".
		 *
		 * @param functionDefinitionString      Function definition in the form
		 *                                      of one String, ie "f(x,y) = sin(x) + cos(x)"
		 * @param elements                      Optional elements list (variadic - comma separated)
		 *                                      of types: Argument, Constant, Function
		 *
		 * @see    PrimitiveElement
		 *
		 */
		public Function(String functionDefinitionString, params PrimitiveElement[] elements) : base(Function.TYPE_ID) {
			parametersNumber = 0;
			if (mXparser.regexMatch(functionDefinitionString, ParserSymbol.functionDefStrRegExp)) {
				HeadEqBody headEqBody = new HeadEqBody(functionDefinitionString);
				this.functionName = headEqBody.headTokens[0].tokenStr;
				functionExpression = new Expression(headEqBody.bodyStr, elements);
				functionExpression.setDescription(headEqBody.headStr);
				functionExpression.UDFExpression = true;
				isVariadic = false;
				if (headEqBody.headTokens.Count > 1) {
					Token t;
					for (int i = 1; i < headEqBody.headTokens.Count; i++) {
						t = headEqBody.headTokens[i];
						if (t.tokenTypeId != ParserSymbol.TYPE_ID)
							functionExpression.addArguments(new Argument(t.tokenStr));
					}
				}
				parametersNumber = functionExpression.getArgumentsNumber() - countRecursiveArguments();
				description = "";
				functionBodyType = BODY_RUNTIME;
				addFunctions(this);
			} else if ( mXparser.regexMatch(functionDefinitionString, ParserSymbol.functionVariadicDefStrRegExp) ) {
				HeadEqBody headEqBody = new HeadEqBody(functionDefinitionString);
				this.functionName = headEqBody.headTokens[0].tokenStr;
				functionExpression = new Expression(headEqBody.bodyStr, elements);
				functionExpression.setDescription(headEqBody.headStr);
				functionExpression.UDFExpression = true;
				isVariadic = true;
				parametersNumber = -1;
				description = "";
				functionBodyType = BODY_RUNTIME;
				addFunctions(this);
			} else {
				functionExpression = new Expression();
				functionExpression.setDescription(functionDefinitionString);
				String errorMessage = ""; errorMessage = errorMessage + "\n [" + functionDefinitionString + "] " + "--> pattern not mathes: f(x1,...,xn) = ... reg exp: " + ParserSymbol.functionDefStrRegExp;
				functionExpression.setSyntaxStatus(Expression.SYNTAX_ERROR_OR_STATUS_UNKNOWN, errorMessage);
			}
		}
		/**
		 * Constructor for function definition based on
		 * your own source code - this is via implementation
		 * of FunctionExtension interface.
		 *
		 * @param functionName       Function name
		 * @param functionExtension  Your own source code
		 */
		public Function(String functionName, FunctionExtension functionExtension) : base(Function.TYPE_ID) {
			if (mXparser.regexMatch(functionName, ParserSymbol.nameOnlyTokenRegExp)) {
				this.functionName = functionName;
				functionExpression = new Expression("{body-ext}");
				isVariadic = false;
				parametersNumber = functionExtension.getParametersNumber();
				description = "";
				this.functionExtension = functionExtension;
				functionBodyType = BODY_EXTENDED;
			} else {
				parametersNumber = 0;
				description = "";
				functionExpression = new Expression("");
				functionExpression.setSyntaxStatus(SYNTAX_ERROR_OR_STATUS_UNKNOWN, "[" + functionName + "]" + "Invalid function name, pattern not matches: " + ParserSymbol.nameTokenRegExp);
			}
		}
		/**
		 * Constructor for function definition based on
		 * your own source code - this is via implementation
		 * of FunctionExtensionVariadic interface.
		 *
		 * @param functionName       Function name
		 * @param functionExtensionVariadic  Your own source code
		 */
		public Function(String functionName, FunctionExtensionVariadic functionExtensionVariadic) : base(Function.TYPE_ID) {
			if ( mXparser.regexMatch(functionName, ParserSymbol.nameOnlyTokenRegExp) ) {
				this.functionName = functionName;
				functionExpression = new Expression("{body-ext-var}");
				isVariadic = true;
				parametersNumber = -1;
				description = "";
				this.functionExtensionVariadic = functionExtensionVariadic;
				functionBodyType = BODY_EXTENDED;
			} else {
				parametersNumber = 0;
				description = "";
				functionExpression = new Expression("");
				functionExpression.setSyntaxStatus(SYNTAX_ERROR_OR_STATUS_UNKNOWN, "[" + functionName + "]" + "Invalid function name, pattern not matches: " + ParserSymbol.nameTokenRegExp);
			}
		}
		/**
		 * Private constructor used for function cloning.
		 *
		 * @param      function            the function, which is going
		 *                                 to be cloned.
		 */
		private Function(Function function) : base(Function.TYPE_ID) {
			functionName = function.functionName;
			description = function.description;
			parametersNumber = function.parametersNumber;
			functionExpression = function.functionExpression.clone();
			functionBodyType = function.functionBodyType;
			isVariadic = function.isVariadic;
			if (functionBodyType == BODY_EXTENDED) {
				if (function.functionExtension != null) functionExtension = function.functionExtension.clone();
				if (function.functionExtensionVariadic != null) functionExtensionVariadic = function.functionExtensionVariadic.clone();
			}
		}
		/**
		 * Constructor for function definition in natural math language,
		 * for instance providing on string "f(x,y) = sin(x) + cos(x)"
		 * is enough to define function "f" with parameters "x and y"
		 * and function body "sin(x) + cos(x)".
		 *
		 * @param functionDefinitionString      Function definition in the form
		 *                                      of one String, ie "f(x,y) = sin(x) + cos(x)"
		 * @param elements                      Optional elements list (variadic - comma separated)
		 *                                      of types: Argument, Constant, Function
		 *
		 * @see    PrimitiveElement
		 *
		 */
		public void setFunction(String functionDefinitionString, params PrimitiveElement[] elements) {
			parametersNumber = 0;
			if ( mXparser.regexMatch(functionDefinitionString, ParserSymbol.functionDefStrRegExp) ) {
				HeadEqBody headEqBody = new HeadEqBody(functionDefinitionString);
				this.functionName = headEqBody.headTokens[0].tokenStr;
				functionExpression = new Expression(headEqBody.bodyStr, elements);
				functionExpression.setDescription(headEqBody.headStr);
				functionExpression.UDFExpression = true;
				isVariadic = false;
				if (headEqBody.headTokens.Count > 1) {
					Token t;
					for (int i = 1; i < headEqBody.headTokens.Count; i++) {
						t = headEqBody.headTokens[i];
						if (t.tokenTypeId != ParserSymbol.TYPE_ID)
							functionExpression.addArguments(new Argument(t.tokenStr));
					}
				}
				parametersNumber = functionExpression.getArgumentsNumber() - countRecursiveArguments();
				description = "";
				functionBodyType = BODY_RUNTIME;
				addFunctions(this);
			} else if ( mXparser.regexMatch(functionDefinitionString, ParserSymbol.functionVariadicDefStrRegExp) ) {
				HeadEqBody headEqBody = new HeadEqBody(functionDefinitionString);
				this.functionName = headEqBody.headTokens[0].tokenStr;
				functionExpression = new Expression(headEqBody.bodyStr, elements);
				functionExpression.setDescription(headEqBody.headStr);
				functionExpression.UDFExpression = true;
				isVariadic = true;
				parametersNumber = -1;
				description = "";
				functionBodyType = BODY_RUNTIME;
				addFunctions(this);
			} else {
				functionExpression = new Expression();
				functionExpression.setDescription(functionDefinitionString);
				String errorMessage = ""; errorMessage = errorMessage + "\n [" + functionDefinitionString + "] " + "--> pattern not mathes: f(x1,...,xn) = ... reg exp: " + ParserSymbol.functionDefStrRegExp;
				functionExpression.setSyntaxStatus(Expression.SYNTAX_ERROR_OR_STATUS_UNKNOWN, errorMessage);
			}
		}
		/**
		 * Sets function description.
		 *
		 * @param      description         the function description
		 */
		public void setDescription(String description) {
			this.description = description;
		}
		/**
		 * Gets function description
		 *
		 * @return     Function description as string
		 */
		public String getDescription() {
			return description;
		}
		/**
		 * Gets function name.
		 *
		 * @return     Function name as string.
		 */
		public String getFunctionName() {
			return functionName;
		}
		/**
		 * Gets function expression string
		 *
		 * @return     Function expression as string.
		 */
		public String getFunctionExpressionString() {
			return functionExpression.getExpressionString();
		}
		/**
		 * Sets function name.
		 *
		 * @param      functionName        the function name
		 */
		public void setFunctionName(String functionName) {
			if (mXparser.regexMatch(functionName, ParserSymbol.nameOnlyTokenRegExp)) {
				this.functionName = functionName;
				setExpressionModifiedFlags();
			}
			else functionExpression.setSyntaxStatus(SYNTAX_ERROR_OR_STATUS_UNKNOWN, "[" + functionName + "]" + "Invalid function name, pattern not matches: " + ParserSymbol.nameTokenRegExp);
		}
		/**
		 * Sets value of function argument (function parameter).
		 *
		 * @param      argumentIndex   the argument index (in accordance to
		 *                             arguments declaration sequence)
		 * @param      argumentValue   the argument value
		 */
		public void setArgumentValue(int argumentIndex, double argumentValue) {
			if (isVariadic == false)
				if (functionBodyType == BODY_RUNTIME)
					functionExpression.argumentsList[argumentIndex].argumentValue = argumentValue;
				else if (isVariadic == false)
					functionExtension.setParameterValue(argumentIndex, argumentValue);
		}
		/**
		 * Returns function body type: {@link Function#BODY_RUNTIME} {@link Function#BODY_EXTENDED}
		 * @return Returns function body type: {@link Function#BODY_RUNTIME} {@link Function#BODY_EXTENDED}
		 */
		public int getFunctionBodyType() {
			return functionBodyType;
		}
		/**
		 * Checks function syntax
		 *
		 * @return     syntax status: Function.NO_SYNTAX_ERRORS,
		 *                            Function.SYNTAX_ERROR_OR_STATUS_UNKNOWN
		 *
		 */
		public bool checkSyntax() {
			bool syntaxStatus = Function.NO_SYNTAX_ERRORS;
			if (functionBodyType != BODY_EXTENDED)
				syntaxStatus = functionExpression.checkSyntax();
			checkRecursiveMode();
			return syntaxStatus;
		}
		/**
		 * Returns error message after checking the syntax.
		 *
		 * @return     Error message as string.
		 */
		public String getErrorMessage() {
			return functionExpression.getErrorMessage();
		}
		/**
		 * clone method
		 */
		internal Function clone() {
			Function newFunction = new Function(this);
			return newFunction;
		}
		/**
		 * Calculates function value
		 *
		 * @return     Function value as double.
		 */
		public double calculate() {
			if (functionBodyType == BODY_RUNTIME)
				return functionExpression.calculate();
			else
				if (isVariadic == false)
					return functionExtension.calculate();
				else {
					List<Double> paramsList = functionExpression.UDFVariadicParamsAtRunTime;
					if (paramsList != null) {
						int n = paramsList.Count;
						double[] parameters = new double[n];
						for (int i = 0; i < n; i++)
							parameters[i] = paramsList[i];
						return functionExtensionVariadic.calculate(parameters);
					} else return Double.NaN;
				}
		}
		/**
		 * Calculates function value
		 *
		 * @param      params              the function parameters values (as doubles)
		 *
		 * @return     function value as double.
		 */
		public double calculate(params double[] parameters) {
			if (parameters.Length > 0) {
				functionExpression.UDFVariadicParamsAtRunTime = new List<Double>();
				foreach (double x in parameters)
					functionExpression.UDFVariadicParamsAtRunTime.Add(x);
			} else return Double.NaN;
			if (isVariadic) {
				if (functionBodyType == BODY_RUNTIME)
					return functionExpression.calculate();
				else
					return functionExtensionVariadic.calculate(parameters);
			} else if (parameters.Length == this.getParametersNumber()) {
				if (functionBodyType == BODY_RUNTIME) {
					for (int p = 0; p < parameters.Length; p++)
						setArgumentValue(p, parameters[p]);
					return functionExpression.calculate();
				} else {
					for (int p = 0; p < parameters.Length; p++)
						functionExtension.setParameterValue(p, parameters[p]);
					return functionExtension.calculate();
				}
			}
			else {
				this.functionExpression.setSyntaxStatus(SYNTAX_ERROR_OR_STATUS_UNKNOWN, "[" + functionName + "] incorrect number of function parameters (expecting " + getParametersNumber() + ", provided " + parameters.Length + ")!");
				return Double.NaN;
			}
		}
		/**
		 * Calculates function value
		 *
		 * @param      arguments   function parameters (as Arguments)
		 *
		 * @return     function value as double
		 */
		public double calculate(params Argument[] arguments) {
			double[] parameters;
			if (arguments.Length > 0) {
				functionExpression.UDFVariadicParamsAtRunTime = new List<Double>();
				parameters = new double[arguments.Length];
				double x;
				for (int i = 0; i < arguments.Length; i++) {
					x = arguments[i].getArgumentValue();
					functionExpression.UDFVariadicParamsAtRunTime.Add(x);
					parameters[i] = x;
				}
			} else return Double.NaN;
			if (isVariadic) {
				if (functionBodyType == BODY_RUNTIME)
					return functionExpression.calculate();
				else
					return functionExtensionVariadic.calculate(parameters);
			} else if (arguments.Length == this.getParametersNumber()) {
				if (functionBodyType == BODY_RUNTIME) {
					for (int p = 0; p < arguments.Length; p++)
						setArgumentValue(p, arguments[p].getArgumentValue());
					return functionExpression.calculate();
				} else {
					for (int p = 0; p < arguments.Length; p++)
						functionExtension.setParameterValue(p, arguments[p].getArgumentValue());
					return functionExtension.calculate();
				}
			}
			else {
				this.functionExpression.setSyntaxStatus(SYNTAX_ERROR_OR_STATUS_UNKNOWN, "[" + functionName + "] incorrect number of function parameters (expecting " + getParametersNumber() + ", provided " + arguments.Length + ")!");
				return Double.NaN;
			}
		}
		/**
		 * Adds user defined elements (such as: Arguments, Constants, Functions)
		 * to the function expressions.
		 *
		 * @param elements Elements list (variadic), where Argument, Constant, Function
		 *                 extend the same class PrimitiveElement
		 *
		 * @see PrimitiveElement
		 */
		public void addDefinitions(params PrimitiveElement[] elements) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.addDefinitions(elements);
		}
		/**
		 * Removes user defined elements (such as: Arguments, Constants, Functions)
		 * from the function expressions.
		 *
		 * @param elements Elements list (variadic), where Argument, Constant, Function
		 *                 extend the same class PrimitiveElement
		 *
		 * @see PrimitiveElement
		 */
		public void removeDefinitions(params PrimitiveElement[] elements) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.removeDefinitions(elements);
		}
		/*=================================================
		 *
		 * Arguments handling API (the same as in Expression)
		 * (protected argument expression)
		 *
		 *=================================================
		 */
		private int countRecursiveArguments() {
			int numOfRecursiveArguments = 0;
			if (functionBodyType == Function.BODY_RUNTIME)
				foreach (Argument argument in functionExpression.argumentsList)
					if (argument.getArgumentType() == Argument.RECURSIVE_ARGUMENT) numOfRecursiveArguments++;
			return numOfRecursiveArguments;
		}
		/**
		 * Adds arguments (variadic) to the function expression definition.
		 *
		 * @param      arguments           the arguments list
		 *                                 (comma separated list)
		 * @see        Argument
		 * @see        RecursiveArgument
		 */
		public void addArguments(params Argument[] arguments) {
			if (functionBodyType == Function.BODY_RUNTIME) {
				functionExpression.addArguments(arguments);
				parametersNumber = functionExpression.getArgumentsNumber() - countRecursiveArguments();
			}
		}
		/**
		 * Enables to define the arguments (associated with
		 * the function expression) based on the given arguments names.
		 *
		 * @param      argumentsNames      the arguments names (variadic)
		 *                                 comma separated list
		 *
		 * @see        Argument
		 * @see        RecursiveArgument
		 */
		public void defineArguments(params String[] argumentsNames) {
			if (functionBodyType == Function.BODY_RUNTIME) {
				functionExpression.defineArguments(argumentsNames);
				parametersNumber = functionExpression.getArgumentsNumber() - countRecursiveArguments();
			}
		}
		/**
		 * Enables to define the argument (associated with the function expression)
		 * based on the argument name and the argument value.
		 *
		 * @param      argumentName        the argument name
		 * @param      argumentValue       the the argument value
		 *
		 * @see        Argument
		 * @see        RecursiveArgument
		 */
		public void defineArgument(String argumentName, double argumentValue) {
			if (functionBodyType == Function.BODY_RUNTIME) {
				functionExpression.defineArgument(argumentName, argumentValue);
				parametersNumber = functionExpression.getArgumentsNumber() - countRecursiveArguments();
			}
		}
		/**
		 * Gets argument index from the function expression.
		 *
		 * @param      argumentName        the argument name
		 *
		 * @return     The argument index if the argument name was found,
		 *             otherwise returns Argument.NOT_FOUND
		 *
		 * @see        Argument
		 * @see        RecursiveArgument
		 */
		public int getArgumentIndex(String argumentName) {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getArgumentIndex(argumentName);
			else
				return -1;
		}
		/**
		 * Gets argument from the function expression.
		 *
		 *
		 * @param      argumentName        the argument name
		 *
		 * @return     The argument if the argument name was found,
		 *             otherwise returns null.
		 *
		 * @see        Argument
		 * @see        RecursiveArgument
		 */
		public Argument getArgument(String argumentName) {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getArgument(argumentName);
			else
				return null;
		}
		/**
		 * Gets argument from the function expression.
		 *
		 * @param      argumentIndex       the argument index
		 *
		 * @return     Argument if the argument index is between 0 and
		 *             the last available argument index (getArgumentsNumber()-1),
		 *             otherwise returns null.
		 *
		 * @see        Argument
		 * @see        RecursiveArgument
		 */
		public Argument getArgument(int argumentIndex) {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getArgument(argumentIndex);
			else
				return null;
		}
		/**
		 * Gets number of parameters associated with the function expression.
		 *
		 * @return     The number of function parameters (int >= 0)
		 *
		 * @see        Argument
		 * @see        RecursiveArgument
		 */
		public int getParametersNumber() {
			if (isVariadic == false)
				return parametersNumber;
			else {
				if (functionExpression.UDFVariadicParamsAtRunTime != null)
					return functionExpression.UDFVariadicParamsAtRunTime.Count;
				else
					return -1;
			}
		}
		/**
		 * Set parameters number.
		 *
		 * @param      parametersNumber    the number of function parameters (default = number of arguments
		 *                                 (less number might be specified).
		 */
		public void setParametersNumber(int parametersNumber) {
			if (functionBodyType == Function.BODY_RUNTIME) {
				this.parametersNumber = parametersNumber;
				functionExpression.setExpressionModifiedFlag();
			}
		}
		/**
		 * Gets user defined function parameter name
		 *
		 * @param parameterIndex  Parameter index between 0 and n-1
		 * @return If parameter exists returns parameters name, otherwise empty string is returned.
		 */
		public String getParameterName(int parameterIndex) {
			if (parameterIndex < 0) return "";
			if (parameterIndex >= parametersNumber) return "";
			if (functionBodyType == BODY_RUNTIME) return getArgument(parameterIndex).getArgumentName();
			if (functionBodyType == BODY_EXTENDED) return this.functionExtension.getParameterName(parameterIndex);
			return "";
		}
		/**
		 * Gets number of arguments associated with the function expression.
		 *
		 * @return     The number of arguments (int >= 0)
		 *
		 * @see        Argument
		 * @see        RecursiveArgument
		 */
		public int getArgumentsNumber() {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getArgumentsNumber();
			else
				return 0;
		}
		/**
		 * Removes first occurrences of the arguments
		 * associated with the function expression.
		 *
		 * @param      argumentsNames      the arguments names
		 *                                 (variadic parameters) comma separated
		 *                                 list
		 *
		 * @see        Argument
		 * @see        RecursiveArgument
		 */
		public void removeArguments(params String[] argumentsNames) {
			if (functionBodyType == Function.BODY_RUNTIME) {
				functionExpression.removeArguments(argumentsNames);
				parametersNumber = functionExpression.getArgumentsNumber() - countRecursiveArguments();
			}
		}
		/**
		 * Removes first occurrences of the arguments
		 * associated with the function expression.
		 *
		 * @param      arguments           the arguments (variadic parameters)
		 *                                 comma separated list
		 *
		 * @see        Argument
		 * @see        RecursiveArgument
		 */
		public void removeArguments(params Argument[] arguments) {
			if (functionBodyType == Function.BODY_RUNTIME) {
				functionExpression.removeArguments(arguments);
				parametersNumber = functionExpression.getArgumentsNumber() - countRecursiveArguments();
			}
		}
		/**
		 * Removes all arguments associated with the function expression.
		 *
		 * @see        Argument
		 * @see        RecursiveArgument
		 */
		public void removeAllArguments() {
			if (functionBodyType == Function.BODY_RUNTIME) {
				functionExpression.removeAllArguments();
				parametersNumber = 0;
			}
		}
		/*=================================================
		 *
		 * Constants handling API (the same as in Expression)
		 * (protected argument expression)
		 *
		 *=================================================
		 */
		/**
		 * Adds constants (variadic parameters) to the function expression definition.
		 *
		 * @param      constants           the constants
		 *                                 (comma separated list)
		 *
		 * @see        Constant
		 */
		public void addConstants(params Constant[] constants) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.addConstants(constants);
		}
		/**
		 * Enables to define the constant (associated with
		 * the function expression) based on the constant name and
		 * constant value.
		 *
		 * @param      constantName        the constant name
		 * @param      constantValue       the constant value
		 *
		 * @see        Constant
		 */
		public void defineConstant(String constantName, double constantValue) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.defineConstant(constantName, constantValue);
		}
		/**
		 * Gets constant index associated with the function expression.
		 *
		 * @param      constantName        the constant name
		 *
		 * @return     Constant index if constant name was found,
		 *             otherwise return Constant.NOT_FOUND.
		 *
		 * @see        Constant
		 */
		public int getConstantIndex(String constantName) {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getConstantIndex(constantName);
			else
				return -1;
		}
		/**
		 * Gets constant associated with the function expression.
		 *
		 * @param      constantName        the constant name
		 *
		 * @return     Constant if constant name was found,
		 *             otherwise return null.
		 *
		 * @see        Constant
		 */
		public Constant getConstant(String constantName) {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getConstant(constantName);
			else
				return null;
		}
		/**
		 * Gets constant associated with the function expression.
		 *
		 * @param      constantIndex       the constant index
		 *
		 * @return     Constant if the constantIndex is between
		 *             0 and the last available constant index
		 *             (getConstantsNumber() - 1),
		 *             otherwise it returns null.
		 *
		 * @see        Constant
		 */
		public Constant getConstant(int constantIndex) {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getConstant(constantIndex);
			else
				return null;
		}
		/**
		 * Gets number of constants associated with the function expression.
		 *
		 * @return     number of constants (int >= 0)
		 *
		 * @see        Constant
		 */
		public int getConstantsNumber() {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getConstantsNumber();
			else
				return 0;
		}
		/**
		 * Removes first occurrences of the constants
		 * associated with the function expression.
		 *
		 * @param      constantsNames      the constants names (variadic parameters)
		 *                                 comma separated list
		 *
		 * @see        Constant
		 */
		public void removeConstants(params String[] constantsNames) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.removeConstants(constantsNames);
		}
		/**
		 * Removes first occurrences of the constants
		 * associated with the function expression
		 *
		 * @param      constants           the constants (variadic parameters)
		 *                                 comma separated list
		 *
		 * @see        Constant
		 */
		public void removeConstants(params Constant[] constants) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.removeConstants(constants);
		}
		/**
		 * Removes all constants
		 * associated with the function expression
		 *
		 * @see        Constant
		 */
		public void removeAllConstants() {
			functionExpression.removeAllConstants();
		}
		/*=================================================
		 *
		 * Functions handling API (the same as in Expression)
		 * (protected argument expression)
		 *
		 *=================================================
		 */
		/**
		 * Adds functions (variadic parameters) to the function expression definition.
		 *
		 * @param      functions           the functions
		 *                                 (variadic parameters) comma separated list
		 *
		 * @see        Function
		 */
		public void addFunctions(params Function[] functions) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.addFunctions(functions);
		}
		/**
		 * Enables to define the function (associated with
		 * the function expression) based on the function name,
		 * function expression string and arguments names (variadic parameters).
		 *
		 * @param      functionName                  the function name
		 * @param      functionExpressionString      the expression string
		 * @param      argumentsNames                the function arguments names
		 *                                           (variadic parameters)
		 *                                           comma separated list
		 *
		 * @see        Function
		 */
		public void defineFunction(String functionName, String  functionExpressionString,
				params String[] argumentsNames) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.defineFunction(functionName, functionExpressionString, argumentsNames);
		}
		/**
		 * Gets index of function associated with the function expression.
		 *
		 * @param      functionName        the function name
		 *
		 * @return     Function index if function name was found,
		 *             otherwise returns Function.NOT_FOUND
		 *
		 * @see        Function
		 */
		public int getFunctionIndex(String functionName) {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getFunctionIndex(functionName);
			else
				return -1;
		}
		/**
		 * Gets function associated with the function expression.
		 *
		 * @param      functionName        the function name
		 *
		 * @return     Function if function name was found,
		 *             otherwise returns null.
		 *
		 * @see        Function
		 */
		public Function getFunction(String functionName) {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getFunction(functionName);
			else
				return null;
		}
		/**
		 * Gets function associated with the function expression.
		 *
		 * @param      functionIndex the function index
		 *
		 * @return     Function if function index is between 0 and
		 *             the last available function index (getFunctionsNumber()-1),
		 *             otherwise returns null.
		 *
		 * @see        Function
		 */
		public Function getFunction(int functionIndex) {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getFunction(functionIndex);
			else
				return null;
		}
		/**
		 * Gets number of functions associated with the function expression.
		 *
		 * @return     number of functions (int >= 0)
		 *
		 * @see        Function
		 */
		public int getFunctionsNumber() {
			if (functionBodyType == Function.BODY_RUNTIME)
				return functionExpression.getFunctionsNumber();
			else
				return 0;
		}
		/**
		 * Removes first occurrences of the functions
		 * associated with the function expression.
		 *
		 * @param      functionsNames      the functions names (variadic parameters)
		 *                                 comma separated list
		 *
		 * @see        Function
		 */
		public void removeFunctions(params String[] functionsNames) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.removeFunctions(functionsNames);
		}
		/**
		 * Removes first occurrences of the functions
		 * associated with the function expression.
		 *
		 * @param      functions           the functions (variadic parameters)
		 *                                 comma separated list.
		 *
		 * @see        Function
		 */
		public void removeFunctions(params Function[] functions) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.removeFunctions(functions);
		}
		/**
		 * Removes all functions
		 * associated with the function expression.
		 *
		 * @see        Function
		 */
		public void removeAllFunctions() {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.removeAllFunctions();
		}
		/*
		 * ---------------------------------------------
		 */
		/**
		 * Enables verbose function mode
		 */
		public void setVerboseMode() {
			functionExpression.setVerboseMode();
		}
		/**
		 * Disables function verbose mode (sets default silent mode)
		 */
		public void setSilentMode() {
			functionExpression.setSilentMode();
		}
		/**
		 * Returns verbose mode status
		 *
		 * @return     true if verbose mode is on,
		 *             otherwise returns false
		 */
		public bool getVerboseMode() {
			return functionExpression.getVerboseMode();
		}
		/**
		 * Checks whether function name appears in function body
		 * if yes the recursive mode is being set
		 */
		internal void checkRecursiveMode() {
			if (functionBodyType == Function.BODY_RUNTIME) {
				List<Token> functionExpressionTokens = functionExpression.getInitialTokens();
				functionExpression.disableRecursiveMode();
				if (functionExpressionTokens != null)
					foreach (Token t in functionExpressionTokens)
						if (t.tokenStr.Equals(functionName)) {
							functionExpression.setRecursiveMode();
							break;
						}
			}
		}
		/**
		 * Gets recursive mode status
		 *
		 * @return     true if recursive mode is enabled,
		 *             otherwise returns false
		 *
		 */
		public bool getRecursiveMode() {
			return functionExpression.getRecursiveMode();
		}
		/**
		 * Gets computing time
		 *
		 * @return     computing time in seconds.
		 */
		public double getComputingTime() {
			return functionExpression.getComputingTime();
		}
		/**
		 * Adds related expression.
		 *
		 * @param      expression          the related expression
		 */
		internal void addRelatedExpression(Expression expression) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.addRelatedExpression(expression);
		}
		/**
		 * Removes related expression.
		 *
		 * @param      expression          the related expression
		 */
		internal void removeRelatedExpression(Expression expression) {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.removeRelatedExpression(expression);
		}
		/**
		 * Set expression modified flags in the related expressions.
		 */
		internal void setExpressionModifiedFlags() {
			if (functionBodyType == Function.BODY_RUNTIME)
				functionExpression.setExpressionModifiedFlag();
		}
	}
}