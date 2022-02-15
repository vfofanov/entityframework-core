using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.DictionaryEntities;

namespace Stenn.EntityFrameworkCore.StaticMigrations
{
    public class DictionaryEntityInfoFactory
    {
        /// <summary>
        /// Create dictionary entity info from CLR type and <see cref="IModel"/>
        /// </summary>
        /// <param name="model"></param>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static DictionaryEntityInfo<T> Create<T>(IModel model)
        {
            var entityType = typeof(T);
            var modelType = model.FindEntityType(entityType);
            if (modelType == null)
            {
                throw new ArgumentException($"{entityType} doesn't present in db model", nameof(entityType));
            }

            var ignoredProperties = entityType.GetCustomAttributes<DictionaryEntityIgnoredPropertiesAttribute>()
                .SelectMany(a => a.PropertiesNames).Distinct().ToList();
            var keyProperties = entityType.GetCustomAttribute<DictionaryEntityKeyPropertiesAttribute>()?.PropertiesNames ?? ArraySegment<string>.Empty;

            var propertyInfos = entityType.GetProperties(BindingFlags.Public |BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);

            List<PropertyInfo> keys = new();
            List<PropertyInfo> properties = new();

            foreach (var property in propertyInfos)
            {
                if (ignoredProperties.Contains(property.Name) ||
                    property.GetCustomAttribute<DictionaryEntityIgnoreAttribute>() != null)
                {
                    continue;
                }
                var modelProperty = modelType.FindProperty(property);
                if (modelProperty == null)
                {
                    //Skip property if it doesn't present in model
                    continue;
                }

                if (modelProperty.IsPrimaryKey() ||
                    keyProperties.Contains(property.Name) ||
                    property.GetCustomAttribute<DictionaryEntityKeyAttribute>() != null)
                {
                    keys.Add(property);
                    continue;
                }
                if (modelProperty.IsConcurrencyToken || modelProperty.ValueGenerated != ValueGenerated.Never)
                {
                    //Skip db generated property
                    continue;
                }
                properties.Add(property);
            }

            keys.Sort((one, other) => string.CompareOrdinal(one.Name, other.Name));
            properties.Sort((one, other) => string.CompareOrdinal(one.Name, other.Name));

            return new DictionaryEntityInfo<T>(keys.ToArray(), properties.ToArray());
        }
    }
}