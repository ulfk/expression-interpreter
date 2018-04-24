# ExpressionInterpreter
Example implmentation of an interpreter for mathematical expressions.

The mathematical expression may consist of these parts:
- the mathematical operators for addition (+), subtraction (-) and multiplication (*)
- constant integer numbers
- variable names consisting of one lowercase character in the range from "a" to "z".
- brackets as needed for grouping parts of the expression
- spaces are allowed anywhere in the expression.

The expression must be given as string to the constuctor of ExpressionInterpreter. To calculate the value of the expression the method CalculateWith can be called with a list of the variable values to be used in the calculation.

Some expression examples:
- 3 + 4
- 7 * ( a + b * (17 - c)) + 2
- x*y+z
