using System;
using System.Collections.Generic;

namespace ExpressionInterpreterExample
{
    /// <summary>
    /// Represents a numerical operation between two other nodes.
    /// </summary>
    public class NodeOperator : Node
    {
        private readonly Node _leftNode;
        private readonly Node _rightNode;
        private readonly OperatorType _operatorType;

        public NodeOperator(Node leftNode, OperatorType operatorType, Node rightNode)
        {
            _leftNode = leftNode ?? throw new ArgumentNullException(nameof(leftNode));
            _rightNode = rightNode ?? throw new ArgumentNullException(nameof(rightNode));
            _operatorType = operatorType;
        }

        public override int Evaluate(IDictionary<Variable, int> variableValues)
        {
            switch (_operatorType)
            {
                case OperatorType.Add:
                    return _leftNode.Evaluate(variableValues) + _rightNode.Evaluate(variableValues);
                case OperatorType.Sub:
                    return _leftNode.Evaluate(variableValues) - _rightNode.Evaluate(variableValues);
                case OperatorType.Mult:
                    return _leftNode.Evaluate(variableValues) * _rightNode.Evaluate(variableValues);
                default:
                    throw new DataException($"Program error: Unknown operator type '{_operatorType}'");
            }
        }
    }
}
