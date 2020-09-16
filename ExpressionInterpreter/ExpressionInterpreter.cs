using System.Collections.Generic;

namespace ExpressionInterpreterExample
{
    public class ExpressionInterpreter : IExpressionInterpreter
    {
        #region PrivateMemberVariablesAndConstants

        /// <summary>
        /// Used during processing and holds all expression elements as single entries.
        /// </summary>
        private string[] _expressionElementList;

        /// <summary>
        /// Used during processing and is the current index to the array expressionElementList.
        /// </summary>
        private int _expressionElementIndex;

        /// <summary>
        /// Used to keep track of number of open bracket and to check for syntax-errors.
        /// </summary>
        private int _bracketDepth;

        /// <summary>
        /// Used during processing to store type of last element to be able to check for syntax-errors.
        /// </summary>
        private ElementType _lastElementType;

        /// <summary>
        /// Expression tree representing the fully processed expression.
        /// </summary>
        private Node _expressionTree;

        #endregion

        #region PublicMethods

        /// <summary>
        /// Creates an interpreter for basic mathematical expressions. Valid expressions may contain
        ///  - the mathematical operators for addition (+), subtraction (-) and multiplication (*)
        ///  - constant integer numbers
        ///  - variable names consisting of one lowercase character in the range from "a" to "z".
        ///  - brackets as needed for grouping parts of the expression
        ///  - spaces are allowed anywhere in the expression.
        /// </summary>
        /// <param name="expressionAsText">
        /// The textual mathematical expression, for example "3 + 5 *(a+b)"
        /// </param>
        public ExpressionInterpreter(string expressionAsText)
        {
            RegisterExpression(expressionAsText);
        }

        /// <summary>
        /// Initializes the interpreter for basic mathematical expressions. Valid expressions may contain
        ///  - the mathematical operators for addition (+), subtraction (-) and multiplication (*)
        ///  - constant integer numbers
        ///  - variable names consisting of one lowercase character in the range from "a" to "z".
        ///  - brackets as needed for grouping parts of the expression
        ///  - spaces are allowed anywhere in the expression.
        /// </summary>
        /// <param name="expressionAsText">
        /// The textual mathematical expression, for example "3 + 5 *(a+b)"
        /// </param>
        public void RegisterExpression(string expressionAsText)
        {
            ResetInstance();
            (expressionAsText != null).EnsureValidData("The parameter 'expressionAsText' cannot be NULL.");
            expressionAsText.EnsureOnlyValidCharacters();
            CreateExpressionTree(expressionAsText);
        }

        /// <summary>
        /// Calculate the result of the mathematical expression by using the given variable values.
        /// 
        /// </summary>
        /// <param name="valuesOfVariables">
        /// Dictionary with variables and the corresponding values of this calculation.
        /// This parameter is optional.
        /// </param>
        /// <returns>
        /// Result of the mathematical expression as integer.
        /// </returns>
        public int CalculateWith(IDictionary<Variable, int> valuesOfVariables = null)
        {
            if(valuesOfVariables == null)
            {
                valuesOfVariables = new Dictionary<Variable, int>();
            }

            return _expressionTree.Evaluate(valuesOfVariables);
        }

        #endregion

        #region PrivateParsingMethods

        /// <summary>
        /// Reset al instance variables.
        /// </summary>
        private void ResetInstance()
        {
            _expressionElementList = null;
            _expressionElementIndex = 0;
            _bracketDepth = 0;
            _lastElementType = ElementType.Undefined;
            _expressionTree = null;
        }

        /// <summary>
        /// Parses the expression and stores a corresponding expression-tree to be used for calculation.
        /// </summary>
        /// <param name="expressionAsText">
        /// Textual mathematical expression.
        /// </param>
        private void CreateExpressionTree(string expressionAsText)
        {
            _lastElementType = ElementType.Undefined;
            _expressionElementList = expressionAsText.SplitExpressionToElements();

            _expressionTree = ProcessExpressionElements();
            (_bracketDepth == 0).EnsureValidSyntax("Missing closing bracket.");
        }

        /// <summary>
        /// Merge two nodes by using the given operator.
        /// </summary>
        /// <param name="pendingNode">
        /// Pending node to be used as left node for operator node.
        /// </param>
        /// <param name="operatorType">
        /// Operator type to be use for operator node.
        /// </param>
        /// <param name="newNode">
        /// New node to be used as right node in combination with pending node.
        /// </param>
        /// <returns>
        /// If pendingNode is null then returns newNode.
        /// If pendingNode is not null then returns a new operator-node using the given operator-type.
        /// </returns>
        private Node MergeNodes(Node pendingNode, OperatorType operatorType, Node newNode)
        {
            // if there is no pending node then return the new node
            if (pendingNode == null) return newNode;

            // if there is already a pending node then there has to be an operator to combine the two nodes
            (operatorType != OperatorType.Undefined).EnsureValidSyntax($"Missing operator at position {_expressionElementIndex}.");
            return new NodeOperator(pendingNode, operatorType, newNode);
        }

        /// <summary>
        /// Creates either a variable node or a numeric node.
        /// </summary>
        /// <param name="element">
        /// String containing either a variable or a numeric constant.
        /// </param>
        /// <returns>
        /// Returns the new node.
        /// </returns>
        private static Node CreateScalarNode(string element) => element.IsVariable() ? (Node) new NodeVariable(element) : new NodeNumeric(element);

