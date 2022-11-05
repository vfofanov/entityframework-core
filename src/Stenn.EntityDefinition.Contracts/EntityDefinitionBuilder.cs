namespace Stenn.EntityDefinition.Contracts
{
    public sealed class EntityDefinitionRowBuilder : DefinitionBuilder<EntityDefinitionRow>
    {
        private readonly DefinitionMap _map;

        /// <inheritdoc />
        public EntityDefinitionRowBuilder(DefinitionMap map, EntityDefinitionRow row)
            : base(row)
        {
            _map = map;
        }

        public DefinitionBuilder<PropertyDefinitionRow> AddProperty(string name)
        {
            var property = new PropertyDefinitionRow(name, _map.PropertyDefinitions.Count);
            base.Row.AddProperty(property);

            return new DefinitionBuilder<PropertyDefinitionRow>(property);
        }
    }
}