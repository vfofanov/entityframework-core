using System.Collections.Generic;

namespace Stenn.EntityDefinition.Contracts
{
    public interface IDefinitionMap
    {
        IReadOnlyCollection<string> ActiveEntityDefinitions { get; }
        IReadOnlyCollection<string> ActivePropertyDefinitions { get; }
        IReadOnlyCollection<EntityDefinition> Entities { get; }
    }
}