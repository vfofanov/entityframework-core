using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore
{
    public sealed class EntityFrameworkCoreDefinitionReader : IDefinitionReader
    {
        private readonly IModel _model;
        private readonly IEFEntityDefinitionInfo[] _entityDefinitions;
        private readonly IEFPropertyDefinitionInfo[] _propertyDefinitions;
        private readonly Func<IEntityType, bool> _filterEntities;
        private readonly Func<IEntityType, IProperty, bool> _filterProperties;

        public EntityFrameworkCoreDefinitionReader(IModel model, 
            IEFEntityDefinitionInfo[] entityDefinitions, 
            IEFPropertyDefinitionInfo[] propertyDefinitions,
            Func<IEntityType, bool>? filterEntities=null,
            Func<IEntityType, IProperty, bool>? filterProperties=null)
        {
            if (entityDefinitions.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(entityDefinitions));
            }
            if (propertyDefinitions.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(propertyDefinitions));
            }
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _entityDefinitions = entityDefinitions;
            _propertyDefinitions = propertyDefinitions;
            _filterEntities = filterEntities ?? (_ => true);
            _filterProperties = filterProperties ?? ((_, _) => true);
        }

        /// <inheritdoc />
        public DefinitionMap Read()
        {
            var context = new EFDefinitionExtractContext();
            var map = new DefinitionMap(_entityDefinitions.Select(d => d.Info).ToList(),
                _propertyDefinitions.Select(d => d.Info).ToList());

            foreach (var entityType in _model.GetEntityTypes().Where(_filterEntities))
            {
                var entityBuilder = map.Add(entityType.Name);
                foreach (var definition in _entityDefinitions)
                {
                    var val = definition.Extract(entityType, context);
                    entityBuilder.AddDefinition(definition.Info, val);
                }
                foreach (var property in entityType.GetProperties().Where(p=>_filterProperties(entityType, p)))
                {
                    var propertyBuilder = entityBuilder.AddProperty(property.Name);
                    foreach (var definition in _propertyDefinitions)
                    {
                        var val = definition.Extract(property, context);
                        propertyBuilder.AddDefinition(definition.Info, val);
                    }   
                }
            }
            return map;
        }
    }
}