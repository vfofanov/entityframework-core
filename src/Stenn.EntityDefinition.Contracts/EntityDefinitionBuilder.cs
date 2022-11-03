namespace Stenn.EntityDefinition.Contracts
{
    public sealed class EntityDefinitionBuilder : DefinitionBuilder<EntityDefinition>
    {
        private readonly DefinitionMap _map;

        /// <inheritdoc />
        public EntityDefinitionBuilder(DefinitionMap map, EntityDefinition obj)
            : base(obj)
        {
            _map = map;
        }

        public DefinitionBuilder<PropertyDefinition> AddProperty(string name)
        {
            var property = new PropertyDefinition(name, _map.PropertyDefinitions.Count);
            Obj.AddProperty(property);

            return new DefinitionBuilder<PropertyDefinition>(property);
        }
    }
}