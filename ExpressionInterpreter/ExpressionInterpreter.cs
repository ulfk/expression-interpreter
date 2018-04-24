using System.Collections.Generic;

namespace worksample
{
    public class ExpressionInterpreter
    {
        #region PrivateMemberVariablesAndConstants

        /// <summary>
        /// Used during processing and holds all expression elements as single entries.
        /// </summary>
        private string[] expressionElementList;

        /// <summary>
        /// Used during processing and is the current index to the array expressionElementList.
        /// </summary>
        private int expressionElementIndex;

        /// <summary>
        /// Used to keep track of number of open bracket and to check for syntax-errors.
        /// </summary>
        private int bracketDepth;

        /// <summary>
        /// Used during processing to store type of last element to be able to check for syntax-errors.
        /// </summary>
        private ElementType lastElementType;

        /// <summary>
        /// Expression tree representring the fully processed expresion.
        /// </summary>
        private Node expressionTree;

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
            AssertHelper.FormatAssert(expressionAsText != null, "The parameter 'expressionAsText' cannot be NULL.");
            SyntaxHelper.CheckForValidCharaters(expressionAsText);
            this.CreateExpressionTree(expressionAsText);
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

            return this.expressionTree.Evaluate(valuesOfVariables);
        }

        #endregion

        #region PrivateParsingMethods

