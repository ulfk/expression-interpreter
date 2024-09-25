using System.Collections.Generic;

namespace ExpressionInterpreterLib
{
    public interface IExpressionInterpreter
    {
        void RegisterExpression(string expressionAsText);

        int CalculateWith(IDictionary<Variable, int> valuesOfVariables = null);
    }
}