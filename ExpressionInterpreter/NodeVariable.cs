using System.Collections.Generic;
using System.Linq;

namespace worksample
{
    /// <summary>
    /// Represents a node containing a variable.
    /// </summary>
    class NodeVariable : Node
    {
        private readonly Variable name;

        public NodeVariable(string name)
        {
            this.name = new Variable(name);
        }

        public override int Evaluate(IDictionary<Variable, int> variableValues)
        {
            var entry = variableValues.Where(v => v.Key.HasSameNameAs(this.name)).FirstOrDefault();
            if (entry.Equals(default(KeyValuePair<Variable, int>)))
            {
                throw new DataException($"No value found for variable '{this.name}'");
            }

            return entry.Value;
        }
    }
}
