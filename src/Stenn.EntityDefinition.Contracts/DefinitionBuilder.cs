using System.Collections.Generic;
using System.Linq;

namespace Stenn.EntityDefinition.Contracts
{
    public class DefinitionBuilder<T>
        where T : DefinitionBase
    {
        public DefinitionBuilder(T obj)
        {
            Obj = obj;
        }

        protected T Obj { get; }

        public void AddDefinition(DefinitionInfo info, object? value)
        {
            Obj.Values.Add(info, value);
        }
    }
}