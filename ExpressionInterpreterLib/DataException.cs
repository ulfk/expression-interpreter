using System;

namespace ExpressionInterpreterLib
{
    public class DataException : Exception
    {
        /// <summary>
        /// Exception thrown in case of data for format errors.
        /// </summary>
        /// <param name="message">
        /// Detailed error description.
        /// </param>
        public DataException(string message)
            : base($"Data or Format error: {message}")
        {
        }
    }
}
