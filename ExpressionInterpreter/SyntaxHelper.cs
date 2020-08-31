using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExpressionInterpreterExample
{
    public class SyntaxHelper
    {
        // Regular expression for parsing the mathematical expression.
        private const string ParsingExpressionPattern = @"([0-9]+|[a-z\+\-\*\(\)])";

        // Regular expression for white-listing of the given input-string.
        private const string WhiteListExpressionPattern = @"^([0-9a-z\+\-\*\(\) ]+)$";

        // Description of valid characters (will be added to error-message if invalid
        // charters had been found).
        private static readonly string AllowedExpressionCharacters = "Allowed characters are: "
                                    + Environment.NewLine + " a-z"
                                    + Environment.NewLine + " 0-9"
                                    + Environment.NewLine + " + - *"
                                    + Environment.NewLine + " ( )";

        /// <summary>
        /// Checks if the expression contains only valid characters. If not, an exception will be thrown.
        /// </summary>
        /// <param name="expressionAsText">
        /// Textual mathematical expression.
        /// </param>
        public static void CheckForValidCharacters(string expressionAsText)
        {
            var onlyValidCharactersFound = Regex.IsMatch(expressionAsText, WhiteListExpressionPattern);
            onlyValidCharactersFound.EnsureValidData($"Invalid characters in expression '{expressionAsText}'. {AllowedExpressionCharacters}");
        }

        /// <summary>
        /// Splits down the expression to its elements. Spaces get removed and thus ignored.
        /// </summary>
        /// <param name="expressionAsText">
        /// Textual mathematical expression.
        /// </param>
        public static string[] SplitExpressionToElements(string expressionAsText)
        {
            var regEx = new Regex(ParsingExpressionPattern, RegexOptions.None);
            var expressionWithoutSpaces = RemoveSpaces(expressionAsText);
            var matchCollection = regEx.Matches(expressionWithoutSpaces);
            return matchCollection.Cast<Match>().Select(m => m.Value).ToArray();
        }

        private static string RemoveSpaces(string inputString)
        {
            return Regex.Replace(inputString, @"\s+", string.Empty);
        }


        public static bool IsScalar(string scalarString)
        {
            return IsConstant(scalarString)
                || IsVariable(scalarString);
        }

        public static bool IsVariable(string variableCharacter)
        {
            return variableCharacter.Length == 1
                && variableCharacter[0] >= 'a'
                && variableCharacter[0] <= 'z';
        }

        public static bool IsConstant(string numberString)
        {
            return numberString.Length > 0
                   && int.TryParse(numberString, out _);
        }

        public static bool IsBracketOpening(string bracketCharacter)
        {
            return bracketCharacter.Length == 1
                && bracketCharacter[0] == '(';
        }

        public static bool IsBracketClosing(string bracketCharacter)
        {
            return bracketCharacter.Length == 1
                && bracketCharacter[0] == ')';
        }

        public static bool IsOperator(string operatorCharacter)
        {
            return IsOperatorAdd(operatorCharacter)
                || IsOperatorSub(operatorCharacter)
                || IsOperatorMult(operatorCharacter);
        }

        public static bool IsOperatorAdd(string operatorCharacter)
        {
            return operatorCharacter.Length == 1
                && operatorCharacter[0] == '+';
        }

        public static bool IsOperatorSub(string operatorCharacter)
        {
            return operatorCharacter.Length == 1
                && operatorCharacter[0] == '-';
        }

        public static bool IsOperatorMult(string operatorCharacter)
        {
            return operatorCharacter.Length == 1
                && operatorCharacter[0] == '*';
        }
        
        /// <summary>
        /// Get operator type by operator character.
        /// </summary>
        /// <param name="operatorString">
        /// String containing the operator character.
        /// </param>
        /// <returns>
        /// Return the type of the operator as OperatorType.
        /// </returns>
        public static OperatorType GetOperatorType(string operatorString)
        {
            if (IsOperatorAdd(operatorString))
            {
                return OperatorType.Add;
            }
            if (IsOperatorSub(operatorString))
            {
                return OperatorType.Sub;
            }
            if (IsOperatorMult(operatorString))
            {
                return OperatorType.Mult;
            }

            throw new DataException($"Unexpected operator '{operatorString}'.");
        }
    }
}