        /// <summary>
        /// Process the parts of the expression and build up a node-tree to be used for calculation.
        /// The function uses recursion to process the brackets in the expression.
        /// </summary>
        /// <returns>
        /// Returns a node representing the current (sub-)expression.
        /// </returns>
        private Node ProcessExpressionElements()
        {
            Node pendingNode = null;
            var operatorType = OperatorType.Undefined;
            var nodeStack = new List<InputStackEntry>();
            // for syntax-check: (sub-)expression cannot start with operator
            var firstElementInExpression = true;

            while(IsElementForProcessingAvailable)
            {
                var currentElement = GetCurrentElement();

                if (currentElement.IsScalar())
                {
                    EnsureElementIsValid(ElementType.Scalar);
                    ConsumeElement(ElementType.Scalar);
                    var newNode = CreateScalarNode(currentElement);
                    pendingNode = MergeNodes(pendingNode, operatorType, newNode);
                    operatorType = OperatorType.Undefined;
                }
                else if(currentElement.IsOperator())
                {
                    (!firstElementInExpression).EnsureValidSyntax("First element of expression cannot be an operator.");
                    EnsureElementIsValid(ElementType.Operator);
                    ConsumeElement(ElementType.Operator);
                    operatorType = currentElement.ToOperatorType();

                    // multiplication before addition/subtraction: 
                    // for addition/subtraction start with new node in next loop
                    if (operatorType != OperatorType.Mult)
                    {
                        nodeStack.Add(new InputStackEntry { Node = pendingNode, OperatorType = operatorType });
                        pendingNode = null;
                        operatorType = OperatorType.Undefined;
                    }
                }
                else if (currentElement.IsBracketOpening())
                {
                    EnsureElementIsValid(ElementType.Scalar);
                    EnterBracket();
                    ConsumeElement(ElementType.Undefined);
                    // brackets are handled by recursion
                    var newNode = ProcessExpressionElements();
                    pendingNode = MergeNodes(pendingNode, operatorType, newNode);
                    operatorType = OperatorType.Undefined;
                }
                else if (currentElement.IsBracketClosing())
                {
                    EnsureElementIsValid(ElementType.Operator);
                    LeaveBracket();
                    ConsumeElement(ElementType.Scalar);
                    // terminate loop to finish current recursion level
                    break;
                }

                firstElementInExpression = false;
            }

            // at the end of the loop we must have a node but no pending operator
            (operatorType == OperatorType.Undefined).EnsureValidSyntax("Missing scalar at the end of the expression.");
            (pendingNode != null).EnsureValidSyntax("Missing element in expression.");

            // build the resulting node-tree out of the stack of nodes and operators
            var resultNode = MergeNodeStackToTree(nodeStack, pendingNode);
            (resultNode != null).EnsureValidSyntax("Missing element in expression.");

            return resultNode;
        }

        /// <summary>
        /// Merge the stack of nodes together to a tree and return the resulting base node.
        /// </summary>
        /// <param name="nodeStack">
        /// Stack of node/operator-pairs.
        /// </param>
        /// <param name="pendingNode">
        /// Possibly pending node, can be null if no pending node exists.
        /// </param>
        /// <returns>
        /// Returns the base node containing all others node from the stack.
        /// </returns>
        private Node MergeNodeStackToTree(IEnumerable<InputStackEntry> nodeStack, Node pendingNode)
        {
            Node resultNode = null;
            var operatorType = OperatorType.Undefined;
            // merge the nodes together
            foreach (var stackEntry in nodeStack)
            {
                resultNode = resultNode == null ? stackEntry.Node : MergeNodes(resultNode, operatorType, stackEntry.Node);
                operatorType = stackEntry.OperatorType;
            }

            // add the last pending node
            resultNode = resultNode == null ? pendingNode : MergeNodes(resultNode, operatorType, pendingNode);

            return resultNode;
        }

        /// <summary>
        /// Checks if there is another expression element to be processed.
        /// </summary>
        /// <returns>
        /// Returns true if there is another expression element, otherwise false.
        /// </returns>
        private bool IsElementForProcessingAvailable => _expressionElementIndex < _expressionElementList.Length;

        /// <summary>
        /// Returns the current element of the expression during processing.
        /// </summary>
        /// <returns>
        /// Current element to be processed.
        /// </returns>
        private string GetCurrentElement() => _expressionElementList[_expressionElementIndex];

        /// <summary>
        /// Increments the array-index that is used while processing the expression elements.
        /// Also stores the type of the last element to be able to do syntax-checks.
        /// </summary>
        /// <param name="elementType">
        /// Type of element that just got processed.
        /// </param>
        private void ConsumeElement(ElementType elementType)
        {
            _expressionElementIndex++;
            _lastElementType = elementType;
        }

        /// <summary>
        /// Track bracket entering by incrementing the bracketDepth value.
        /// </summary>
        private void EnterBracket() => _bracketDepth++;

        /// <summary>
        /// Track bracket leaving by decrementing the bracketDepth value.
        /// Throws SyntaxException if there is currently no open bracket i.e. bracketDepth is zero.
        /// </summary>
        private void LeaveBracket()
        {
            (_bracketDepth > 0).EnsureValidSyntax($"unexpected closing bracket at position {_expressionElementIndex}");
            _bracketDepth--;
        }

        /// <summary>
        /// Check if the given element type against the type of the last element.
        /// Throws syntax exception if 
        /// </summary>
        /// <param name="elementType">
        /// Type of the current element.
        /// </param>
        private void EnsureElementIsValid(ElementType elementType)
        {
            (_lastElementType == ElementType.Undefined || _lastElementType != elementType)
                .EnsureValidSyntax($"wrong element type '{_lastElementType}' at index {_expressionElementIndex}");
        }

        #endregion

        #region PrivateExpressionTreeClasses

        /// <summary>
        /// Used to build a stack of previous nodes and operators while processing the expression.
        /// </summary>
        private class InputStackEntry
        {
            public Node Node { get; set; }

            public OperatorType OperatorType { get; set; }
        }

        #endregion
    }
}
