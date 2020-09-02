using System;
using ExpressionInterpreterExample;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTestsExpressionInterpreter
{
    [TestClass]
    public class UnitTestExpressionInterpreter
    {
        /// <summary>
        /// Test single numbers.
        /// </summary>
        [DataTestMethod]
        [DataRow("5", 5)]
        [DataRow("42", 42)]
        [DataRow("987654", 987654)]
        public void TestSingleNumber(string expression, int value)
        {
            ExecuteTestCase(expression, value);
        }

        /// <summary>
        /// Test simple expressions without variables or brackets. 
        /// Test also the rule "multiplication before addition/subtraction".
        /// </summary>
        [DataTestMethod]
        [DataRow("4+ 5", 9)]
        [DataRow("2 - 15", -13)]
        [DataRow("42 *3", 126)]
        [DataRow("14 - 4 * 4 + 2", 0)]
        [DataRow("1*2*3*4+1", 25)]
        public void TestSimpleExpressions(string expression, int value)
        {
            ExecuteTestCase(expression, value);
        }

        /// <summary>
        /// Text expressions with variables.
        /// </summary>
        [DataTestMethod]
        [DataRow("a+ 5", 7)]
        [DataRow("2 - b", -3)]
        [DataRow("42 *c", 336)]
        [DataRow("c", 8)]
        [DataRow("a + a * a", 6)]
        [DataRow("c *c*c", 512)]
        public void TestVariableExpressions(string expression, int value)
        {
            var variableValues = new Dictionary<Variable, int>
            {
                {new Variable("a"), 2}, {new Variable("b"), 5}, {new Variable("c"), 8}
            };

            ExecuteTestCase(expression, value, variableValues);
        }

        /// <summary>
        /// Test the expression given in the problem description.
        /// </summary>
        [TestMethod]
        public void TestPredefinedExpression()
        {
            var variableValues = new Dictionary<Variable, int>
            {
                {new Variable("x"), 1}, {new Variable("y"), 2}, {new Variable("z"), 3}
            };

            ExecuteTestCase("3*x + 20- y *(z + 17)", -17, variableValues);
        }

        /// <summary>
        /// Test expressions with brackets and variables.
        /// </summary>
        [DataTestMethod]
        [DataRow("a * ( 3 + 7)", 20)]
        [DataRow("3 * ( a + b * (17 - c)) + 2", 143)]
        [DataRow("(33 + b) * (4711-23 * (42 - 0)) + a", 142312)]
        [DataRow("( 3 + 7)", 10)]
        public void TestBracketExpressions(string expression, int value)
        {
            var variableValues = new Dictionary<Variable, int>
            {
                {new Variable("a"), 2}, {new Variable("b"), 5}, {new Variable("c"), 8}
            };

            ExecuteTestCase(expression, value, variableValues);
        }

        /// <summary>
        /// Test expressions with variables but variable values are missing. 
        /// The constructor should run without error because the expressions are valid.
        /// During calculation we expect an error.
        /// </summary>
        [DataTestMethod]
        [DataRow("a * b")]
        [DataRow("a + b - c")]
        [DataRow("123 + a - 22 * 13 - c + b")]
        [DataRow("100 + 5 - x - 1")]
        [ExpectedException(typeof(DataException))]
        public void TestMissingVariableValues(string expression)
        {
            var variableValues = new Dictionary<Variable, int> {{new Variable("a"), 2}, {new Variable("c"), 8}};

            ExecuteTestCase(expression, 0, variableValues);
        }

        /// <summary>
        /// Test several invalid expressions: syntax errors and invalid characters.
        /// </summary>
        [DataTestMethod]
        // missing closing parenthesis
        [DataRow("a * ( 3 + 7) (")]
        // unexpected closing parenthesis at the end
        [DataRow("a * ( 3 + 7) )")]
        // unexpected closing parenthesis in the middle
        [DataRow("a * ) ( 3 + 7) ")]
        // unexpected operator i.e. missing scalar
        [DataRow("* 3 + 5")]
        [DataRow("3 + * 5")]
        [DataRow("3 + 5 * ")]
        [DataRow("3 + 5 * (+ 7 - 3)")]
        [ExpectedException(typeof(SyntaxException))]
        public void TestInvalidExpressionsCausingSyntaxException(string expression)
        {
            var variableValues = new Dictionary<Variable, int>
            {
                {new Variable("a"), 2}, {new Variable("b"), 5}, {new Variable("c"), 8}
            };

            ExecuteTestCase(expression, 0, variableValues);
        }

        /// <summary>
        /// Test several invalid expressions: syntax errors and invalid characters.
        /// </summary>
        [DataTestMethod]
        // invalid characters
        [DataRow("3 + 5 % 2")]
        [DataRow("3 + 5 * A")]
        [DataRow("3 + 5 * ^")]
        [DataRow("3232XYZ")]
        [DataRow("äöü")]
        [DataRow("=/&%$§!\"")]
        // empty expression
        [DataRow("")]
        [DataRow(null)]
        [ExpectedException(typeof(DataException))]
        public void TestInvalidExpressionsCausingDataException(string expression)
        {
            var variableValues = new Dictionary<Variable, int>
            {
                {new Variable("a"), 2}, {new Variable("b"), 5}, {new Variable("c"), 8}
            };

            ExecuteTestCase(expression, 0, variableValues);
        }

        [TestMethod]
        [ExpectedException(typeof(DataException))]
        public void VariableConstructorWithInvalidNameThrows()
        {
            var _ = new Variable("ab");
        }

        [DataTestMethod]
        [DataRow("/")]
        [DataRow("x")]
        [DataRow("")]
        [DataRow("**")]
        [ExpectedException(typeof(DataException))]
        public void SyntaxHelperGetOperatorTypeWithInvalidOperatorThrows(string value)
        {
            SyntaxHelper.GetOperatorType(value);
        }

        [TestMethod]
        [ExpectedException(typeof(DataException))]
        public void NodeOperatorEvaluateWithInvalidOperatorTypeThrows()
        {
            var node = new NodeOperator(new NodeNumeric(1), (OperatorType)99, new NodeNumeric(1));
            node.Evaluate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NodeOperatorConstructorWithLeftNodeNullThrows()
        {
            var _ = new NodeOperator(null, (OperatorType)99, new NodeNumeric(1));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NodeOperatorConstructorWithRightNodeNullThrows()
        {
            var _ = new NodeOperator(new NodeNumeric(1), (OperatorType)99, null);
        }


        private void ExecuteTestCase(string expression, int value, Dictionary<Variable, int> variableValues = null)
        {
            var expressionInterpreter = new ExpressionInterpreter(expression);
            var result = expressionInterpreter.CalculateWith(variableValues);
            Assert.AreEqual(value, result, $"Expected result for expression '{expression}' was {value} but got {result}");
        }
    }
}
