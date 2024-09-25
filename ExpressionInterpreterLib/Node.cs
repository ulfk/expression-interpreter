using System.Collections.Generic;

namespace ExpressionInterpreterLib
{
    /// <summary>
    /// Base class for node in expression tree.
    /// </summary>
    public abstract class Node
    {
        public abstract int Evaluate(IDictionary<Variable, int> variableValues);
    }
}
