using System;
using worksample;
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
        [TestMethod]
        public void TestSingleNumber()
        {
            var testCases = new Dictionary<string, int>();
            testCases["5"] = 5;
            testCases["42"] = 42;
            testCases["987654"] = 987654;

            this.ExecuteTestCases(testCases);
        }

        /// <summary>
        /// Test simple expressions without variables or brackets. 
        /// Test also the rule "multiplication before addition/subtraction".
        /// </summary>
        [TestMethod]
        public void TestSimpleExpressions()
        {
            var testCases = new Dictionary<string,int>();
            testCases["4+ 5"] = 9;
            testCases["2 - 15"] = -13;
            testCases["42 *3"] = 126;
            testCases["14 - 4 * 4 + 2"] = 0;
            testCases["1*2*3*4+1"] = 25;

            this.ExecuteTestCases(testCases);
        }

        /// <summary>
        /// Text expressions with variables.
        /// </summary>
        [TestMethod]
        public void TestVariableExpressions()
        {
            var testCases = new Dictionary<string, int>();
            testCases["a+ 5"] = 7;
            testCases["2 - b"] = -3;
            testCases["42 *c"] = 336;
            testCases["c"] = 8;
            testCases["a + a * a"] = 6;
            testCases["c *c*c"] = 512;
            var variableValues = new Dictionary<Variable, int>();
            variableValues.Add(new Variable("a"), 2);
            variableValues.Add(new Variable("b"), 5);
            variableValues.Add(new Variable("c"), 8);

            this.ExecuteTestCases(testCases, variableValues);
        }

        /// <summary>
        /// Test the expression given in the problem description.
        /// </summary>
        [TestMethod]
        public void TestPredefinedExpression()
        {
            var testCases = new Dictionary<string, int>();
            testCases["3*x + 20- y *(z + 17)"] = -17;
            var variableValues = new Dictionary<Variable, int>();
            variableValues.Add(new Variable("x"), 1);
            variableValues.Add(new Variable("y"), 2);
            variableValues.Add(new Variable("z"), 3);

            this.ExecuteTestCases(testCases, variableValues);
        }

        /// <summary>
        /// Test expressions with brackets and variables.
        /// </summary>
        [TestMethod]
        public void TestBracketExpressions()
        {
            var testCases = new Dictionary<string, int>();
            testCases["a * ( 3 + 7)"] = 20;
            testCases["3 * ( a + b * (17 - c)) + 2"] = 143;
            testCases["(33 + b) * (4711-23 * (42 - 0)) + a"] = 142312;
            testCases["( 3 + 7)"] = 10;
            var variableValues = new Dictionary<Variable, int>();
            variableValues.Add(new Variable("a"), 2);
            variableValues.Add(new Variable("b"), 5);
            variableValues.Add(new Variable("c"), 8);

            this.ExecuteTestCases(testCases, variableValues);
        }

        /// <summary>
        /// Test expressions with variables but variable values are missing. 
        /// The constructor should run without error because the expressions are valid.
        /// During calcualtion we expect an error.
        /// </summary>
        [TestMethod]
        public void TestMissingVariableValues()
        {
            var testCases = new Dictionary<string, int>();
            testCases["a * b"] = 0;
            testCases["a + b - c"] = 0;
            testCases["123 + a - 22 * 13 - c + b"] = 0;
            testCases["100 + 5 - x - 1"] = 0;
            var variableValues = new Dictionary<Variable, int>();
            variableValues.Add(new Variable("a"), 2);
            variableValues.Add(new Variable("c"), 8);

            foreach (var testCase in testCases)
            {
                var expressionInterpreter = new ExpressionInterpreter(testCase.Key);
                try
                {
                    var result = expressionInterpreter.CalculateWith(variableValues);
                    Assert.Fail($"The calculation should have created an error for expression '{testCase.Key}'");
                }
                catch (DataException)
                {
                    // The expected positive result it to get an exception for invalid expressions.
                    // No further action needed here.
                }
                catch(Exception ex)
                {
                    Assert.Fail($"Possible program error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Test several invalid expressions: syntax errors and invalid charcters.
        /// </summary>
        [TestMethod]
        public void TestInvalidExpressions()
        {
            // key of dictionary is the expression to be tested, value is expected result
            var testCases = new Dictionary<string, int>();
            // missing closing parenthesis
            testCases["a * ( 3 + 7) ("] = 0;
            // unexpected closing parenthesis at the end
            testCases["a * ( 3 + 7) )"] = 0;
            // unexpected closing parenthesis in the middle
            testCases["a * ) ( 3 + 7) "] = 0;
            // unexpected operator i.e. missing scalar
            testCases["* 3 + 5"] = 0;
            testCases["3 + * 5"] = 0;
            testCases["3 + 5 * "] = 0;
            testCases["3 + 5 * (+ 7 - 3)"] = 0;
            // invalid charaters
            testCases["3 + 5 % 2"] = 0;
            testCases["3 + 5 * A"] = 0;
            testCases["3 + 5 * ^"] = 0;
            testCases["3232XYZ"] = 0;
            testCases["äöü"] = 0;
            testCases["=/&%$§!\""] = 0;
            // empty expression
            testCases[""] = 0;

            var variableValues = new Dictionary<Variable, int>();
            variableValues.Add(new Variable("a"), 2);
            variableValues.Add(new Variable("b"), 5);
            variableValues.Add(new Variable("c"), 8);

            foreach (var testCase in testCases)
            {
                try
                {
                    var expressionInterpreter = new ExpressionInterpreter(testCase.Key);
                    Assert.Fail($"The expression should have been rejected as invalid: '{testCase.Key}'");
                }
                catch (SyntaxException)
                {
                    // The expected positive result it to get an exception for invalid expressions.
                    // No further action needed here.
                }
                catch(DataException)
                {
                    // This is also ok.
                }
                catch(Exception ex)
                {
                    Assert.Fail($"Possible program error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Test for correct handling if given expression-string is NULL.
        /// </summary>
        [TestMethod]
        public void TestNullExpression()
        {
            try
            {
                var expressionInterpreter = new ExpressionInterpreter(null);
            }
            catch(DataException)
            {
                // The expected positive result it to get an exception for invalid expressions.
                // No further action needed here.
            }
            catch (Exception ex)
            {
                Assert.Fail($"Possible program error: {ex.Message}");
            }
        }

        /// <summary>
        /// Helper method that executes a list of testcases given in a dictionary: the key is the
        /// expression and the value is the expected result-value of the expression.
        /// </summary>
        /// <param name="testCases">Dictionary with testcases.</param>
        /// <param name="variableValues">Dictionary with variable-values.</param>
        private void ExecuteTestCases(Dictionary<string, int> testCases, 
                                      Dictionary<Variable, int> variableValues = null)
        {
            foreach (var testCase in testCases)
            {
                var expressionInterpreter = new ExpressionInterpreter(testCase.Key);
                var result = expressionInterpreter.CalculateWith(variableValues);
                Assert.AreEqual(testCase.Value, result, 
                                $"Expected result for expression '{testCase.Key}' was {testCase.Value} but got {result}");
            }
        }
    }
}
