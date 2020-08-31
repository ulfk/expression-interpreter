using System.Collections.Generic;

namespace ExpressionInterpreterExample
{
    public interface IExpressionInterpreter
    {
        void RegisterExpression(string expressionAsText);

        int CalculateWith(IDictionary<Variable, int> valuesOfVariables = null);
    }
}