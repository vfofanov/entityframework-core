using System.Collections.Generic;

namespace Stenn.EntityDefinition.Contracts
{
    public sealed class DefinitionMap : IDefinitionMap
    {
        /// <inheritdoc />
        IReadOnlyCollection<string> IDefinitionMap.ActiveEntityDefinitions => ActiveEntityDefinitions;

        /// <inheritdoc />
        IReadOnlyCollection<string> IDefinitionMap.ActivePropertyDefinitions => ActivePropertyDefinitions;

        /// <inheritdoc />
        IReadOnlyCollection<EntityDefinition> IDefinitionMap.Entities => Entities;

        internal List<string> ActiveEntityDefinitions { get; }

        internal List<string> ActivePropertyDefinitions { get; }

        internal List<EntityDefinition> Entities { get; }

        public DefinitionMap()
        {
            ActiveEntityDefinitions = new List<string>();
            ActivePropertyDefinitions = new List<string>();
            Entities = new List<EntityDefinition>();
        }

        public EntityDefinitionBuilder Add(string name)
        {
            var entityDefinition = new EntityDefinition(name, ActiveEntityDefinitions.Count);
            Entities.Add(entityDefinition);

            return new EntityDefinitionBuilder(this, entityDefinition);
        }
    }
}