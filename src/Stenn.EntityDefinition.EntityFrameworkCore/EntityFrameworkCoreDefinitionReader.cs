using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore
{
    public sealed class EntityFrameworkCoreDefinitionReader : IDefinitionReader
    {
        private readonly IModel _model;
        private readonly IEFEntityDefinitionInfo[] _entityDefinitions;
        private readonly IEFPropertyDefinitionInfo[] _propertyDefinitions;
        private readonly EntityFrameworkCoreDefinitionReaderOptions _options;
        private readonly Func<IEntityType, bool> _filterEntities;
        private readonly Func<IEntityType, IPropertyBase, bool> _filterProperties;

        public EntityFrameworkCoreDefinitionReader(IModel model,
            IEFEntityDefinitionInfo[] entityDefinitions,
            IEFPropertyDefinitionInfo[] propertyDefinitions,
            Func<IEntityType, bool>? filterEntities = null,
            Func<IEntityType, IPropertyBase, bool>? filterProperties = null,
            EntityFrameworkCoreDefinitionReaderOptions options = EntityFrameworkCoreDefinitionReaderOptions.None)
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
            _options = options;
            _filterEntities = filterEntities ?? (_ => true);
            if (_options.HasFlag(EntityFrameworkCoreDefinitionReaderOptions.ExcludeAbstractEntities))
            {
                var tempFilter = _filterEntities;
                _filterEntities = type => !type.IsAbstract() && tempFilter(type);
            }
            
            _filterProperties = filterProperties ?? ((_, _) => true);
            if (_options.HasFlag(EntityFrameworkCoreDefinitionReaderOptions.ExcludeScalarProperties))
            {
                var tempFilter = _filterProperties;
                _filterProperties = (t, p) => p is not IProperty  && tempFilter(t, p);
            }
            if (_options.HasFlag(EntityFrameworkCoreDefinitionReaderOptions.ExcludeNavigationProperties))
            {
                var tempFilter = _filterProperties;
                _filterProperties = (t, p) => p is not INavigation  && tempFilter(t, p);
            }
            if (_options.HasFlag(EntityFrameworkCoreDefinitionReaderOptions.ExcludeShadowProperties))
            {
                var tempFilter = _filterProperties;
                _filterProperties = (t, p) => !p.IsShadowProperty() && tempFilter(t, p);
            }

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

                var handledProperties = new List<PropertyInfo>();
                foreach (var property in entityType.GetPropertiesAndNavigations().Where(p => _filterProperties(entityType, p)))
                {
                    var propertyBuilder = entityBuilder.AddProperty(property.Name);
                    foreach (var definition in _propertyDefinitions)
                    {
                        var val = definition.Extract(property, property.PropertyInfo, context);
                        propertyBuilder.AddDefinition(definition.Info, val);
                        handledProperties.Add(property.PropertyInfo);
                    }
                }

                if (!_options.HasFlag(EntityFrameworkCoreDefinitionReaderOptions.ExcludeIgnoredProperties))
                {
                    foreach (var property in entityType.ClrType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                 .Where(p => !handledProperties.Contains(p)))
                    {
                        var propertyBuilder = entityBuilder.AddProperty(property.Name);
                        foreach (var definition in _propertyDefinitions)
                        {
                            var val = definition.Extract(null, property, context);
                            propertyBuilder.AddDefinition(definition.Info, val);
                        }
                    }
                }
            }
            return map;
        }
    }

    [Flags]
    public enum EntityFrameworkCoreDefinitionReaderOptions
    {
        None = 0,
        ExcludeIgnoredProperties = 0x01,
        ExcludeScalarProperties = 0x02,
        ExcludeNavigationProperties = 0x04,
        ExcludeAbstractEntities = 0x08,
        ExcludeShadowProperties = 0x10
    }
}