using System.Collections.Generic;

namespace worksample
{
    /// <summary>
    /// Represents a constant numeric node.
    /// </summary>
    class NodeNumeric : Node
    {
        private readonly int value;

        public NodeNumeric(int value)
        {
            this.value = value;
        }

        public override int Evaluate(IDictionary<Variable, int> variableValues)
        {
            return this.value;
        }
    }
}
