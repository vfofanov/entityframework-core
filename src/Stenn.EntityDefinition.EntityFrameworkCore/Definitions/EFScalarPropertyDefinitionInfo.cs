using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public sealed class EFScalarPropertyDefinitionInfo<T> : EFPropertyDefinitionInfo<T>
    {
        /// <inheritdoc />
        public EFScalarPropertyDefinitionInfo(string name,
            Func<IProperty, PropertyInfo?, IEFDefinitionExtractContext, T?> extract, Func<T, string>? convertToString = null)
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