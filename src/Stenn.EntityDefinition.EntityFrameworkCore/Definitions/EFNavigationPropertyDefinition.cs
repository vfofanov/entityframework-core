using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stenn.EntityDefinition.Contracts.Definitions;

namespace Stenn.EntityDefinition.EntityFrameworkCore.Definitions
{
    public sealed class EFNavigationPropertyDefinition<T> : EFPropertyDefinition<T>
    {
        /// <inheritdoc />
        public EFNavigationPropertyDefinition(string name,
            Func<INavigation?, PropertyInfo?, T?, DefinitionContext, T?> extract, Func<T, string>? convertToString = null)
            : base(name,
                (property, propertyInfo, parentValue, context) =>
                {
                    if (property is INavigation p)
                    {
                        return extract(p, propertyInfo, parentValue, context);
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