
namespace ExpressionInterpreterExample
{
    /// <summary>
    /// Represents a variable by its name.
    /// </summary>
    public class Variable
    {
        public Variable(string name)
        {
            if (name.Length == 1)
            {
                Name = name[0];
            }
            else
            {
                throw new DataException($"Invalid variable name: '{name}'");
            }
        }

        public char Name { get; }

        public bool HasSameNameAs(Variable variableToCompareWith)
        {
            return Name == variableToCompareWith.Name;
        }
    }
}
