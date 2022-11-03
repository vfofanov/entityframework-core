using System;
using System.Collections.Generic;
using System.Linq;

namespace Stenn.EntityDefinition.Contracts
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Property)]
    public abstract class DefinitionAttribute : Attribute
    {
        protected DefinitionAttribute(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }
    }

    public sealed class DefinitionMap : IDefinitionMap
    {
        /// <inheritdoc />
        IReadOnlyCollection<string> IDefinitionMap.ActiveEntityDefinitions => ActiveEntityDefinitions;

        /// <inheritdoc />
        IReadOnlyCollection<string> IDefinitionMap.ActivePropertyDefinitions => ActivePropertyDefinitions;

        /// <inheritdoc />
        IReadOnlyCollection<EntityDefinition> IDefinitionMap.Entities => Entities;

        internal List<string> ActiveEntityDefinitions { get; }

        internal List<string> ActivePropertyDefinitions { get; }

        internal List<EntityDefinition> Entities { get; }

        public DefinitionMap()
        {
            ActiveEntityDefinitions = new List<string>();
            ActivePropertyDefinitions = new List<string>();
            Entities = new List<EntityDefinition>();
        }

        public EntityDefinitionBuilder Add(string name)
        {
            var entityDefinition = new EntityDefinition(name, ActiveEntityDefinitions.Count);
            Entities.Add(entityDefinition);

            return new EntityDefinitionBuilder(this, entityDefinition);
        }
    }

    public interface IDefinitionMap
    {
        IReadOnlyCollection<string> ActiveEntityDefinitions { get; }
        IReadOnlyCollection<string> ActivePropertyDefinitions { get; }
        IReadOnlyCollection<EntityDefinition> Entities { get; }
    }

    public abstract class DefinitionBase
    {
        protected DefinitionBase(string name, int valuesCount)
        {
            Name = name;
            Values = new Dictionary<string, object>(valuesCount);
        }

        public string Name { get; }
        public Dictionary<string, object> Values { get; }
    }

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

    public sealed class PropertyDefinition : DefinitionBase
    {
        /// <inheritdoc />
        public PropertyDefinition(string name, int valuesCount)
            : base(name, valuesCount)
        {
        }
    }

    public class Builder<T>
        where T : DefinitionBase
    {
        internal ICollection<string> ActiveMap { get; }

        public Builder(ICollection<string> activeMap, T obj)
        {
            ActiveMap = activeMap;
            Obj = obj;
        }

        protected T Obj { get; }

        public void AddDefinition(DefinitionAttribute definition)
        {
            AddDefinition(definition.Name, definition.Value);
        }

        public void AddDefinition(string name, object value)
        {
            if (ActiveMap.SingleOrDefault(v => v == name) is null)
            {
                ActiveMap.Add(name);
            }

            Obj.Values.Add(name, value);
        }
    }

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