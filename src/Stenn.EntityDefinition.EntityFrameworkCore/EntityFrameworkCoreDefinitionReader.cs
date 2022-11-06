using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore
{
    public sealed class EntityFrameworkCoreDefinitionReader : IDefinitionReader
    {
        private readonly IModel _model;
        private readonly IEFEntityDefinition[] _entityDefinitions;
        private readonly IEFPropertyDefinition[] _propertyDefinitions;
        private readonly EntityFrameworkDefinitionReaderOptions _options;
        private readonly Func<IEntityType, bool> _filterEntities;
        private readonly Func<IEntityType, IPropertyBase, bool> _filterProperties;

        public EntityFrameworkCoreDefinitionReader(IModel model,
            EntityFrameworkCoreDefinitionReaderOptions options)
        {
            var entityDefinitions = options.GetEntityDefinitions();
            var propertyDefinitions = options.GetPropertyDefinitions();

            if (entityDefinitions.Length == 0 && propertyDefinitions.Length == 0)
            {
                throw new ArgumentException("Definitions' list is empty.", nameof(options));
            }
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _entityDefinitions = entityDefinitions;
            _propertyDefinitions = propertyDefinitions;
            _options = options.ReaderOptions;
            _filterEntities = options.GetEntitiesFilter();
            _filterProperties = options.GetPropertiesFilter();
        }

        /// <inheritdoc />
        public DefinitionMap Read()
        {
            using var context = new DefinitionContext();
            var map = new DefinitionMap(_entityDefinitions.Select(d => d.Info).ToList(),
                _propertyDefinitions.Select(d => d.Info).ToList());

            foreach (var entityType in _model.GetEntityTypes().Where(_filterEntities))
            {
                var entityBuilder = map.Add(entityType.Name);
                foreach (var definition in _entityDefinitions)
                {
                    var val = definition.Extract(entityType, null, context);
                    entityBuilder.AddDefinition(definition.Info, val);
                }

                var entityRow = entityBuilder.Row;
                var handledProperties = new List<PropertyInfo>();
                foreach (var property in entityType.GetPropertiesAndNavigations().Where(p => _filterProperties(entityType, p)))
                {
                    var propertyBuilder = entityBuilder.AddProperty(property.Name);
                    foreach (var definition in _propertyDefinitions)
                    {
                        var val = definition.Extract(property, property.PropertyInfo,
                            entityRow.Values.GetValueOrDefault(definition.Info), context);

                        propertyBuilder.AddDefinition(definition.Info, val);
                        handledProperties.Add(property.PropertyInfo);
                    }
                }

                if (!_options.HasFlag(EntityFrameworkDefinitionReaderOptions.ExcludeIgnoredProperties))
                {
                    foreach (var property in entityType.ClrType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                 .Where(p => !handledProperties.Contains(p)))
                    {
                        var propertyBuilder = entityBuilder.AddProperty(property.Name);
                        foreach (var definition in _propertyDefinitions)
                        {
                            var val = definition.Extract(null, property,
                                entityRow.Values.GetValueOrDefault(definition.Info), context);

                            propertyBuilder.AddDefinition(definition.Info, val);
                        }
                    }
                }
            }
            return map;
        }
    }
}