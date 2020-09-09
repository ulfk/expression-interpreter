using System.Collections.Generic;

namespace ExpressionInterpreterExample
{
    /// <summary>
    /// Represents a constant numeric node.
    /// </summary>
    public class NodeNumeric : Node
    {
        private readonly int _value;

        public NodeNumeric(string value)
        {
            var numericValue = int.Parse(value);
            _value = numericValue;
        }

        public override int Evaluate(IDictionary<Variable, int> variableValues)
        {
            return _value;
        }
    }
}
