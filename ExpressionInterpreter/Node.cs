using System.Collections.Generic;

namespace worksample
{
    /// <summary>
    /// Base class for node in expression tree.
    /// </summary>
    abstract class Node
    {
        public abstract int Evaluate(IDictionary<Variable, int> variableValues);
    }
}
