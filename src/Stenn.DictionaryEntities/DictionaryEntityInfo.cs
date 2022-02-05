using System;
using System.Diagnostics;
using System.Reflection;

namespace Stenn.DictionaryEntities
{
    [DebuggerDisplay("{EntityType.Name}")]
    public abstract class DictionaryEntityInfo
    {
        public DictionaryEntityInfo(Type entityType, PropertyInfo[] keys, PropertyInfo[] properties)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));

            Keys = keys ?? throw new ArgumentNullException(nameof(keys));
            if (keys.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(keys));
            }

            foreach (var key in Keys)
            {
                if (key.GetMethod == null)
                {
                    throw new NotSupportedException($"Property '{key.Name}' of type '{entityType}' must have get method");
                }
            }

            Properties = properties ?? throw new ArgumentNullException(nameof(properties));

            foreach (var property in Properties)
            {
                if (property.GetMethod == null)
                {
                    throw new NotSupportedException($"Property '{property.Name}' of type '{entityType}' must have get method");
                }
                if (property.SetMethod == null)
                {
                    throw new NotSupportedException($"Property '{property.Name}' of type '{entityType}' must have set method");
                }
            }
        }

        public Type EntityType { get; }
        public PropertyInfo[] Keys { get; }
        public PropertyInfo[] Properties { get; }
    }

    public sealed class DictionaryEntityInfo<T> : DictionaryEntityInfo
    {
        /// <inheritdoc />
        public DictionaryEntityInfo(PropertyInfo[] keys, PropertyInfo[] properties) 
            : base(typeof(T), keys, properties)
        {
        }

        public bool EqualsByKey(T one, T other)
        {
            for (var i = 0; i < Keys.Length; i++)
            {
                var key = Keys[i];
                if (!Equals(key.GetValue(one), key.GetValue(other)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool EqualsByProperties(T one, T other)
        {
            for (var i = 0; i < Properties.Length; i++)
            {
                var property = Properties[i];
                if (!Equals(property.GetValue(one), property.GetValue(other)))
                {
                    return false;
                }
            }
            return true;
        }

        public void CopyPropertiesFrom(T source, T destination)
        {
            for (var i = 0; i < Properties.Length; i++)
            {
                var property = Properties[i];
                var value = property.GetValue(source);
                property.SetValue(destination, value);
            }
        }
    }
}