using System.Collections.Generic;

namespace worksample
{
    /// <summary>
    /// Represents a numerical operation between two other nodes.
    /// </summary>
    class NodeOperator : Node
    {
        private readonly Node leftNode;
        private readonly Node rightNode;
        private readonly OperatorType operatorType;

        public NodeOperator(Node leftNode, OperatorType operatorType, Node rightNode)
        {
            this.leftNode = leftNode;
            this.rightNode = rightNode;
            this.operatorType = operatorType;
        }

        public override int Evaluate(IDictionary<Variable, int> variableValues)
        {
            switch (this.operatorType)
            {
                case OperatorType.Add:
                    return leftNode.Evaluate(variableValues) + rightNode.Evaluate(variableValues);
                case OperatorType.Sub:
                    return leftNode.Evaluate(variableValues) - rightNode.Evaluate(variableValues);
                case OperatorType.Mult:
                    return leftNode.Evaluate(variableValues) * rightNode.Evaluate(variableValues);
                default:
                    throw new DataException($"Program error: Unknown operator type '{this.operatorType}'");
            }
        }
    }
}