        /// <summary>
        /// Parses the expression and stores a corresponding expression-tree to be used for calcualtion.
        /// </summary>
        /// <param name="expressionAsText">
        /// Textual mathematical expression.
        /// </param>
        private void CreateExpressionTree(string expressionAsText)
        {
            this.lastElementType = ElementType.Undefined;
            this.expressionElementList = SyntaxHelper.SplitExpressionToElements(expressionAsText);

            this.expressionTree = ProcessExpressionElements();
            AssertHelper.SyntaxAssert(this.bracketDepth == 0, "Missing closing bracket.");
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
            if(pendingNode != null)
            {
                // if there is already a pending node then there has to be an operator to combine the two nodes
                AssertHelper.SyntaxAssert(operatorType != OperatorType.Undefined, 
                             $"Missing operator at position {this.expressionElementIndex}.");
                return new NodeOperator(pendingNode, operatorType, newNode);
            }

            // if there is no pending node then return the new node
            return newNode;
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
        private Node CreateScalarNode(string element)
        {
            if (SyntaxHelper.IsVariable(element))
            {
                return new NodeVariable(element);
            }
            else
            {
                var numericValue = int.Parse(element);
                return new NodeNumeric(numericValue);
            }
        }

        /// <summary>
        /// Process the parts of the expression and build up a node-tree to be used for calculation.
        /// The function uses recursion the process the bracket in the expression.
        /// </summary>
        /// <returns>
        /// Returns a node representing the current (sub-)expression.
        /// </returns>
        private Node ProcessExpressionElements()
        {
            Node pendingNode = null;
            OperatorType operatorType = OperatorType.Undefined;
            var nodeStack = new List<InputStackEntry>();
            // for syntax-check: (sub-)expression cannot start with operator
            var firstElementInExpression = true;

            while(this.IsElementForProcessingAvailable())
            {
                var currentElement = this.GetCurrentElement();

                if (SyntaxHelper.IsScalar(currentElement))
                {
                    SyntaxAssert(ElementType.Scalar);
                    this.ConsumeElement(ElementType.Scalar);
                    var newNode = this.CreateScalarNode(currentElement);
                    pendingNode = this.MergeNodes(pendingNode, operatorType, newNode);
                    operatorType = OperatorType.Undefined;
                }
                else if(SyntaxHelper.IsOperator(currentElement))
                {
                    AssertHelper.SyntaxAssert(!firstElementInExpression,
                                 "First element of expression cannot be an operator.");
                    SyntaxAssert(ElementType.Operator);
                    this.ConsumeElement(ElementType.Operator);
                    operatorType = SyntaxHelper.GetOperatorType(currentElement);

                    // multiplication before addition/substraction: 
                    // for addition/substraction start with new node in next loop
                    if (operatorType != OperatorType.Mult)
                    {
                        nodeStack.Add(new InputStackEntry { Node = pendingNode,
                                                            OperatorType = operatorType });
                        pendingNode = null;
                        operatorType = OperatorType.Undefined;
                    }
                }
                else if (SyntaxHelper.IsBracketOpening(currentElement))
                {
                    SyntaxAssert(ElementType.Scalar);
                    this.EnterBracket();
                    this.ConsumeElement(ElementType.Undefined);
                    // brackets are handled by recursion
                    var newNode = this.ProcessExpressionElements();
                    pendingNode = this.MergeNodes(pendingNode, operatorType, newNode);
                    operatorType = OperatorType.Undefined;
                }
                else if (SyntaxHelper.IsBracketClosing(currentElement))
                {
                    SyntaxAssert(ElementType.Operator);
                    this.LeaveBracket();
                    this.ConsumeElement(ElementType.Scalar);
                    // terminate loop to finish current recursion level
                    break;
                }

                firstElementInExpression = false;
            }

            // at the end of the loop we must have a node but no pending operator
            AssertHelper.SyntaxAssert(operatorType == OperatorType.Undefined, "Missing scalar at the end of the expression.");
            AssertHelper.SyntaxAssert(pendingNode != null, "Missing element in expression.");

            // build the resulting node-tree out of the stack of nodes and operators
            var resultNode = this.MergeNodeStackToTree(nodeStack, pendingNode);
            AssertHelper.SyntaxAssert(resultNode != null, "Missing element in expression.");

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
        private Node MergeNodeStackToTree(List<InputStackEntry> nodeStack, Node pendingNode)
        {
            Node resultNode = null;
            var operatorType = OperatorType.Undefined;
            // merge the nodes together
            foreach (var stackEntry in nodeStack)
            {
                if (resultNode == null)
                {
                    resultNode = stackEntry.Node;
                }
                else
                {
                    resultNode = this.MergeNodes(resultNode, operatorType, stackEntry.Node);
                }

                operatorType = stackEntry.OperatorType;
            }

            // add the last pending node
            if (resultNode == null)
            {
                resultNode = pendingNode;
            }
            else
            {
                resultNode = this.MergeNodes(resultNode, operatorType, pendingNode);
            }

            return resultNode;
        }

        /// <summary>
        /// Checks if there is another expression element to be processed.
        /// </summary>
        /// <returns>
        /// Returns true if there is another expression element, otherwise false.
        /// </returns>
        private bool IsElementForProcessingAvailable()
        {
            return this.expressionElementIndex < this.expressionElementList.Length;
        }

        /// <summary>
        /// Returns the current element of the expression during processing.
        /// </summary>
        /// <returns>
        /// Current element to be processed.
        /// </returns>
        private string GetCurrentElement()
        {
            return this.expressionElementList[this.expressionElementIndex];
        }

        /// <summary>
        /// Increments the array-index that is used while processing the expression elements.
        /// Also stores the type of the last element to be able to do syntax-checks.
        /// </summary>
        /// <param name="elementType">
        /// Type of element that just got processed.
        /// </param>
        private void ConsumeElement(ElementType elementType)
        {
            this.expressionElementIndex++;
            this.lastElementType = elementType;
        }

        /// <summary>
        /// Track bracket entering by incrementing the bracketDepth value.
        /// </summary>
        private void EnterBracket()
        {
            this.bracketDepth++;
        }

        /// <summary>
        /// Track bracket leaving by decrementing the bracketDepth value.
        /// Throws SyntaxException if there is currently no open braket i.e. bracketDepth is zero.
        /// </summary>
        private void LeaveBracket()
        {
            AssertHelper.SyntaxAssert(this.bracketDepth > 0, $"unexpected closing bracket at position {this.expressionElementIndex}");
            this.bracketDepth--;
        }

        /// <summary>
        /// Check if the given element type against the type of the last element.
        /// Throws syntax exception if 
        /// </summary>
        /// <param name="elementType">
        /// Type of the current element.
        /// </param>
        private void SyntaxAssert(ElementType elementType)
        {
            AssertHelper.SyntaxAssert(this.lastElementType == ElementType.Undefined || this.lastElementType != elementType,
                                      $"wrong element type '{this.lastElementType}' at index {this.expressionElementIndex}");
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
