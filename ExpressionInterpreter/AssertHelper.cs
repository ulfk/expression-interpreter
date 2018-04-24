using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace worksample
{
    class AssertHelper
    {

        /// <summary>
        /// Throws syntax-exception if condition is not true. Adds 'message' to to the exception-message.
        /// </summary>
        /// <param name="assertCondition">
        /// Condition to be checked.
        /// </param>
        /// <param name="message">
        /// Message to be added to the exception message.
        /// </param>
        public static void SyntaxAssert(bool assertCondition, string message)
        {
            if (!assertCondition)
            {
                throw new SyntaxException(message);
            }
        }

        /// <summary>
        /// Throws format-exception if assertCondition is not true. Adds 'message' to to the exception-message.
        /// </summary>
        /// <param name="assertCondition">
        /// Condition to be checked.
        /// </param>
        /// <param name="message">
        /// Message to be added to the exception message.
        /// </param>
        public static void FormatAssert(bool assertCondition, string message)
        {
            if (!assertCondition)
            {
                throw new DataException(message);
            }
        }
    }
}
