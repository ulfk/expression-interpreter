using System;

namespace worksample
{
    public class SyntaxException : Exception
    {
        /// <summary>
        /// Exception thrown in case of syntax errors. 
        /// </summary>
        /// <param name="message">
        /// Detailed error description.
        /// </param>
        public SyntaxException(string message)
            : base($"Syntax error: {message}")
        {
        }
    }
}
