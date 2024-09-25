using System;
using System.Collections.Generic;

namespace ExpressionInterpreterLib
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
            return _operatorType switch
            {
                OperatorType.Add => _leftNode.Evaluate(variableValues) + _rightNode.Evaluate(variableValues),
                OperatorType.Sub => _leftNode.Evaluate(variableValues) - _rightNode.Evaluate(variableValues),
                OperatorType.Mult => _leftNode.Evaluate(variableValues) * _rightNode.Evaluate(variableValues),
                _ => throw new DataException($"Program error: Unknown operator type '{_operatorType}'")
            };
        }
    }
}
