using System.Collections.Generic;

namespace Stenn.EntityDefinition.Contracts
{
    public sealed class DefinitionMap : IDefinitionMap
    {
        /// <inheritdoc />
        IReadOnlyCollection<EntityDefinition> IDefinitionMap.Entities => Entities;

        public IReadOnlyCollection<DefinitionInfo> EntityDefinitions { get; }

        public IReadOnlyCollection<DefinitionInfo> PropertyDefinitions { get; }

        internal List<EntityDefinition> Entities { get; }

        public DefinitionMap(IReadOnlyCollection<DefinitionInfo> entityDefinitions,
            IReadOnlyCollection<DefinitionInfo> propertyDefinitions)
        {
            EntityDefinitions = entityDefinitions;
            PropertyDefinitions = propertyDefinitions;
            Entities = new List<EntityDefinition>();
        }

        public EntityDefinitionBuilder Add(string name)
        {
            var entityDefinition = new EntityDefinition(name, EntityDefinitions.Count);
            Entities.Add(entityDefinition);

            return new EntityDefinitionBuilder(this, entityDefinition);
        }
    }
}