﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionInterpreterExample
{
    /// <summary>
    /// Represents a node containing a variable.
    /// </summary>
    public class NodeVariable : Node
    {
        private readonly Variable _name;

        public NodeVariable(string name)
        {
            _name = new Variable(name);
        }

        public override int Evaluate(IDictionary<Variable, int> variableValues)
        {
            if(variableValues == null) throw new ArgumentNullException(nameof(variableValues));

            var entry = variableValues.FirstOrDefault(v => v.Key.IsSameAs(_name));
            if (entry.Equals(default(KeyValuePair<Variable, int>)))
            {
                throw new DataException($"No value found for variable '{_name}'");
            }

            return entry.Value;
        }
    }
}
