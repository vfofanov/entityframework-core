﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stenn.EntityDefinition.Contracts;
using Stenn.EntityDefinition.Contracts.Definitions;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using static Stenn.EntityDefinition.EntityFrameworkCore.EntityFrameworkDefinitionReaderOptions;

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
        private readonly Func<IEntityType, IPropertyBase, bool> _filterOwnedTypeProperties;

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
            _filterOwnedTypeProperties= options.GetOwnedTypePropertiesFilter();
        }

        /// <inheritdoc />
        public DefinitionMap Read()
        {
            using var context = new DefinitionContext();
            var map = new DefinitionMap(_entityDefinitions.Select(d => d.Info).ToList(),
                _propertyDefinitions.Select(d => d.Info).ToList());

            foreach (var entityType in _model.GetEntityTypes().Where(t => !t.IsOwned()).Where(_filterEntities))
            {
                var entityBuilder = map.Add(entityType.Name);
                foreach (var definition in _entityDefinitions)
                {
                    var val = definition.Extract(entityType, null, context);
                    entityBuilder.AddDefinition(definition.Info, val);
                }

                ExtractProperties(entityType, entityBuilder, context, _filterProperties);
            }
            return map;
        }

        private void ExtractProperties(IEntityType entityType, EntityDefinitionRowBuilder entityBuilder, 
            DefinitionContext context, 
            Func<IEntityType, IPropertyBase, bool> filterProperties,
            string? namePrefix = null, 
            EntityFrameworkDefinitionReaderOptions excludeIgnored=ExcludeIgnoredProperties)
        {
            
            var handledProperties = new List<string>();
            foreach (var property in entityType.GetPropertiesAndNavigations().Where(p => filterProperties(entityType, p)))
            {
                handledProperties.Add(property.PropertyInfo.Name);

                if (property is Navigation navigation && navigation.TargetEntityType.IsOwned())
                {
                    ExtractProperties(navigation.TargetEntityType, entityBuilder, context,
                        _filterOwnedTypeProperties,
                        EntityDefinitionRowBuilder.ConcatenateName(namePrefix, property.Name),
                        excludeIgnored | ExcludeOwnedTypeIgnoredProperties);
                }
                else
                {
                    ExtractProperty(entityBuilder, context, namePrefix, property, property.PropertyInfo);
                }
            }

            if ((_options & excludeIgnored) == 0)
            {
                foreach (var property in entityType.ClrType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                             .Where(p => !handledProperties.Contains(p.Name)))
                {
                    ExtractProperty(entityBuilder, context, namePrefix, null, property);
                }
            }
        }

        private void ExtractProperty(EntityDefinitionRowBuilder entityBuilder, DefinitionContext context, string? namePrefix,
            IPropertyBase? property, PropertyInfo? propertyInfo)
        {
            var name = property?.Name ?? propertyInfo?.Name;
            if (name is null)
            {
                throw new ArgumentException("property");
            }

            var propertyBuilder = entityBuilder.AddProperty(name, namePrefix);
            foreach (var definition in _propertyDefinitions)
            {
                var val = definition.Extract(property, propertyInfo,
                    propertyBuilder.Row.Name,
                    entityBuilder.Row.Values.GetValueOrDefault(definition.Info), context);

                propertyBuilder.AddDefinition(definition.Info, val);
            }
        }
    }
}