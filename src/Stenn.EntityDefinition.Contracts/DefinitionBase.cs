using System.Collections.Generic;

namespace Stenn.EntityDefinition.Contracts
{
    public abstract class DefinitionBase
    {
        protected DefinitionBase(string name, int valuesCount)
        {
            Name = name;
            Values = new Dictionary<string, object>(valuesCount);
        }

        public string Name { get; }
        public Dictionary<string, object> Values { get; }
    }
}