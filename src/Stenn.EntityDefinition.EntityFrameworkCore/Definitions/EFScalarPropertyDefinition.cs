using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public sealed class EFScalarPropertyDefinition<T> : EFPropertyDefinition<T>
    {
        /// <inheritdoc />
        public EFScalarPropertyDefinition(string name,
            Func<IProperty, PropertyInfo?,DefinitionContext, T?> extract, Func<T, string>? convertToString = null)
            : base(name,
                (property, propertyInfo, context) =>
                {
                    if (property is IProperty p)
                    {
                        return extract(p, propertyInfo, context);
                    }
                    return default;
                }, convertToString)
        {
            if (extract == null)
            {
                throw new ArgumentNullException(nameof(extract));
            }
        }
    }
}