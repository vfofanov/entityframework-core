using System.Collections.Generic;

namespace Stenn.EntityDefinition.Contracts
{
    public sealed class EntityDefinition : DefinitionBase
    {
        private readonly List<PropertyDefinition> _properties;

        /// <inheritdoc />
        public EntityDefinition(string name, int valuesCount)
            : base(name, valuesCount)
        {
            _properties = new List<PropertyDefinition>();
        }

        public IReadOnlyCollection<PropertyDefinition> Properties => _properties;

        internal void AddProperty(PropertyDefinition property)
        {
            _properties.Add(property);
        }
    }
}