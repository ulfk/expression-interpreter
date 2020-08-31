
namespace ExpressionInterpreterExample
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Throws syntax-exception if condition is not true. Adds 'message' to to the exception-message.
        /// </summary>
        /// <param name="condition">
        /// Condition to be checked.
        /// </param>
        /// <param name="message">
        /// Message to be added to the exception message.
        /// </param>
        public static void EnsureValidData(this bool condition, string message)
        {
            if(!condition)
                throw new DataException(message);
        }

        /// <summary>
        /// Throws format-exception if assertCondition is not true. Adds 'message' to to the exception-message.
        /// </summary>
        /// <param name="condition">
        /// Condition to be checked.
        /// </param>
        /// <param name="message">
        /// Message to be added to the exception message.
        /// </param>
        public static void EnsureValidSyntax(this bool condition, string message)
        {
            if (!condition)
                throw new SyntaxException(message);
        }
    }
}
