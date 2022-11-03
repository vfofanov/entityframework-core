namespace Stenn.EntityDefinition.Contracts
{
    public sealed class EntityDefinitionBuilder : Builder<EntityDefinition>
    {
        private readonly DefinitionMap _map;

        /// <inheritdoc />
        public EntityDefinitionBuilder(DefinitionMap map, EntityDefinition obj)
            : base(map.ActiveEntityDefinitions, obj)
        {
            _map = map;
        }

        public Builder<PropertyDefinition> AddProperty(string name)
        {
            var property = new PropertyDefinition(name, _map.ActivePropertyDefinitions.Count);
            Obj.AddProperty(property);

            return new Builder<PropertyDefinition>(_map.ActivePropertyDefinitions, property);
        }
    }
}