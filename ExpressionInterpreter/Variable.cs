using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace worksample
{
    /// <summary>
    /// Represents a variable by its name.
    /// </summary>
    public class Variable
    {
        public Variable(char name)
        {
            this.Name = name;
        }

        public Variable(string name)
        {
            if (name.Length == 1)
            {
                this.Name = name[0];
            }
            else
            {
                throw new DataException($"Invalid variable name: '{name}'");
            }
        }

        public char Name { get; private set; }

        public bool HasSameNameAs(Variable variableToCompareWith)
        {
            return this.Name == variableToCompareWith.Name;
        }
    }
}
