using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.EntityFrameworkCore.Definitions;
using static Stenn.EntityDefinition.EntityFrameworkCore.EntityFrameworkDefinitionReaderOptions;

namespace Stenn.EntityDefinition.EntityFrameworkCore
{
    public sealed class EntityFrameworkCoreDefinitionReaderOptions
    {
        private readonly List<IEFEntityDefinition> _entityDefinitions = new();
        private readonly List<IEFPropertyDefinition> _propertyDefinitions = new();
        private Func<IEntityType, bool>? _filterEntities;
        private Func<IEntityType, IPropertyBase, bool>? _filterProperties;

        public EntityFrameworkDefinitionReaderOptions ReaderOptions { get; set; } = ExcludeOwnedTypeShadowProperties |
                                                                                    ExcludeOwnedTypeIgnoredProperties;

        public void SetEntitiesFilter(Func<IEntityType, bool>? filter)
        {
            _filterEntities = filter;
        }

        public void SetPropertiesFilter(Func<IEntityType, IPropertyBase, bool>? filter)
        {
            _filterProperties = filter;
        }

        internal void AddEntityDefinition(IEFEntityDefinition definition)
        {
            _entityDefinitions.Add(definition);
        }

        internal void AddPropertyDefinition(IEFPropertyDefinition definition)
        {
            _propertyDefinitions.Add(definition);
        }

        internal Func<IEntityType, bool> GetEntitiesFilter()
        {
            var filter = _filterEntities ?? (_ => true);
            if (ReaderOptions.HasFlag(ExcludeAbstractEntities))
            {
                var tempFilter = filter;
                filter = type => !type.IsAbstract() && tempFilter(type);
            }
            return filter;
        }

        internal Func<IEntityType, IPropertyBase, bool> GetPropertiesFilter()
        {
            var filter = _filterProperties ?? ((_, _) => true);
            if (ReaderOptions.HasFlag(ExcludeScalarProperties))
            {
                var tempFilter = filter;
                filter = (t, p) => p is not IProperty && tempFilter(t, p);
            }
            if (ReaderOptions.HasFlag(ExcludeRelationNavigationProperties))
            {
                var tempFilter = filter;
                filter = (t, p) => (p is not INavigation n || n.TargetEntityType.IsOwned()) && tempFilter(t, p);
            }
            if (ReaderOptions.HasFlag(ExcludeOwnedNavigationProperties))
            {
                var tempFilter = filter;
                filter = (t, p) => (p is not INavigation n || !n.TargetEntityType.IsOwned()) && tempFilter(t, p);
            }
            if (ReaderOptions.HasFlag(ExcludeShadowProperties))
            {
                var tempFilter = filter;
                filter = (t, p) => !p.IsShadowProperty() && tempFilter(t, p);
            }
            return filter;
        }

        public Func<IEntityType, IPropertyBase, bool> GetOwnedTypePropertiesFilter()
        {
            var filter = GetPropertiesFilter();
            if (ReaderOptions.HasFlag(ExcludeOwnedTypeShadowProperties))
            {
                var tempFilter = filter;
                filter = (t, p) => !p.IsShadowProperty() && tempFilter(t, p);
            }
            return filter;
        }

        internal IEFEntityDefinition[] GetEntityDefinitions()
        {
            return _entityDefinitions.OrderBy(d => d.ReadOrder).ToArray();
        }

        internal IEFPropertyDefinition[] GetPropertyDefinitions()
        {
            return _propertyDefinitions.OrderBy(d => d.ReadOrder).ToArray();
        }
    }
}