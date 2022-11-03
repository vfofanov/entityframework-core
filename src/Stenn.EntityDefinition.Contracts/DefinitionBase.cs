using System.Collections.Generic;
using System.Diagnostics;

namespace Stenn.EntityDefinition.Contracts
{
    [DebuggerDisplay("{Name}")]
    public abstract class DefinitionBase
    {
        protected DefinitionBase(string name, int valuesCount)
        {
            Name = name;
            Values = new Dictionary<DefinitionInfo, object?>(valuesCount);
        }

        public string Name { get; }
        public Dictionary<DefinitionInfo, object?> Values { get; }
    }
}