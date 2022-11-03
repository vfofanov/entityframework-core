using System.Collections.Generic;
using System.Linq;

namespace Stenn.EntityDefinition.Contracts
{
    public class DefinitionBuilder<T>
        where T : DefinitionBase
    {
        internal ICollection<string> ActiveMap { get; }

        public DefinitionBuilder(ICollection<string> activeMap, T obj)
        {
            ActiveMap = activeMap;
            Obj = obj;
        }

        protected T Obj { get; }

        public void AddDefinition(DefinitionAttribute definition)
        {
            AddDefinition(definition.Name, definition.Value);
        }

        public void AddDefinition(string name, object value)
        {
            if (ActiveMap.SingleOrDefault(v => v == name) is null)
            {
                ActiveMap.Add(name);
            }

            Obj.Values.Add(name, value);
        }
    }
}