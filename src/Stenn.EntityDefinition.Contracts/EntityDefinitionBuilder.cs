namespace Stenn.EntityDefinition.Contracts
{
    public sealed class EntityDefinitionBuilder : DefinitionBuilder<EntityDefinitionRow>
    {
        private readonly DefinitionMap _map;

        /// <inheritdoc />
        public EntityDefinitionBuilder(DefinitionMap map, EntityDefinitionRow obj)
            : base(obj)
        {
            _map = map;
        }

        public DefinitionBuilder<PropertyDefinitionRow> AddProperty(string name)
        {
            var property = new PropertyDefinitionRow(name, _map.PropertyDefinitions.Count);
            Obj.AddProperty(property);

            return new DefinitionBuilder<PropertyDefinitionRow>(property);
        }
    }
}