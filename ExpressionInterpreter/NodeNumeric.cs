using System.Collections.Generic;

namespace ExpressionInterpreterExample
{
    /// <summary>
    /// Represents a constant numeric node.
    /// </summary>
    public class NodeNumeric : Node
    {
        private readonly int _value;

        public NodeNumeric(int value)
        {
            _value = value;
        }

        public override int Evaluate(IDictionary<Variable, int> variableValues)
        {
            return _value;
        }
    }
}
