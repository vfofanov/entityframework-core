using System.Collections.Generic;

namespace Stenn.EntityDefinition.Contracts
{
    public interface IDefinitionMap
    {
        IReadOnlyCollection<DefinitionInfo> EntityDefinitions { get; }
        IReadOnlyCollection<DefinitionInfo> PropertyDefinitions { get; }
        IReadOnlyCollection<EntityDefinition> Entities { get; }
    }
}